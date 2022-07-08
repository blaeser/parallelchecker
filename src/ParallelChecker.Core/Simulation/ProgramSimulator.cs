using ParallelChecker.Core.ControlFlow;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using ParallelChecker.Core.Simulation.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.Simulation {
  internal class ProgramSimulator {
    private readonly Program _program;

    private readonly List<Rule> _rules = new() {
      new EntryRule(),
      new ExitRule(),
      new EmptyRule(),
      new DuplicateRule(),
      new DiscardRule(),
      new CastRule(),
      new ConstantRule(),
      new OperatorRule(),
      new VariableSelectionRule(),
      new ElementSelectionRule(),
      new ReadRule(),
      new WriteRule(),
      new PropertyGetRule(),
      new PropertySetRule(),
      new BranchRule(),
      new ObjectCreationRule(),
      new ArrayCreationRule(),
      new InvocationRule(),
      new ThreadCreationRule(),
      new ThreadStartRule(),
      new ThreadJoinRule(),
      new TaskMultiWaitRule(),
      new TaskResultRule(),
      new TaskRunRule(),
      new TaskContinueRule(),
      new TaskMultiContinueRule(),
      new TaskDelayRule(),
      new TaskConfigureAwaitRule(),
      new AsTaskRule(),
      new ParallelInvokeRule(),
      new ParallelForRule(),
      new LockRule(),
      new UnlockRule(),
      new MonitorEnterRule(),
      new MonitorExitRule(),
      new MonitorWaitRule(),
      new MonitorPulsingRule(),
      new ThisRule(),
      new ArrayInitializerRule(),
      new CollectionInitializerRule(),
      new ObjectCloneRule(),
      new ForeachStartRule(),
      new ForeachNextRule(),
      new ForeachEndRule(),
      new AwaitRule(),
      new ThrowRule(),
      new EnterTryRule(),
      new ExitTryRule(),
      new CatchRule(),
      new UnknownRule(),
      new SwapRule(),
      new TupleCreationRule(),
      new UndeclareRule(),
      new AliasRule(),
      new LinqRule()
    };

    private readonly MultiDictionary<Type, Rule> _ruleMap = new();

    public ProgramSimulator(ControlFlowModel controlFlowModel, AnalysisOptions options, SimulationBounds bounds, long round, Random random) {
      _program = new Program(controlFlowModel, options, bounds, round, random);
      RegisterRules();
    }

    private void RegisterRules() {
      foreach (var rule in _rules) {
        _ruleMap.Add(rule.BlockType, rule);
      }
    }

    public ICollection<Issue> Issues {
      get { return _program.Issues; }
    }

    public long SimulationTime {
      get { return _program.SimulationTime; }
    }

    public long Variations {
      get { return _program.Variations; }
    }

    public void Simulate(ProgramEntry entry) {
      _program.InitializeEntry(entry);
      SimulateExecution();
      _program.SetupMainExecution(entry);
      SimulateExecution();
      _program.CollectGarbage();
      SimulateExecution();
    }

    private void SimulateExecution() {
      while ((_program.ActiveThread = _program.SelectRunnableThread()) != null) {
        SimulateStep();
      }
    }

    private void SimulateStep() {
      _program.CompilationModel.ThrowIfCancellationRequested();
      var block = _program.ActiveBlock;
      var candidates =
        from rule in _ruleMap[block.GetType()]
        where rule.Applicable(_program)
        select rule;
      var actualRule = candidates.Single();
      try {
        actualRule.Apply(_program);
      } catch (Model.Exception exception) {
        _program.HandleException(exception);
      } finally {
        _program.IncreaseSimulationTime(actualRule.TimeCost);
      }
    }
  }
}
