using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class AsTaskRule : Rule<InvocationBlock> {
    public override int TimeCost => 5;

    public override bool Applicable(Program program, InvocationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Method)) {
        return false;
      }
      return block.Method.Is(Symbols.ValueTaskAsTask);
    }

    public override void Apply(Program program, InvocationBlock block) {
      program.GoToNextBlock();
    }
  }
}
