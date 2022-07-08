using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class UnknownBlock : StraightBlock {
    public UnknownBlock(Location location) : 
      base(location) {
    }
  }
}
