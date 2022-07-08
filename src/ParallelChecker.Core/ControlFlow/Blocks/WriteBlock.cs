using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class WriteBlock : StraightBlock {
    public WriteBlock(Location location) :
      base(location) {
    }
  }
}
