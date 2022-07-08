using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal class DiscardBlock : StraightBlock {
    public DiscardBlock(Location location)
      : base(location) {
    }
  }
}
