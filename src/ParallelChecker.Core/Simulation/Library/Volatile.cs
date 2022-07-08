using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading")]
  internal static class Volatile {
    [Member]
    public static object Read(Program program, Variable variable) {
      program.SyncVolatileRead(variable);
      var access = new Access(variable, true);
      program.RecordRead(access);
      return variable.Value;
    }

    [Member]
    public static void Write(Program program, Variable variable, object value) {
      program.SyncVolatileWrite(variable);
      var access = new Access(variable, true);
      program.RecordWrite(access);
      variable.Value = value;
    }
  }
}
