using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class MonitorEnterRule : Rule<InvocationBlock> {
    public override int TimeCost => 5;

    public override bool Applicable(Program program, InvocationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Method)) {
        return false;
      }
      return block.Method.Is(Symbols.MonitorEnter);
    }
    
    public override void Apply(Program program, InvocationBlock block) {
      var thread = program.ActiveThread;
      if (thread.NestedLocks > 0 && program.Random.Next(SimulationBounds.NestedLockScheduling) > 0) {
        return;
      }
      var callee = block.Method;
      var method = program.ActiveMethod;
      int nofParameters = callee.Parameters.Length;
      var arguments = method.CollectArguments(nofParameters);
      var instance = arguments[0];
      if (instance == null) {
        throw new Exception(program.ActiveLocation, "Monitor lock on null");
      }
      var target = instance as Object ?? Unknown.Value;
      if (!program.TryMonitorEnter(target)) {
        method.PutBackArguments(arguments);
      }
    }
  }
}
