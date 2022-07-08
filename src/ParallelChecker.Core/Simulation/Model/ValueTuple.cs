using System;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class ValueTuple : Object {
    public Variable[] Items { get; }

    public ValueTuple(Variable[] items) {
      Items = items ?? throw new ArgumentNullException(nameof(items));
    }

    public ValueTuple TupleClone() {
      var copy = new Variable[Items.Length];
      for (int index = 0; index < Items.Length; index++) {
        copy[index] = Items[index].Clone();
      }
      return new ValueTuple(copy);
    }
  }
}
