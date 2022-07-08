using Microsoft.CodeAnalysis;
using ParallelChecker.Core.General;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class ThreadLocalState {
    public ISymbol OriginalVariable { get; }
    public object DefaultValue { get; }
    private readonly Dictionary<Thread, Variable> _values = new();

    public ThreadLocalState(ISymbol originalVariable, object defaultValue) {
      OriginalVariable = originalVariable;
      DefaultValue = defaultValue;
    }

    public Variable this[Thread thread] {
      get {
        if (!_values.ContainsKey(thread)) {
          _values[thread] = new ImplicitVariable($"thread local {OriginalVariable?.Name}", OriginalVariable?.GetVariableType(), DefaultValue);
        }
        return _values[thread];
      }
    }
  }
}
