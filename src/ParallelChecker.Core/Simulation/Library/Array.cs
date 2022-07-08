using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System")]
  internal static class Array {
    [Member]
    public static int GetLength(Program program, Model.Array array, int dimension) {
      if (dimension < 0 || dimension >= array.Lengths.Length) {
        throw new Model.Exception(program.ActiveLocation, new IndexOutOfRangeException("Invalid dimension"));
      }
      return array.Lengths[dimension];
    }

    [Member]
    public static int GetLength(Program program, Model.Array array) {
      return GetLength(program, array, 0);
    }
  }
}
