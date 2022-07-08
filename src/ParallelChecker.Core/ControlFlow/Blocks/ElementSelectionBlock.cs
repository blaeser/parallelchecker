using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class ElementSelectionBlock : StraightBlock {
    public int Dimensions { get; }

    public ElementSelectionBlock(Location location, int dimensions) :
      base(location) {
      Dimensions = dimensions;
    }
  }
}
