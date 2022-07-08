using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class ReadBlock : StraightBlock {
    public ReadBlock(Location location) :
      base(location) {
    }
  }
}
