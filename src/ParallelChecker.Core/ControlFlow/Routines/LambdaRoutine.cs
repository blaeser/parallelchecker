using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.ControlFlow.Routines {
  internal sealed class LambdaRoutine : Routine {
    public AnonymousFunctionExpressionSyntax Lambda { get; }

    public LambdaRoutine(AnonymousFunctionExpressionSyntax lambda) {
      Lambda = lambda ?? throw new ArgumentNullException(nameof(lambda));
    }

    public override Location Location {
      get {
        return Lambda.GetLocation();
      }
    }

    public override IEnumerable<SyntaxNode> Nodes {
      get { 
        yield return Lambda; 
      }
    }

    public override bool Equals(object obj) {
      return obj is LambdaRoutine routine && routine.Lambda.Equals(Lambda);
    }

    public override int GetHashCode() {
      return Lambda.GetHashCode();
    }
  }
}
