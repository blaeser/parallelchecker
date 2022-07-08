using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelChecker.Core.General;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.ControlFlow.Routines {
  internal sealed class GlobalRoutine : Routine {
    public GlobalStatementSyntax[] TopLevelStatements { get; }

    public GlobalRoutine(GlobalStatementSyntax[] topLevelStatements) {
      TopLevelStatements = topLevelStatements ?? throw new ArgumentNullException(nameof(topLevelStatements));
    }

    public override Location Location {
      get {
        if (TopLevelStatements.Length > 0) {
          return TopLevelStatements[0].GetLocation();
        }
        return Location.None;
      }
    }

    public override IEnumerable<SyntaxNode> Nodes => TopLevelStatements;

    public override bool Equals(object obj) {
      return obj is GlobalRoutine routine && routine.TopLevelStatements.ArrayEquals(TopLevelStatements);
    }

    public override int GetHashCode() {
      return TopLevelStatements.ArrayHashCode();
    }
  }
}
