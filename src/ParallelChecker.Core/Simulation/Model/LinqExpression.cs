using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class LinqExpression {
    public ISymbol Parameter { get; }
    public ExpressionSyntax Expression { get; }
    public Method Closure { get; }

    public LinqExpression(ISymbol parameter, ExpressionSyntax expression, Method closure) {
      Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
      Closure = closure;
    }

    public override bool Equals(object obj) {
      if (obj is not LinqExpression other) {
        return false;
      }
      return SymbolEqualityComparer.Default.Equals(Parameter, other.Parameter) &&
        Equals(Expression, other.Expression) &&
        Equals(Closure, other.Closure);
    }

    public override int GetHashCode() {
      return Expression.GetHashCode();
    }
  }
}
