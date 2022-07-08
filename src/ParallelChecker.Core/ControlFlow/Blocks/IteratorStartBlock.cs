using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class IteratorStartBlock : StraightBlock {
    public IteratorStartBlock(Location location) : 
      base(location) {
    }
  }
}
