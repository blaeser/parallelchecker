using Microsoft.CodeAnalysis;
using ParallelChecker.Core.General;
using System;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class Delegate {
    public Object Instance { get; }
    public IMethodSymbol Method { get; }
    public Method Closure { get; }

    public Delegate(Object instance, IMethodSymbol method, Method closure) {
      Method = method ?? throw new ArgumentNullException(nameof(method));
      if (!method.IsStaticSymbol() && instance == null) {
        throw new ArgumentNullException(nameof(instance));
      }
      Instance = instance;
      Closure = closure;
    }

    public override bool Equals(object obj) {
      if (obj is not Delegate other) {
        return false;
      }
      return SymbolEqualityComparer.Default.Equals(Method, other.Method) &&
        Equals(Instance, other.Instance) &&
        Equals(Closure, other.Closure);
    }

    public override int GetHashCode() {
      return Method.GetHashCode();
    }
  }
}
