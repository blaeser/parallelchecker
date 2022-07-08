using ParallelChecker.Core.Simulation.Model;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.General;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class MonitorExitRule : Rule<InvocationBlock> {
    public override int TimeCost => 2; 

    public override bool Applicable(Program program, InvocationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Method)) {
        return false;
      }
      return block.Method.Is(Symbols.MonitorExit);
    }
    
    public override void Apply(Program program, InvocationBlock block) {
      var method = program.ActiveMethod;
      var instance = method.EvaluationStack.Pop();
      if (instance == null) {
        throw new Exception(program.ActiveLocation, "Monitor unlock on null");
      }
      var target = instance as Object ?? Unknown.Value;
      program.MonitorExit(target);
    }
  }
}
