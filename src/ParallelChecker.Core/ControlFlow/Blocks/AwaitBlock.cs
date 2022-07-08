using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class AwaitBlock : StraightBlock {
    public AwaitBlock(Location location) : 
      base(location) {
    }
  }
}
