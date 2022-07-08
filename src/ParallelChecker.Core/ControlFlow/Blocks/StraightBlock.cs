using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal abstract class StraightBlock : BasicBlock {
    public BasicBlock Successor { get; set; }

    public StraightBlock(Location location) : 
      base(location) {
    }
  }
}
