using System;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System")]
  internal sealed class Range : Model.Object {
    private readonly object _start;
    private readonly object _end;

    [Member]
    public Range(object start, object end) {
      _start = start ?? new Index(0, false);
      _end = end ?? new Index(0, true);
    }

    [Member]
    public object GetStart() {
      return _start;
    }

    [Member]
    public object GetEnd() {
      return _end;
    }

    [Member]
    public new bool Equals(object other) {
      return other is Range otherRange && _start is Index startIndex && _end is Index endIndex &&
        startIndex.Equals(otherRange._start) is bool startResult &&
        endIndex.Equals(otherRange._end) is bool endResult &&
        startResult && endResult;
    }

    [Member]
    public static Range GetAll() {
      return new Range(new Index(0, false), new Index(0, true));
    }

    public bool IsConcrete => _start is Index startIndex && _end is Index endIndex && startIndex.IsConcrete && endIndex.IsConcrete;
  }
}
