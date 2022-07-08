using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class EnterTryBlock : StraightBlock {
    public EmptyBlock Catches { get; set; }
    public EmptyBlock Finally { get; set;  }

    public EnterTryBlock(Location location) : 
      base(location) {
    }
  }
}
