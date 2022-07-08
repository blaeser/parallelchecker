using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class UndeclareBlock : StraightBlock {
    public ILocalSymbol Variable { get; }

    public UndeclareBlock(Location location, ILocalSymbol variable) : 
      base(location) {
      Variable = variable ?? throw new ArgumentNullException(nameof(variable));
    }
  }
}
