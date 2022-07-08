using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class PropertySetBlock : StraightBlock {
    public IPropertySymbol Property { get; }

    public PropertySetBlock(Location location, IPropertySymbol property) 
      : base(location) {
      Property = property ?? throw new ArgumentNullException(nameof(property));
    }
  }
}
