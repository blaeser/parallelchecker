using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.Simulation.Model {
  internal class ImplicitVariable : Variable {
    public string Name { get; }

    private readonly ITypeSymbol _type;
    
    public ImplicitVariable(string name, ITypeSymbol type, object initial) {
      Name = name ?? throw new ArgumentNullException(nameof(name));
      _type = type;
      Value = initial;
    }

    public override ITypeSymbol Type {
      get { return _type; }
    }

    public override bool IsVolatile {
      get { return false; }
    }

    public override Variable Clone() {
      return new ImplicitVariable(Name, Type, Value);
    }

    public override string ToString() {
      return Name;
    }
  }
}
