using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class EmptyBlock : StraightBlock {
    public EmptyBlock(Location location) : 
      base(location) {
    }
  }
}
