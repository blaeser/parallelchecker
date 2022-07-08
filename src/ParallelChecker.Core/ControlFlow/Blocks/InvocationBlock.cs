using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class InvocationBlock : StraightBlock {
    public IMethodSymbol Method { get; }
    public bool IsVirtual { get; }

    public InvocationBlock(Location location, IMethodSymbol method, bool isVirtual) :
      base(location) {
      Method = method ?? throw new ArgumentNullException(nameof(method));
      IsVirtual = isVirtual;
    }
  }
}
