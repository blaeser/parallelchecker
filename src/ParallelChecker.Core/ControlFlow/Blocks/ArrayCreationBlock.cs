using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal class ArrayCreationBlock : StraightBlock {
    public ITypeSymbol ElementType { get; }
    public int Ranks { get; }
    public int Lengths { get; }

    public ArrayCreationBlock(Location location, ITypeSymbol elementType, int ranks, int lengths) : 
      base(location) {
      ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
      if (ranks < 1) {
        throw new ArgumentException(nameof(ranks));
      }
      Ranks = ranks;
      if (lengths < 1) {
        throw new ArgumentException(nameof(lengths));
      }
      Lengths = lengths;
    }
  }
}
