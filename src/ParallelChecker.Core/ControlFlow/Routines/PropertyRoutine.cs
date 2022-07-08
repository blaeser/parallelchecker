using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.ControlFlow.Routines {
  internal sealed class PropertyRoutine : Routine {
    public AccessorDeclarationSyntax Accessor { get; }

    public PropertyRoutine(AccessorDeclarationSyntax accessor) {
      Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
      if (accessor.Body == null) {
        throw new ArgumentException(nameof(accessor));
      }
    }

    public override Location Location {
      get {
        return Accessor.GetLocation();
      }
    }

    public override IEnumerable<SyntaxNode> Nodes {
      get {
        yield return Accessor;
      }
    }

    public override bool Equals(object obj) {
      return obj is PropertyRoutine routine && routine.Accessor.Equals(Accessor);
    }

    public override int GetHashCode() {
      return Accessor.GetHashCode();
    }
  }
}
