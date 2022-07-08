using Microsoft.CodeAnalysis;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class AliasRule : Rule<AliasBlock> {
    public override int TimeCost => 5;

    public override void Apply(Program program, AliasBlock block) {
      var method = program.ActiveMethod;
      var value = method.EvaluationStack.Pop();
      if (value is Variable right && block.Variable is ILocalSymbol && program.IsDefinedVariable(block.Variable)) {
        var scope = program.GetVariableScope(block.Variable);
        scope.SetAlias(block.Variable, right);
      }
      program.GoToNextBlock();
    }
  }
}
