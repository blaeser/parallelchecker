using ParallelChecker.Core.ControlFlow.Blocks;
using System;

namespace ParallelChecker.Core.ControlFlow.Scopes {
  internal class BreakScope : Scope {
    public BasicBlock Exit { get; }

    public BreakScope(BasicBlock exit) {
      Exit = exit ?? throw new ArgumentNullException(nameof(exit));
    }
  }
}
