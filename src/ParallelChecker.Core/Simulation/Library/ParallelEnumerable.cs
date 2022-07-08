using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Linq")]
  internal static class ParallelEnumerable {
    [Member]
    public static object AsParallel(Program program, object input) {
      return SwitchParallelMode(program, input, true);
    }

    [Member]
    public static object AsSequential(Program program, object input) {
      return SwitchParallelMode(program, input, false);
    }

    private static object SwitchParallelMode(Program program, object input, bool parallel) {
      if (input == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (input == Unknown.Value) {
        return Unknown.Value;
      }
      var baseQuery = input.ExtractQuery();
      return new ParallelModeQuery(baseQuery, parallel);
    }

    [Member]
    public static object Where(Program program, object input, object predicate) {
      return Enumerable.Where(program, input, predicate);
    }

    [Member]
    public static object Select(Program program, object input, object mapping) {
      return Enumerable.Select(program, input, mapping);
    }
  }
}
