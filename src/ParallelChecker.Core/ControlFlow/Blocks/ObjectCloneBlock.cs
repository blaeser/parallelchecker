using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class ObjectCloneBlock : StraightBlock {
    public bool CloneRecord { get; }

    public ObjectCloneBlock(Location location, bool cloneRecord) : 
      base(location) {
      CloneRecord = cloneRecord;
    }
  }
}
