using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ForeachNextRule : Rule<IteratorNextBlock> {
    public override int TimeCost => 8; 

    public override void Apply(Program program, IteratorNextBlock block) {
      var method = program.ActiveMethod;
      var iterator = method.OpenIterators.Peek();
      if (!iterator.TryFetchNext(program)) {
        return;
      }
      var query = iterator.Query;
      if (query.RaceTarget?.Type != null) {
        program.RecordCall(new Call(query.RaceTarget.Type, query.RaceTarget));
      }
      bool hasNext;
      try {
        hasNext = iterator.MoveNext();
      } catch (InvalidOperationException exception) {
        throw new Model.Exception(program.ActiveLocation, exception);
      }
      if (hasNext) {
        // foreach defines new local variable per iteration step
        var local = new ExplicitVariable(block.Variable) {
          Value = iterator.Current
        };
        method.LocalVariables.SetExplicit(block.Variable, local);
        program.GoToNextBlock(block.SuccessorOnContinue);
      } else {
        program.GoToNextBlock(block.SuccessorOnFinished);
      }
    }
  }
}
