using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ObjectCloneRule : Rule<ObjectCloneBlock> {
    public override int TimeCost => 10;

    public override void Apply(Program program, ObjectCloneBlock block) {
      var method = program.ActiveMethod;
      var instance = method.EvaluationStack.Pop();
      if (instance is ValueTuple tuple) {
        instance = tuple.TupleClone();
      } else if (instance is Object objectInstance) {
        var type = objectInstance.Type;
        if (type != null) {
          instance = type.ObjectClone(instance, block.CloneRecord);
        }
      } 
      method.EvaluationStack.Push(instance);
      program.GoToNextBlock();
    }
  }
}
