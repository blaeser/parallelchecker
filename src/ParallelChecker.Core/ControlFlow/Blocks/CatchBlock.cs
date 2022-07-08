using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class CatchBlock : StraightBlock {
    public CatchBlock(Location location) : 
      base(location) {
    }
  }
}
