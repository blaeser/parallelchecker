using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  public enum LinqKind {
    Unknown,
    From,
    Where,
    Select
  }

  internal sealed class LinqBlock : StraightBlock {
    public LinqKind Kind { get; }
    public ISymbol Parameter { get; } 
    public ExpressionSyntax Expression { get; }

    public LinqBlock(Location location, LinqKind kind, ISymbol parameter, ExpressionSyntax expression) : 
      base(location) {
      Kind = kind;
      Parameter = parameter;
      Expression = expression;
    }
  }
}
