using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.ControlFlow.Routines {
  internal sealed class MethodRoutine : Routine {
    public SyntaxNode Method { get; }

    public MethodRoutine(SyntaxNode method) {
      if (method is not BaseMethodDeclarationSyntax && method is not LocalFunctionStatementSyntax) {
        throw new ArgumentException(nameof(method));
      }
      Method = method;
    }

    public override Location Location {
      get {
        return Method.GetLocation();
      }
    }

    public SyntaxTokenList Modifiers {
      get {
        if (Method is BaseMethodDeclarationSyntax method) {
          return method.Modifiers;
        } else {
          return ((LocalFunctionStatementSyntax)Method).Modifiers;
        }
      }
    }

    public BlockSyntax Body {
      get {
        if (Method is BaseMethodDeclarationSyntax method) {
          return method.Body;
        } else {
          return ((LocalFunctionStatementSyntax)Method).Body;
        }
      }
    }

    public ArrowExpressionClauseSyntax ExpressionBody {
      get {
        if (Method is BaseMethodDeclarationSyntax method) {
          return method.ExpressionBody;
        } else {
          return ((LocalFunctionStatementSyntax)Method).ExpressionBody;
        }
      }
    }

    public override IEnumerable<SyntaxNode> Nodes {
      get {
        yield return Method;
      }
    }

    public override bool Equals(object obj) {
      return obj is MethodRoutine routine && routine.Method.Equals(Method);
    }

    public override int GetHashCode() {
      return Method.GetHashCode();
    }
  }
}
