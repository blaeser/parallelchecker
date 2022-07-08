using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class Call {
    public ITypeSymbol Type { get; }
    public Object Instance { get; }

    public Call(ITypeSymbol type, Object instance) {
      Type = type ?? throw new ArgumentNullException(nameof(type));
      Instance = instance;
    }

    public override bool Equals(object obj) {
      return obj is Call other &&
        other.Type.Equals(Type, SymbolEqualityComparer.Default) &&
        Equals(other.Instance, Instance);
    }

    public override int GetHashCode() {
      return
        Type.GetHashCode() * 31 +
        (Instance == null ? 0 : Instance.GetHashCode());
    }
  }
}
