using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class PropertyGetBlock : StraightBlock {
    public IPropertySymbol Property { get; }

    public PropertyGetBlock(Location location, IPropertySymbol property) 
      : base(location) {
      Property = property ?? throw new ArgumentNullException(nameof(property));
    }
  }
}
