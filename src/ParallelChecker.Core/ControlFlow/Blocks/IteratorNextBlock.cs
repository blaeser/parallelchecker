using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class IteratorNextBlock : BasicBlock {
    public ILocalSymbol Variable { get; }
    public BasicBlock SuccessorOnContinue { get; set; }
    public BasicBlock SuccessorOnFinished { get; set; }

    public IteratorNextBlock(Location location, ILocalSymbol variable) : 
      base(location) {
      Variable = variable ?? throw new ArgumentNullException(nameof(variable));
    }
  }
}
