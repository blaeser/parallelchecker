using ParallelChecker.Core.ControlFlow.Blocks;
using System;

namespace ParallelChecker.Core.ControlFlow.Scopes {
  internal class LoopScope : BreakScope {
    public BasicBlock Entry { get; }
    
    public LoopScope(BasicBlock entry, BasicBlock exit) 
      : base(exit) {
      Entry = entry ?? throw new ArgumentNullException(nameof(entry));
    }
  }
}
