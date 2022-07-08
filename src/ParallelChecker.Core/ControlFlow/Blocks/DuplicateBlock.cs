using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal class DuplicateBlock : StraightBlock {
    public DuplicateBlock(Location location)
      : base(location) {
    }
  }
}
