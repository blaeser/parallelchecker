using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class CollectionInitializerBlock : StraightBlock {
    public int[] Sizes { get; }
    public bool AddAll { get; }
    public ITypeSymbol Type { get; }

    public CollectionInitializerBlock(Location location, int[] sizes, bool addAll, ITypeSymbol type) : 
      base(location) {
      Sizes = sizes ?? throw new ArgumentNullException(nameof(sizes));
      AddAll = addAll;
      Type = type ?? throw new ArgumentNullException(nameof(type));
    }
  }
}
