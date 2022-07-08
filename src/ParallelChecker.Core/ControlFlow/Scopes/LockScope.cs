using ParallelChecker.Core.ControlFlow.Blocks;
using System;

namespace ParallelChecker.Core.ControlFlow.Scopes {
  internal sealed class LockScope : Scope {
    public LockBlock Block { get; }

    public LockScope(LockBlock block) {
      Block = block ?? throw new ArgumentNullException(nameof(block));
    }
  }
}
