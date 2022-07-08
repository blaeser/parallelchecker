using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal class ArrayInitializerBlock : StraightBlock {
    public ITypeSymbol ElementType { get; }
    public int Rank { get; }
    public int Length { get; }

    public ArrayInitializerBlock(Location location, ITypeSymbol elementType, int rank, int length) 
      : base(location) {
      if (length < 0) {
        throw new ArgumentException(nameof(length));
      }
      if (rank < 1) {
        throw new ArgumentException(nameof(rank));
      }
      ElementType = elementType;
      Rank = rank;
      Length = length;
    }
  }
}
