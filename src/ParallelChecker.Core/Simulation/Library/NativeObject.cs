using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System", "Object")]
  internal sealed class NativeObject {
    [Member]
    public static new object ReferenceEquals(object first, object second) {
      if (first == Unknown.Value || second == Unknown.Value) {
        return Unknown.Value;
      }
      return object.ReferenceEquals(first, second);
    }
  }
}
