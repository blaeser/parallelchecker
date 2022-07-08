using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class ThrowBlock : BasicBlock {
    public bool IsRethrow { get; }

    public ThrowBlock(Location location, bool isRethrow) : 
      base(location) {
      IsRethrow = isRethrow;
    }
  }
}
