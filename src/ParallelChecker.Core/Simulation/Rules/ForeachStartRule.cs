using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ForeachStartRule : Rule<IteratorStartBlock> {
    public override int TimeCost => 8;

    // TODO: Support custom collection sub-classes and iteration over characters in strings
    public override void Apply(Program program, IteratorStartBlock block) {
      var method = program.ActiveMethod;
      var instance = method.EvaluationStack.Pop();
      if (instance == null) {
        throw new Exception(program.ActiveLocation, new System.NullReferenceException());
      }
      var query = instance.ExtractQuery();
      method.OpenIterators.Push(new Iterator(query));
      program.GoToNextBlock();
    }
  }
}
