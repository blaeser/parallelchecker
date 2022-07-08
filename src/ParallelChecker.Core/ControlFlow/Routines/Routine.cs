using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace ParallelChecker.Core.ControlFlow.Routines {
  internal abstract class Routine {
    public abstract Location Location { get; }
    public abstract IEnumerable<SyntaxNode> Nodes { get; }
    public abstract override bool Equals(object obj);
    public abstract override int GetHashCode();
  }
}
