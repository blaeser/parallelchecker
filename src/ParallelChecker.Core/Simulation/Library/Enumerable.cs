using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Linq")]
  internal static class Enumerable {
    [Member]
    public static object Where(Program program, object input, object predicate) {
      if (input == null || predicate == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value || predicate == Unknown.Value) {
        return Unknown.Value;
      }
      var baseQuery = ExtractQuery(input);
      return new WhereQuery(baseQuery, predicate);
    }

    [Member]
    public static object Select(Program program, object input, object mapping) {
      if (input == null || mapping == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value || mapping == Unknown.Value) {
        return Unknown.Value;
      }
      var baseQuery = ExtractQuery(input);
      return new SelectQuery(baseQuery, mapping);
    }

    [Member]
    public static object Any(Program program, object input) {
      if (input == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value) {
        return Unknown.Value;
      }
      var query = ExtractQuery(input);
      if (!query.TryFetchNext(program)) {
        throw new RetryException();
      }
      return query.Available.Count > 0;
    }

    [Member]
    public static object Contains(Program program, object input, object value) {
      if (input == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value) {
        return Unknown.Value;
      }
      var query = ExtractQuery(input);
      if (!query.TryFetchAll(program)) {
        throw new RetryException();
      }
      if (value == Unknown.Value) {
        return Unknown.Value;
      }
      return query.Available.Contains(value);
    }

    [Member]
    public static object Reverse(Program program, object input) {
      if (input == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value) {
        return Unknown.Value;
      }
      var query = ExtractQuery(input);
      if (!query.TryFetchAll(program)) {
        throw new RetryException();
      }
      return new FinalQuery(query.RaceTarget, query.Available.AsEnumerable().Reverse());
    }

    [Member]
    public static object Single(Program program, object input) {
      if (input == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value) {
        return Unknown.Value;
      }
      var query = ExtractQuery(input);
      if (!query.TryFetchAll(program)) {
        throw new RetryException();
      }
      if (query.Available.Count != 1) {
        throw new Model.Exception(program.ActiveLocation, new InvalidOperationException());
      }
      return query.Available[0];
    }

    [Member]
    public static object SingleOrDefault(Program program, object input) {
      if (input == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value) {
        return Unknown.Value;
      }
      var query = ExtractQuery(input);
      if (!query.TryFetchAll(program)) {
        throw new RetryException();
      }
      if (query.Available.Count > 1) {
        throw new Model.Exception(program.ActiveLocation, new InvalidOperationException());
      } else if (query.Available.Count == 1) {
        return query.Available[0];
      } else {
        // TODO: infer base type and return its default
        return Unknown.Value;
      }
    }

    [Member]
    public static object First(Program program, object input) {
      if (input == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value) {
        return Unknown.Value;
      }
      var query = ExtractQuery(input);
      if (!query.TryFetchNext(program)) {
        throw new RetryException();
      }
      if (query.Available.Count == 0) {
        throw new Model.Exception(program.ActiveLocation, new InvalidOperationException());
      }
      return query.Available.First();
    }

    [Member]
    public static object FirstOrDefault(Program program, object input) {
      if (input == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value) {
        return Unknown.Value;
      }
      var query = ExtractQuery(input);
      if (!query.TryFetchNext(program)) {
        throw new RetryException();
      }
      if (query.Available.Count == 0) {
        // TODO: infer base type and return its default
        return Unknown.Value;
      }
      return query.Available.First();
    }

    [Member]
    public static object Last(Program program, object input) {
      if (input == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value) {
        return Unknown.Value;
      }
      var query = ExtractQuery(input);
      if (!query.TryFetchAll(program)) {
        throw new RetryException();
      }
      if (query.Available.Count == 0) {
        throw new Model.Exception(program.ActiveLocation, new InvalidOperationException());
      }
      return query.Available.Last();
    }

    [Member]
    public static object LastOrDefault(Program program, object input) {
      if (input == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value) {
        return Unknown.Value;
      }
      var query = ExtractQuery(input);
      if (!query.TryFetchAll(program)) {
        throw new RetryException();
      }
      if (query.Available.Count == 0) {
        // TODO: infer base type and return its default
        return Unknown.Value;
      }
      return query.Available.Last();
    }

    [Member]
    public static object Count(Program program, object input) {
      if (input == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value) {
        return Unknown.Value;
      }
      var query = ExtractQuery(input);
      if (!query.TryFetchAll(program)) {
        throw new RetryException();
      }
      return query.Available.Count;
    }

    [Member]
    public static object LongCount(Program program, object input) {
      if (input == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value) {
        return Unknown.Value;
      }
      var query = ExtractQuery(input);
      if (!query.TryFetchAll(program)) {
        throw new RetryException();
      }
      return query.Available.LongCount();
    }

    [Member]
    public static object Union(Program program, object first, object second) {
      if (first == null || second == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (first == Unknown.Value || second == null) {
        return Unknown.Value;
      }
      var firstQuery = ExtractQuery(first);
      var secondQuery = ExtractQuery(second);
      // TODO: combined query with lazy evaluation
      if (!firstQuery.TryFetchAll(program) || !secondQuery.TryFetchAll(program)) {
        throw new RetryException();
      }
      return new FinalQuery(firstQuery.RaceTarget, firstQuery.Available.Union(secondQuery.Available));
    }

    [Member]
    public static object Intersect(Program program, object first, object second) {
      if (first == null || second == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (first == Unknown.Value || second == null) {
        return Unknown.Value;
      }
      var firstQuery = ExtractQuery(first);
      var secondQuery = ExtractQuery(second);
      // TODO: combined query with lazy evaluation
      if (!firstQuery.TryFetchAll(program) || !secondQuery.TryFetchAll(program)) {
        throw new RetryException();
      }
      return new FinalQuery(firstQuery.RaceTarget, firstQuery.Available.Intersect(secondQuery.Available));
    }

    [Member]
    public static object Except(Program program, object first, object second) {
      if (first == null || second == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (first == Unknown.Value || second == null) {
        return Unknown.Value;
      }
      var firstQuery = ExtractQuery(first);
      var secondQuery = ExtractQuery(second);
      // TODO: combined query with lazy evaluation
      if (!firstQuery.TryFetchAll(program) || !secondQuery.TryFetchAll(program)) {
        throw new RetryException();
      }
      return new FinalQuery(firstQuery.RaceTarget, firstQuery.Available.Except(secondQuery.Available));
    }

    public static Query ExtractQuery(this object input) {
      if (input == null) {
        return null;
      } else if (input is string text) {
        return new FinalQuery(null, text.ToArray());
      } else if (input == Unknown.Value) {
        return new FinalQuery(null, new object[] { });
      } else if (input is Query query) {
        return query;
      } else if (input is Model.Array array) {
        return new FinalQuery(null, array.AllValues());
      } else if (input is SystemObject collection) {
        return new FinalQuery(collection, (IEnumerable)collection.NativeInstance);
      } else if (input is IEnumerable enumerable) {
        return new FinalQuery(null, enumerable);
      } else if (input is Model.Object) { // e.g. invented object
        return new FinalQuery(null, new object[] { });
      } else {
        throw new NotImplementedException();
      }
    }
  }
}
