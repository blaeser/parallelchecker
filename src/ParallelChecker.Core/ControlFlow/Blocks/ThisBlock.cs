using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class ThisBlock : StraightBlock {
    public ThisBlock(Location location) :
      base(location) {
    }
  }
}
