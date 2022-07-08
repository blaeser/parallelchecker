using System;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class Iterator {
    public Query Query { get; }
    private int index = -1;

    public Iterator(Query query) {
      Query = query ?? throw new ArgumentNullException(nameof(query));
    }

    public bool TryFetchNext(Program program) {
      if (index + 1 >= Query.Available.Count && !Query.FullyEvaluated) {
        return Query.TryFetchNext(program);
      }
      return true;
    }

    public bool HasNext {
      get {
        CheckAvailable();
        return index < Query.Available.Count;
      }
    }

    public bool MoveNext() {
      if (HasNext) {
        index++;
      }
      return HasNext;
    }

    public object Current {
      get {
        if (index < 0) {
          throw new System.Exception("MoveNext required before Current");
        }
        CheckAvailable();
        return Query.Available[index];
      }
    }

    private void CheckAvailable() {
      if (index >= Query.Available.Count && !Query.FullyEvaluated) {
        throw new System.Exception("Query needs further evaluation");
      }
    }
  }
}
