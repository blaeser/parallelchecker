using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class AliasBlock : StraightBlock {
    public ISymbol Variable { get; }

    public AliasBlock(Location location, ISymbol variable) : 
      base(location) {
      Variable = variable ?? throw new ArgumentNullException(nameof(variable));
    }
  }
}
