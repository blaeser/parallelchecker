using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class SwapBlock : StraightBlock {
    public SwapBlock(Location location) : 
      base(location) {
    }
  }
}
