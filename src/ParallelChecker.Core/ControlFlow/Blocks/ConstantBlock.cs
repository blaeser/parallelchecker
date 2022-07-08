using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class ConstantBlock : StraightBlock {
    public object Value { get; }

    public ConstantBlock(Location location, object value) :
      base(location) {
      Value = value;
    }
  }
}
