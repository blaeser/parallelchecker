using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class UnlockRule : Rule<UnlockBlock> {
    public override int TimeCost => 5;

    public override void Apply(Program program, UnlockBlock block) {
      var thread = program.ActiveThread;
      var method = thread.ActiveMethod;
      var target = method.OpenLocks.Pop();
      program.MonitorExit(target);
    }
  }
}
