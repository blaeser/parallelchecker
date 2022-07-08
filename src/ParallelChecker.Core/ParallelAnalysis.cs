using Microsoft.CodeAnalysis;
using ParallelChecker.Core.ControlFlow;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation;
using ParallelChecker.Core.Simulation.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ParallelChecker.Core {
  public static class ParallelAnalysis {
    private const int _RandomSeed = 4711;

    public static IEnumerable<Issue> FindIssues(Compilation compilation, CancellationToken cancellationToken, AnalysisOptions options, out bool faulted) {
      var stopwatch = Stopwatch.StartNew();
      try {
        var compilationModel = new CompilationModel(compilation, cancellationToken);
        Debug.WriteLine(string.Format("Compilation model computed in {0} ms", stopwatch.ElapsedMilliseconds));
        var controlFlowModel = new ControlFlowModel(compilationModel);
        var issues = new HashSet<Issue>();
        var random = new Random(_RandomSeed);
        faulted = false;
        var allEntries = compilationModel.GetProgramEntries();
        Debug.WriteLine(string.Format("Simulation with {0} program entries", allEntries.Count));
        var divider = Math.Max(1, allEntries.Count);
        var timeBound = Math.Max(SimulationBounds.MinimumTimeBound, SimulationBounds.MaximumTimeBound / divider);
        var variationBound = Math.Max(SimulationBounds.MinimumVariationBound, SimulationBounds.MaximumVariationBound / divider);
        for (int index = 0; index < allEntries.Count; index++) {
          var entry = allEntries[index];
          Debug.WriteLine($"Analyzing entry {index} of {allEntries.Count} ({index * 100 / allEntries.Count}%)");
          var bounds = new SimulationBounds {
            TimeBound = timeBound,
            VariationBound = variationBound
          };
          RunMultipleRounds(entry, controlFlowModel, options, issues, random, bounds, ref faulted);
        }
        Debug.WriteLine(string.Format("Parallel analysis completed {0} issues {1} ms", issues.Count, stopwatch.ElapsedMilliseconds));
        return issues;
      } catch (OperationCanceledException) {
        Debug.WriteLine(string.Format("Parallel analysis cancelled {0} ms", stopwatch.ElapsedMilliseconds));
        throw;
      }
    }

    private static void RunMultipleRounds(ProgramEntry entry, ControlFlowModel controlFlowModel, AnalysisOptions options, HashSet<Issue> issues, Random random, SimulationBounds bounds, ref bool faulted) {
      // TODO: Run rounds in parallel
      long variations = 1L;
      for (long round = 0; round < variations && bounds.TotalTime < bounds.TimeBound; round++) {
        controlFlowModel.CompilationModel.ThrowIfCancellationRequested();
        var roundWatch = Stopwatch.StartNew();
        var simulation = new ProgramSimulator(controlFlowModel, options, bounds, round, random);
        try {
          simulation.Simulate(entry);
        } catch (BoundException e) {
          Debug.WriteLine(e);
        }
#if !DEBUG
        catch (Exception e) {
          if (e is OperationCanceledException) {
            throw e;
          }
          Debug.WriteLine("FAULT: " + e);
          faulted = true;
        }
#endif
        issues.AddAll(simulation.Issues);
        bounds.TotalTime += simulation.SimulationTime;
        if (bounds.TotalTime < 0) {
          throw new Exception("Negative total time");
        }
        variations = Math.Min(Math.Max(variations, simulation.Variations * 2), bounds.VariationBound);
        Debug.WriteLine(string.Format("Round completed in {0} ms {1} variations", roundWatch.ElapsedMilliseconds, variations));
      }
    }
  }
}
