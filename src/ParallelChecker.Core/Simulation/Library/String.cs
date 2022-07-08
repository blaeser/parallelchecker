using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System")]
  internal static class String {
    [Member]
    public static readonly string Empty = string.Empty;

    [Member]
    public static object GetLength(object value) {
      if (value is string stringValue) {
        return stringValue.Length;
      } else {
        return Unknown.Value;
      }
    }

    [Member]
    public static object IsNullOrEmpty(object value) {
      if (value is string stringValue) {
        return string.IsNullOrEmpty(stringValue);
      } else {
        return Unknown.Value;
      }
    }

    [Member]
    public static object IsNullOrWhiteSpace(object value) {
      if (value is string stringValue) {
        return string.IsNullOrWhiteSpace(stringValue);
      } else {
        return Unknown.Value;
      }
    }

    [Member]
#pragma warning disable IDE1006 // Naming Styles
    public static string op_Addition(object operand1, object operand2) {
#pragma warning restore IDE1006 // Naming Styles
      var string1 = operand1.ConvertToString();
      var string2 = operand2.ConvertToString();
      return string1 + string2;
    }

    [Member]
#pragma warning disable IDE1006 // Naming Styles
    public static bool op_Equality(object operand1, object operand2) {
#pragma warning restore IDE1006 // Naming Styles
      var string1 = operand1.ConvertToString();
      var string2 = operand2.ConvertToString();
      return string1 == string2;
    }

    [Member]
#pragma warning disable IDE1006 // Naming Styles
    public static bool op_Inequality(object operand1, object operand2) {
#pragma warning restore IDE1006 // Naming Styles
      var string1 = operand1.ConvertToString();
      var string2 = operand2.ConvertToString();
      return string1 != string2;
    }
  }
}
