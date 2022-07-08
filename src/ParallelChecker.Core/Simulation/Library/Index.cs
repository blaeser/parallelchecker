using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System")]
  internal sealed class Index : Model.Object {
    private readonly object _value;
    private readonly object _fromEnd;

    public Index(object value, object fromEnd) {
      _value = value;
      _fromEnd = fromEnd;
    }

    [Member]
    public object GetValue() {
      return _value;
    }

    [Member]
    public object GetIsFromEnd() {
      return _fromEnd;
    }

    [Member]
    public new object Equals(object other) {
      if (other == Unknown.Value || _value == Unknown.Value || _fromEnd == Unknown.Value) {
        return Unknown.Value;
      } else if (other is Index otherIndex) {
        return Equals(_value, otherIndex._value) && Equals(_fromEnd, otherIndex._fromEnd);
      }
      return false;
    }

    [Member("op_Implicit")]
    public static Index ImplicitCast(object value) {
      return new Index(value, false);
    }

    public bool IsConcrete => _value is int && _fromEnd is bool;
  }
}
