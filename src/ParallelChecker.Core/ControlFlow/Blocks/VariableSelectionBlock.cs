using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class VariableSelectionBlock : StraightBlock {
    public ISymbol Variable { get; }

    public VariableSelectionBlock(Location location, ISymbol variable) :
      base(location) {
      Variable = variable ?? throw new ArgumentNullException(nameof(variable));
    }
  }
}
