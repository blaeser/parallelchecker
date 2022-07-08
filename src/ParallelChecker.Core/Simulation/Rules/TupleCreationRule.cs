using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class TupleCreationRule : Rule<TupleCreationBlock> {
    public override int TimeCost => 5;

    public override void Apply(Program program, TupleCreationBlock block) {
      var method = program.ActiveMethod;
      var items = new Variable[block.Fields.Length];
      for (int index = items.Length - 1; index >= 0; index--) {
        var field = block.Fields[index];
        Variable variable;
        var value = method.EvaluationStack.Pop();
        if (field == null) {
          variable = new ImplicitVariable("tuple field", null, value);
        } else {
          variable = new ExplicitVariable(block.Fields[index]) {
            Value = value
          };
        }
        items[index] = variable;
      }
      var tuple = new ValueTuple(items);
      method.EvaluationStack.Push(tuple);
      program.GoToNextBlock();
    }
  }
}
