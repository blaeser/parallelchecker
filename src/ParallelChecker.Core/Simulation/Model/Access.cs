using System;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class Access {
    public Variable Variable { get; }
    public bool IsVolatile { get; }

    public Access(Variable variable, bool isVolatile) {
      Variable = variable ?? throw new ArgumentNullException(nameof(variable));
      IsVolatile = isVolatile;
    }

    public override bool Equals(object obj) {
      return obj is Access other &&
        other.Variable.Equals(Variable) &&
        other.IsVolatile == IsVolatile;
    }

    public override int GetHashCode() {
      return Variable.GetHashCode();
    }
  }
}
