using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class CastBlock : StraightBlock {
    public ITypeSymbol Type { get; }

    public CastBlock(Location location, ITypeSymbol type) : 
      base(location) {
      Type = type;
    }
  }
}
