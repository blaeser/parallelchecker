using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class ObjectCreationBlock : StraightBlock {
    public ITypeSymbol Type { get; }
    public IMethodSymbol Constructor { get; }
    public int NofParameters { get; }

    public ObjectCreationBlock(Location location, ITypeSymbol type, IMethodSymbol constructor, int nofParameters) :
      base(location) {
      Type = type;
      Constructor = constructor;
      NofParameters = nofParameters;
    }
  }
}
