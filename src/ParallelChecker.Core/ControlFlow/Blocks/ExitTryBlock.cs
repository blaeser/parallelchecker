using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class ExitTryBlock : StraightBlock {
    public ExitTryBlock(Location location) : 
      base(location) {
    }
  }
}
