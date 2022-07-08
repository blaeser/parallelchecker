using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class IteratorEndBlock : StraightBlock {
    public IteratorEndBlock(Location location) : 
      base(location) {
    }
  }
}
