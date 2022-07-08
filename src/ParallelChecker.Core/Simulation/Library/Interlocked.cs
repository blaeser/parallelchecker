using Microsoft.CodeAnalysis.CSharp;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading")]
  internal static class Interlocked {
    [Member]
    public static object Add(Program program, Variable variable, object increment) {
      program.RecordVariations();
      program.SyncVolatileRead(variable);
      program.RecordRead(new Access(variable, true));
      if (variable.Value == Unknown.Value || increment == Unknown.Value) {
        variable.Value = Unknown.Value;
      } else {
        variable.Value = SyntaxKind.AddExpression.Apply(variable.Value, increment);
      }
      program.RecordWrite(new Access(variable, true));
      program.SyncVolatileWrite(variable);
      return variable.Value;
    }

    [Member]
    public static object CompareExchange(Program program, Variable variable, object newValue, object comparand) {
      program.RecordVariations();
      program.SyncVolatileRead(variable);
      program.RecordRead(new Access(variable, true));
      var oldValue = variable.Value;
      if (oldValue != Unknown.Value && comparand != Unknown.Value && 
        SyntaxKind.EqualsExpression.Apply(oldValue, comparand).Equals(true)) {
        variable.Value = newValue;
        program.RecordWrite(new Access(variable, true));
        program.SyncVolatileWrite(variable);
      }
      return oldValue;
    }

    [Member]
    public static object Increment(Program program, Variable variable) {
      return Add(program, variable, 1);
    }

    [Member]
    public static object Decrement(Program program, Variable variable) {
      return Add(program, variable, -1);
    }

    [Member]
    public static object Exchange(Program program, Variable variable, object newValue) {
      return CompareExchange(program, variable, newValue, variable.Value);
    }

    [Member]
    public static void MemoryBarrier(Program program) {
      program.RecordVariations();
      program.ActiveThread.AdvanceTime();
    }
  }
}
