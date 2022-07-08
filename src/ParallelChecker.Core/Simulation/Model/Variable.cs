using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Model {
  internal abstract class Variable {
    public object Value { get; set; }
    public VectorTime VolatileAccess { get; } = new();
    public HashSet<Epoch> Writes { get; } = new();
    public HashSet<Epoch> Reads { get; } = new();

    public abstract ITypeSymbol Type { get; }

    public abstract bool IsVolatile { get; }

    public abstract Variable Clone();

    public abstract override string ToString();
  }
}
