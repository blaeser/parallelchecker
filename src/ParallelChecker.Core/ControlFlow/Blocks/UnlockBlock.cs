using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class UnlockBlock : StraightBlock {
    public UnlockBlock(Location location) :
      base(location) {
    }
  }
}
