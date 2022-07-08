using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class VariableSet {
    private readonly Dictionary<ISymbol, Variable> _state = new(SymbolEqualityComparer.Default);

    public Variable this[ISymbol symbol] {
      get {
        if (!_state.ContainsKey(symbol)) {
          _state[symbol] = new ExplicitVariable(symbol);
        }
        return _state[symbol];
      }
    }

    public void SetAlias(ISymbol symbol, Variable existent) {
      _state.Add(symbol, existent);
    }

    public void Remove(ISymbol symbol) {
      _state.Remove(symbol);
    }

    public void SetExplicit(ISymbol symbol, Variable variable) {
      _state[symbol] = variable;
    }

    public IEnumerable<ISymbol> AllSymbols() {
      return _state.Keys;
    }

    public IEnumerable<Variable> AllVariables() {
      return _state.Values;
    }
  }
}
