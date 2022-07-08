using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class Lambda {
    public AnonymousFunctionExpressionSyntax Expression { get; }
    public IMethodSymbol Symbol { get; }
    public Method Closure { get; }

    public Lambda(AnonymousFunctionExpressionSyntax expression, IMethodSymbol symbol, Method closure) {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
      Symbol = symbol;
      Closure = closure;
    }

    public override bool Equals(object obj) {
      if (obj is not Lambda other) {
        return false;
      }
      return Equals(Expression, other.Expression) &&
        SymbolEqualityComparer.Default.Equals(Symbol, other.Symbol) &&
        Equals(Closure, other.Closure);
    }

    public override int GetHashCode() {
      return Expression.GetHashCode();
    }
  }
}
