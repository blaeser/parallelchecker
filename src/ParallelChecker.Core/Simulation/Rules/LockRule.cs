using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class LockRule : Rule<LockBlock> {
    public override int TimeCost => 5;

    public override void Apply(Program program, LockBlock block) {
      var thread = program.ActiveThread;
      if (thread.NestedLocks > 0 && program.Random.Next(SimulationBounds.NestedLockScheduling) > 0) {
        return;
      }
      var method = thread.ActiveMethod;
      var value = method.EvaluationStack.Pop();
      if (value == null) {
        throw new Exception(program.ActiveLocation, "Monitor lock on null");
      }
      var monitor = value as Object ?? Unknown.Value;
      if (program.TryMonitorEnter(monitor)) {
        method.OpenLocks.Push(monitor);
      } else {
        method.EvaluationStack.Push(monitor);
      }
    }
  }
}
