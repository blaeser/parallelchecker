using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal class OperatorBlock : StraightBlock {
    public SyntaxKind Kind { get; }
    public IMethodSymbol Method { get; }

    public OperatorBlock(Location location, SyntaxKind kind, IMethodSymbol method)
      : base(location) {
      Kind = kind;
      Method = method;
    }
  }
}
