using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class ExitBlock : BasicBlock {
    public ExitBlock(Location location) :
      base(location) {
    }
  }
}
