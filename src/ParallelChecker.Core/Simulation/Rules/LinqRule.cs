using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class LinqRule : Rule<LinqBlock> {
    public override int TimeCost => 10;

    public override void Apply(Program program, LinqBlock block) {
      var method = program.ActiveMethod;
      var instance = method.EvaluationStack.Pop();
      if (instance == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      var query = instance.ExtractQuery();
      switch (block.Kind) {
        case LinqKind.Unknown:
          method.EvaluationStack.Push(new UnknownQuery(query));
          break;
        case LinqKind.From:
          method.EvaluationStack.Push(query);
          break;
        case LinqKind.Where:
          if (block.Parameter == null || block.Expression == null) {
            method.EvaluationStack.Push(new UnknownQuery(query));
          } else {
            var predicate = new LinqExpression(block.Parameter, block.Expression, method);
            method.EvaluationStack.Push(new WhereQuery(query, predicate));
          }
          break;
        case LinqKind.Select:
          if (block.Parameter == null || block.Expression == null) {
            method.EvaluationStack.Push(new UnknownQuery(query));
          } else {
            var mapping = new LinqExpression(block.Parameter, block.Expression, method);
            method.EvaluationStack.Push(new SelectQuery(query, mapping));
          }
          break;
        default:
          throw new NotImplementedException();
      }
      program.GoToNextBlock();
    }
  }
}
