using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class LockBlock : StraightBlock {
    public LockBlock(Location location) :
      base(location) {
    }
  }
}
