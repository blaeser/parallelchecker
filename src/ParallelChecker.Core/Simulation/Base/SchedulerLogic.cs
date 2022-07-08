using Microsoft.CodeAnalysis;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Base {
  internal static class SchedulerLogic {
    public static Location CurrentLocation(this Program program) {
      return CurrentLocation(program, program.ActiveThread);
    }

    public static Location CurrentLocation(this Program program, Thread thread) {
      var compilation = program.CompilationModel.Compilation;
      foreach (var method in thread.CallStack) {
        var location = method.ActiveBlock.Location;
        if (program.Options.AllowExternalLocations || compilation.ContainsSyntaxTree(location.SourceTree)) {
          return location;
        }
      }
      return Location.None;
    }

    public static void RecordVariations(this Program program) {
      program.Variations += Math.Max(0, program.RunnableThreads.Count - 1);
    }

    public static Thread SelectRunnableThread(this Program program) {
      if (program.StaticLoader.State == ThreadState.Runnable) {
        return program.StaticLoader;
      }
      var candidates = program.RunnableThreads;
      if (candidates.Count == 0) {
        return null;
      }
      return candidates.PickRandom(program.Random);
    }

    public static void GoToNextBlock(this Program program) {
      var block = program.ActiveBlock;
      program.GoToNextBlock(((StraightBlock)block).Successor);
    }

    public static void GoToNextBlock(this Program program, BasicBlock block) {
      program.PreviousLocation = program.ActiveLocation;
      var method = program.ActiveMethod;
      method.ActiveBlock = block ?? throw new System.Exception();
    }
  }
}
