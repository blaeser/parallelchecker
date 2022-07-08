using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.ControlFlow.Routines {
  internal sealed class ExpressionRoutine : Routine {
    public ExpressionSyntax Expression { get; }
    public bool ReturnsVoid { get; }

    public ExpressionRoutine(ExpressionSyntax expression, bool returnsVoid) {
      Expression = expression ?? throw new ArgumentNullException(nameof(expression));
      ReturnsVoid = returnsVoid;
    }

    public override Location Location {
      get {
        return Expression.GetLocation();
      }
    }

    public override IEnumerable<SyntaxNode> Nodes {
      get {
        yield return Expression;
      }
    }

    public override bool Equals(object obj) {
      return obj is ExpressionRoutine routine && routine.Expression.Equals(Expression);
    }

    public override int GetHashCode() {
      return Expression.GetHashCode();
    }
  }
}
