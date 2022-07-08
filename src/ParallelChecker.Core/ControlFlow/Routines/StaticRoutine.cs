using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.ControlFlow.Routines {
  internal sealed class StaticRoutine : Routine {
    public TypeDeclarationSyntax Type { get; }

    public StaticRoutine(TypeDeclarationSyntax type) {
      Type = type ?? throw new ArgumentNullException(nameof(type));
    }

    public override Location Location {
      get {
        return Type.GetLocation();
      }
    }

    public override IEnumerable<SyntaxNode> Nodes {
      get {
        yield return Type;
      }
    }

    public override bool Equals(object obj) {
      return obj is StaticRoutine routine && routine.Type.Equals(Type);
    }

    public override int GetHashCode() {
      return Type.GetHashCode();
    }
  }
}
