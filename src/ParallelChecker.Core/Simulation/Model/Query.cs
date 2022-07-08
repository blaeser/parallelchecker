using ParallelChecker.Core.Simulation.Base;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Model {
  internal abstract class Query {
    public List<object> Available { get; } = new();
    public abstract bool FullyEvaluated { get; }
    public abstract Object RaceTarget { get; }
    public abstract bool IsParallel { get; }

    public IEnumerable AllItems {
      get {
        if (!FullyEvaluated) {
          throw new System.Exception("Query is not fully evaluated");
        }
        return Available;
      }
    }

    public abstract bool TryFetchNext(Program program);

    public bool TryFetchAll(Program program) {
      while (!FullyEvaluated) {
        if (!TryFetchNext(program)) {
          return false;
        }
        program.IncreaseSimulationTime(1);
      }
      return true;
    }

    protected static Thread LaunchLinqFunction(Program program, bool parallel, object function, object argument) {
      if (parallel) {
        var subTask = program.CreateThread(function);
        // TODO: unify this, introduce symbol-invariant parameter info in methods
        if (function is LinqExpression linqExpression) {
          subTask.ActiveMethod.LocalVariables[linqExpression.Parameter].Value = argument;
        } else {
          subTask.PassSingleParameter(0, argument);
        }
        program.StartThread(subTask);
        var state = new JoinWaitState();
        state.AwaitedThreads.Add(subTask);
        program.ActiveThread.WaitState = state;
        return subTask;
      } else {
        program.InvokeAnyCallable(function, new object[] { argument });
        return program.ActiveThread;
      }
    }
  }

  internal sealed class FinalQuery : Query {
    private readonly Object _target;

    public FinalQuery(Object target, IEnumerable enumerable) {
      _target = target;
      foreach (var item in enumerable) {
        Available.Add(item);
      }
    }
    
    public override bool FullyEvaluated => true;
    public override Object RaceTarget => _target;
    public override bool IsParallel => false;

    public override bool TryFetchNext(Program program) {
      return true;
    }
  }

  internal abstract class FunctionalQuery : Query {
    public Iterator BaseIterator { get; }
    public object Function { get; }
    private int _pending = 0;

    public FunctionalQuery(Query baseQuery, object function) {
      BaseIterator = new Iterator(baseQuery ?? throw new ArgumentNullException(nameof(baseQuery)));
      Function = function ?? throw new ArgumentNullException(nameof(function));
    }

    public Query BaseQuery => BaseIterator.Query;
    public override bool FullyEvaluated => BaseQuery.FullyEvaluated && !BaseIterator.HasNext;
    public override Object RaceTarget => BaseQuery.RaceTarget;
    public override bool IsParallel => BaseQuery.IsParallel;

    public override bool TryFetchNext(Program program) {
      program.IncreaseSimulationTime(10);
      if (_pending > 0 || !BaseIterator.TryFetchNext(program)) {
        return false;
      }
      if (BaseIterator.MoveNext()) {
        var value = BaseIterator.Current;
        if (value == Unknown.Value) {
          Available.Add(value);
        } else {
          var evaluationThread = LaunchLinqFunction(program, IsParallel, Function, value);
          _pending++;
          evaluationThread.ActiveMethod.ResultInterceptor = result => {
            HandleResult(value, result);
            _pending--;
          };
          return false;
        }
      }
      return true;
    }

    protected abstract void HandleResult(object input, object output);
  }

  internal sealed class WhereQuery : FunctionalQuery {
    public WhereQuery(Query baseQuery, object predicate) 
      : base(baseQuery, predicate) {
    }

    protected override void HandleResult(object input, object output) {
      if (output is bool boolResult && boolResult) {
        Available.Add(input);
      }
    }
  }

  internal sealed class SelectQuery : FunctionalQuery {
    public SelectQuery(Query baseQuery, object mapping)
      : base(baseQuery, mapping) {
    }

    protected override void HandleResult(object input, object output) {
      Available.Add(output);
    }
  }

  internal sealed class ParallelModeQuery : Query {
    public Iterator BaseIterator { get; }
    public bool _parallel;

    public ParallelModeQuery(Query baseQuery, bool parallel) {
      BaseIterator = new Iterator(baseQuery ?? throw new ArgumentNullException(nameof(baseQuery)));
      _parallel = parallel;
    }

    public Query BaseQuery => BaseIterator.Query;
    public override bool FullyEvaluated => BaseQuery.FullyEvaluated && !BaseIterator.HasNext;
    public override Object RaceTarget => BaseQuery.RaceTarget;
    public override bool IsParallel => _parallel;

    public override bool TryFetchNext(Program program) {
      program.IncreaseSimulationTime(10);
      if (!BaseIterator.TryFetchNext(program)) {
        return false;
      }
      if (BaseIterator.MoveNext()) {
        var value = BaseIterator.Current;
        Available.Add(value);
      }
      return true;
    }
  }

  internal sealed class UnknownQuery : Query {
    public Iterator BaseIterator { get; }

    public UnknownQuery(Query baseQuery) {
      BaseIterator = new Iterator(baseQuery ?? throw new ArgumentNullException(nameof(baseQuery)));
    }

    public Query BaseQuery => BaseIterator.Query;
    public override bool FullyEvaluated => BaseQuery.FullyEvaluated && !BaseIterator.HasNext;
    public override Object RaceTarget => BaseQuery.RaceTarget;
    public override bool IsParallel => BaseQuery.IsParallel;

    public override bool TryFetchNext(Program program) {
      program.IncreaseSimulationTime(10);
      if (!BaseIterator.TryFetchNext(program)) {
        return false;
      }
      if (BaseIterator.MoveNext()) {
        Available.Add(Unknown.Value);
      }
      return true;
    }
  }
}
