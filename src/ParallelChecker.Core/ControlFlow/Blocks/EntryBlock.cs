using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class EntryBlock : StraightBlock {
    public EntryBlock(Location location) :
      base(location) {
    }
  }
}
