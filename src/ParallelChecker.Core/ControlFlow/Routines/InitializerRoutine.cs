using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.ControlFlow.Routines {
  internal sealed class InitializerRoutine : Routine {
    public TypeDeclarationSyntax Type { get; }
    public bool StructDefault { get; }

    public InitializerRoutine(TypeDeclarationSyntax type, bool structDefault) {
      Type = type ?? throw new ArgumentNullException(nameof(type));
      StructDefault = structDefault;
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
      return obj is InitializerRoutine initializer && 
        initializer.Type.Equals(Type) && initializer.StructDefault == StructDefault;
    }

    public override int GetHashCode() {
      return Type.GetHashCode() * 31 + StructDefault.GetHashCode();
    }
  }
}
