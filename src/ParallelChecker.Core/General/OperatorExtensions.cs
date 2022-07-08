using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.General {
  internal static class OperatorLogic {
    private interface INumericType {
      Type Type { get; }
      Func<object, object> Neg { get; }
      Func<object, object> BitNot { get; }
      Func<object, object, object> Add { get; }
      Func<object, object, object> Sub { get; }
      Func<object, object, object> Mul { get; }
      Func<object, object, object> Div { get; }
      Func<object, object, object> Mod { get; }
      Func<object, object, object> Less { get; }
      Func<object, object, object> BitAnd { get; }
      Func<object, object, object> BitOr { get; }
      Func<object, object, object> Xor { get; }
      Func<object, object, object> LeftShift { get; }
      Func<object, object, object> RightShift { get; }
      Func<object, double> Conversion1 { get; }
      Func<double, object> Conversion2 { get; }
    }

    private class NumericType<T> : INumericType {
      private readonly Func<T, object> _neg;
      private readonly Func<T, object> _bitNot;
      private readonly Func<T, T, object> _add;
      private readonly Func<T, T, object> _sub;
      private readonly Func<T, T, object> _mul;
      private readonly Func<T, T, object> _div;
      private readonly Func<T, T, object> _mod;
      private readonly Func<T, T, object> _less;
      private readonly Func<T, T, object> _bitAnd;
      private readonly Func<T, T, object> _bitOr;
      private readonly Func<T, T, object> _xor;
      private readonly Func<T, T, object> _leftShift;
      private readonly Func<T, T, object> _rightShift;
      private readonly Func<T, double> _conversion1;
      private readonly Func<double, T> _conversion2;

      public NumericType(
        Func<T, object> neg,
        Func<T, object> bitNot,
        Func<T, T, object> add,
        Func<T, T, object> sub,
        Func<T, T, object> mul,
        Func<T, T, object> div,
        Func<T, T, object> mod,
        Func<T, T, object> less,
        Func<T, T, object> bitAnd,
        Func<T, T, object> bitOr,
        Func<T, T, object> xor,
        Func<T, T, object> leftShift,
        Func<T, T, object> rightShift,
        Func<T, double> conversion1,
        Func<double, T> conversion2) {
        _neg = neg;
        _bitNot = bitNot;
        _add = add;
        _sub = sub;
        _mul = mul;
        _div = div;
        _mod = mod;
        _less = less;
        _bitAnd = bitAnd;
        _bitOr = bitOr;
        _xor = xor;
        _leftShift = leftShift;
        _rightShift = rightShift;
        _conversion1 = conversion1;
        _conversion2 = conversion2;
      }

      public Func<object, object> Neg {
        get { return x => _neg((T)x); }
      }

      public Func<object, object> BitNot {
        get { return x => _bitNot((T)x); }
      }

      public Func<object, object, object> Add {
        get { return (x, y) => _add((T)x, (T)y); }
      }

      public Func<object, object, object> Sub {
        get { return (x, y) => _sub((T)x, (T)y); }
      }

      public Func<object, object, object> Mul {
        get { return (x, y) => _mul((T)x, (T)y); }
      }

      public Func<object, object, object> Div {
        get { return (x, y) => _div((T)x, (T)y); }
      }

      public Func<object, object, object> Mod {
        get { return (x, y) => _mod((T)x, (T)y); }
      }

      public Func<object, object, object> Less {
        get { return (x, y) => _less((T)x, (T)y); }
      }

      public Func<object, object, object> BitAnd {
        get { return (x, y) => _bitAnd((T)x, (T)y); }
      }

      public Func<object, object, object> BitOr {
        get { return (x, y) => _bitOr((T)x, (T)y); }
      }

      public Func<object, object, object> Xor {
        get { return (x, y) => _xor((T)x, (T)y); }
      }

      public Func<object, object, object> LeftShift {
        get { return (x, y) => _leftShift((T)x, (T)y); }
      }

      public Func<object, object, object> RightShift {
        get { return (x, y) => _rightShift((T)x, (T)y); }
      }

      public Func<object, double> Conversion1 {
        get { return x => _conversion1((T)x); }
      }

      public Func<double, object> Conversion2 {
        get { return x => _conversion2(x); }
      }

      public Type Type {
        get { return typeof(T); }
      }
    }

    private static readonly IDictionary<SyntaxKind, int> _nofOperands = new Dictionary<SyntaxKind, int> {
      { SyntaxKind.UnaryPlusExpression, 1 },
      { SyntaxKind.UnaryMinusExpression, 1 },
      { SyntaxKind.LogicalNotExpression, 1 },
      { SyntaxKind.BitwiseNotExpression, 1 },
      { SyntaxKind.AddExpression, 2 },
      { SyntaxKind.SubtractExpression, 2 },
      { SyntaxKind.MultiplyExpression, 2 },
      { SyntaxKind.DivideExpression, 2 },
      { SyntaxKind.ModuloExpression, 2 },
      { SyntaxKind.LessThanExpression, 2 },
      { SyntaxKind.LessThanOrEqualExpression, 2 },
      { SyntaxKind.GreaterThanExpression, 2 },
      { SyntaxKind.GreaterThanOrEqualExpression, 2 },
      { SyntaxKind.EqualsExpression, 2 },
      { SyntaxKind.NotEqualsExpression, 2 },
      { SyntaxKind.BitwiseAndExpression, 2 },
      { SyntaxKind.BitwiseOrExpression, 2 },
      { SyntaxKind.ExclusiveOrExpression, 2 },
      { SyntaxKind.LeftShiftExpression, 2 },
      { SyntaxKind.RightShiftExpression, 2 },
      { SyntaxKind.IsExpression, 2 },
      { SyntaxKind.AsExpression, 2 }
    };

    // TODO: Potential numeric errors with casting from long/ulong to double and back
    // ordered by implicit type casting
    private static readonly IList<INumericType> _numericTypes = new List<INumericType> {
      new NumericType<char>(x => -x, x => ~x, (x, y) => x + y, (x, y) => x - y, (x, y) => x * y, (x, y) => x / y, (x, y) => x % y, (x, y) => x < y, (x, y) => x & y, (x, y) => x | y, (x, y) => x ^ y, (x, y) => x << y, (x, y) => x >> y, x => x, x => (char)x),
      new NumericType<byte>(x => -x, x => ~x, (x, y) => x + y, (x, y) => x - y, (x, y) => x * y, (x, y) => x / y, (x, y) => x % y, (x, y) => x < y, (x, y) => x & y, (x, y) => x | y, (x, y) => x ^ y, (x, y) => x << y, (x, y) => x >> y, x => x, x => (byte)x),
      new NumericType<sbyte>(x => -x, x => ~x, (x, y) => x + y, (x, y) => x - y, (x, y) => x * y, (x, y) => x / y, (x, y) => x % y, (x, y) => x < y, (x, y) => x & y, (x, y) => x | y, (x, y) => x ^ y, (x, y) => x << y, (x, y) => x >> y, x => x, x => (sbyte)x),
      new NumericType<short>(x => -x, x => ~x, (x, y) => x + y, (x, y) => x - y, (x, y) => x * y, (x, y) => x / y, (x, y) => x % y, (x, y) => x < y, (x, y) => x & y, (x, y) => x | y, (x, y) => x ^ y, (x, y) => x << y, (x, y) => x >> y, x => x, x => (short)x),
      new NumericType<ushort>(x => -x, x => ~x, (x, y) => x + y, (x, y) => x - y, (x, y) => x * y, (x, y) => x / y, (x, y) => x % y, (x, y) => x < y, (x, y) => x & y, (x, y) => x | y, (x, y) => x ^ y, (x, y) => x << y, (x, y) => x >> y, x => x, x => (ushort)x),
      new NumericType<int>(x => -x, x => ~x, (x, y) => x + y, (x, y) => x - y, (x, y) => x * y, (x, y) => x / y, (x, y) => x % y, (x, y) => x < y, (x, y) => x & y, (x, y) => x | y, (x, y) => x ^ y, (x, y) => x << y, (x, y) => x >> y, x => x, x => (int)x),
      new NumericType<uint>(x => -x, x => ~x, (x, y) => x + y, (x, y) => x - y, (x, y) => x * y, (x, y) => x / y, (x, y) => x % y, (x, y) => x < y, (x, y) => x & y, (x, y) => x | y, (x, y) => x ^ y, (x, y) => x << (int)y, (x, y) => x >> (int)y, x => x, x => (uint)x),
      new NumericType<long>(x => -x, x => ~x, (x, y) => x + y, (x, y) => x - y, (x, y) => x * y, (x, y) => x / y, (x, y) => x % y, (x, y) => x < y, (x, y) => x & y, (x, y) => x | y, (x, y) => x ^ y, (x, y) => x << (int)y, (x, y) => x >> (int)y, x => x, x => (long)x),
      new NumericType<ulong>(null, x => ~x, (x, y) => x + y, (x, y) => x - y, (x, y) => x * y, (x, y) => x / y, (x, y) => x % y, (x, y) => x < y, (x, y) => x & y, (x, y) => x | y, (x, y) => x ^ y, (x, y) => x << (int)y, (x, y) => x >> (int)y, x => x, x => (ulong)x),
      new NumericType<float>(x => -x, null, (x, y) => x + y, (x, y) => x - y, (x, y) => x * y, (x, y) => x / y, (x, y) => x % y, (x, y) => x < y, null, null, null, null, null, x => x, x => (float)x),
      new NumericType<double>(x => -x, null, (x, y) => x + y, (x, y) => x - y, (x, y) => x * y, (x, y) => x / y, (x, y) => x % y, (x, y) => x < y, null, null, null, null, null, x => x, x => x),
      // TODO: float/double is not implicitly convertable to decimal
      new NumericType<decimal>(x => -x, null, (x, y) => x + y, (x, y) => x - y, (x, y) => x * y, (x, y) => x / y, (x, y) => x % y, (x, y) => x < y, null, null, null, null, null, x => (double)x, x => (decimal)x) // decimal can only be casted to smaller data types
    };

    private static readonly ISet<Type> _numericTypeSet =
      new HashSet<Type>(from t in _numericTypes select t.Type);

    private static readonly IDictionary<Type, int> _numericTypeLevel =
      (from t in _numericTypes select new KeyValuePair<Type, int>(t.Type, _numericTypes.IndexOf(t))).ToDictionary();

    private static readonly IDictionary<Type, INumericType> _numericTypeDictionary =
      (from t in _numericTypes select new KeyValuePair<Type, INumericType>(t.Type, t)).ToDictionary();

    public static bool IsUnary(this SyntaxKind op) {
      return _nofOperands[op] == 1;
    }

    public static bool IsBinary(this SyntaxKind op) {
      return _nofOperands[op] == 2;
    }

    public static bool CanApply(this SyntaxKind op, object operand) {
      var operandType = operand.GetType();
      return op switch {
        SyntaxKind.UnaryPlusExpression or SyntaxKind.UnaryMinusExpression or SyntaxKind.BitwiseNotExpression => _numericTypeDictionary.ContainsKey(operandType),
        SyntaxKind.LogicalNotExpression => operand is bool,
        _ => throw new NotImplementedException(),
      };
    }

    public static object Apply(this SyntaxKind op, object operand) {
      var operandType = operand.GetType();
      switch (op) {
        case SyntaxKind.UnaryPlusExpression:
          return operand;
        case SyntaxKind.UnaryMinusExpression:
          var type = _numericTypeDictionary[operandType];
          return type.Neg(operand);
        case SyntaxKind.LogicalNotExpression:
          return !(bool)operand;
        case SyntaxKind.BitwiseNotExpression:
          type = _numericTypeDictionary[operandType];
          return type.BitNot(operand);
      }
      throw new NotImplementedException();
    }

    public static bool CanApply(this SyntaxKind op, object operand1, object operand2) {
      var type1 = operand1?.GetType();
      var type2 = operand2?.GetType();
      return IsNumeric(type1) && IsNumeric(type2) ||
        type1 == typeof(bool) && type2 == typeof(bool) ||
        op == SyntaxKind.EqualsExpression || op == SyntaxKind.NotEqualsExpression ||
        op == SyntaxKind.AddExpression;
    }

    public static object Apply(this SyntaxKind op, object operand1, object operand2) {
      var type1 = operand1?.GetType();
      var type2 = operand2?.GetType();
      if (IsNumeric(type1) && IsNumeric(type2)) {
        return ApplyNumeric(op, operand1, operand2);
      } else if (type1 == typeof(bool) && type2 == typeof(bool)) {
        return ApplyBool(op, operand1, operand2);
      } else if (op == SyntaxKind.EqualsExpression) {
        return ApplyEquals(operand1, operand2);
      } else if (op == SyntaxKind.NotEqualsExpression) {
        return !ApplyEquals(operand1, operand2);
      } else if (op == SyntaxKind.AddExpression) {
        return ApplyStringConcat(operand1, operand2);
      }
      throw new NotImplementedException();
    }

    private static object ApplyStringConcat(object operand1, object operand2) {
      var string1 = operand1.ConvertToString();
      var string2 = operand2.ConvertToString();
      return string1 + string2;
    }

    public static string ConvertToString(this object value) {
      if (value == null) {
        return null;
      }
      if (value is char charValue) {
        return charValue + string.Empty;
      }
      if (value is string stringValue) {
        return stringValue;
      }
      return value.ToString();
    }

    private static bool ApplyEquals(object operand1, object operand2) {
      if (operand1 is string && operand2 is string) {
        return operand1.Equals(operand2);
      }
      if (operand1 is Simulation.Model.ValueTuple tuple1 && operand2 is Simulation.Model.ValueTuple tuple2) {
        return TupleEquals(tuple1, tuple2);
      }
      return operand1 == operand2;
    }

    private static bool TupleEquals(Simulation.Model.ValueTuple tuple1, Simulation.Model.ValueTuple tuple2) {
      if (tuple1.Items.Length != tuple2.Items.Length) {
        return false;
      }
      for (int index = 0; index < tuple1.Items.Length; index++) {
        var comparison = Apply(SyntaxKind.EqualsExpression, tuple1.Items[index].Value, tuple2.Items[index].Value);
        if (!(comparison is bool result && result)) {
          return false;
        }
      }
      return true;
    }

    private static object ApplyBool(SyntaxKind op, object operand1, object operand2) {
      var value1 = (bool)operand1;
      var value2 = (bool)operand2;
      return op switch {
        SyntaxKind.BitwiseAndExpression => value1 & value2,
        SyntaxKind.BitwiseOrExpression => value1 | value2,
        SyntaxKind.ExclusiveOrExpression => value1 ^ value2,
        SyntaxKind.EqualsExpression => value1 == value2,
        SyntaxKind.NotEqualsExpression => value1 != value2,
        _ => throw new NotImplementedException(),
      };
    }

    private static bool IsNumeric(Type type) {
      return _numericTypeSet.Contains(type);
    }

    private static INumericType CommonNumericType(Type type1, Type type2) {
      // TODO: Precompute type order index
      var intLevel = _numericTypeLevel[typeof(int)]; // operator promotes at least to int
      var index1 = _numericTypeLevel[type1];
      var index2 = _numericTypeLevel[type2];
      var level = Math.Max(intLevel, Math.Max(index1, index2));
      return _numericTypes[level];
    }

    private static object ApplyNumeric(SyntaxKind op, object operand1, object operand2) {
      var type = CommonNumericType(operand1.GetType(), operand2.GetType());
      operand1 = Convert(operand1, type.Type);
      operand2 = Convert(operand2, type.Type);
      return op switch {
        SyntaxKind.AddExpression => type.Add(operand1, operand2),
        SyntaxKind.SubtractExpression => type.Sub(operand1, operand2),
        SyntaxKind.MultiplyExpression => type.Mul(operand1, operand2),
        SyntaxKind.DivideExpression => type.Div(operand1, operand2),
        SyntaxKind.ModuloExpression => type.Mod(operand1, operand2),
        SyntaxKind.LessThanExpression => type.Less(operand1, operand2),
        SyntaxKind.LessThanOrEqualExpression => (bool)type.Less(operand1, operand2) || operand1.Equals(operand2),
        SyntaxKind.GreaterThanExpression => type.Less(operand2, operand1),
        SyntaxKind.GreaterThanOrEqualExpression => (bool)type.Less(operand2, operand1) || operand1.Equals(operand2),
        SyntaxKind.EqualsExpression => operand1.Equals(operand2),
        SyntaxKind.NotEqualsExpression => !operand1.Equals(operand2),
        SyntaxKind.BitwiseAndExpression => type.BitAnd(operand1, operand2),
        SyntaxKind.BitwiseOrExpression => type.BitOr(operand1, operand2),
        SyntaxKind.ExclusiveOrExpression => type.Xor(operand1, operand2),
        SyntaxKind.LeftShiftExpression => type.LeftShift(operand1, operand2),
        SyntaxKind.RightShiftExpression => type.RightShift(operand1, operand2),
        _ => throw new NotImplementedException(),
      };
    }

    public static object Convert(this object value, Type target) {
      if (value == null) {
        return null;
      }
      var source = value.GetType();
      if (source == target) {
        return value;
      }
      if (_numericTypeDictionary.TryGetValue(source, out var sourceNumeric) && _numericTypeDictionary.TryGetValue(target, out var targetNumeric)) {
        return targetNumeric.Conversion2(sourceNumeric.Conversion1(value));
      }
      if (target.IsGenericType && target.GetGenericTypeDefinition() == typeof(Nullable<>)) {
        var typeArgument = target.GetGenericArguments()[0];
        return Convert(value, typeArgument);
      }
      throw new NotImplementedException();
    }
  }
}
