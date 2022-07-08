using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ConstantRule : Rule<ConstantBlock> {
    public override int TimeCost => 1; 

    public override void Apply(Program program, ConstantBlock block) {
      if (program.InitializeStatics(block.Value as ITypeSymbol)) {
        return;
      }
      var method = program.ActiveMethod;
      var value = block.Value;
      if (value is AnonymousFunctionExpressionSyntax function) {
        var symbol = (IMethodSymbol)program.CompilationModel.GetReferencedSymbol(function);
        value = new Lambda(function, symbol, method);
      }
      if (value is IMethodSymbol delegateMethod) {
        Object delegateInstance = null;
        if (!delegateMethod.IsStaticSymbol()) {
          var thisReference = method.EvaluationStack.Pop();
          // TODO: Homogenize values in simulation. Primitive values, collections etc. should be objects
          delegateInstance = thisReference as Object ?? Unknown.Value;
        }
        Method closure = null;
        if (delegateMethod.ContainingSymbol is IMethodSymbol) {
          // local function
          closure = method;
        }
        value = new Delegate(delegateInstance, delegateMethod, closure);
      }
      method.EvaluationStack.Push(value);
      program.GoToNextBlock();
    }
  }
}
