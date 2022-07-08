using Microsoft.CodeAnalysis;
using ParallelChecker.Core.General;
using System;

namespace ParallelChecker.Core.Simulation.Model {
  internal class ExplicitVariable : Variable {
    public ISymbol Symbol { get; }

    public ExplicitVariable(ISymbol symbol) {
      Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
      var type = symbol.GetVariableType();
      if (type == null || type.IsTypeParameter()) {
        Value = Unknown.Value;
      } else if (type.IsStruct()) {
        Value = new Object(type);
      } else {
        Value = type.GetDefaultValue();
      }
      if (symbol.IsThreadStaticVariable()) {
        Value = new ThreadLocalState(symbol, Value);
      }
    }

    public override ITypeSymbol Type {
      get { return Symbol.GetVariableType(); }
    }

    public override bool IsVolatile {
      get { return Symbol.IsVolatile(); }
    }

    public override Variable Clone() {
      return new ExplicitVariable(Symbol) {
        Value = Value
      };
    }

  public override string ToString() {
    return Symbol.ToString();
  }
}
}
