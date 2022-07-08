using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal class TupleCreationBlock : StraightBlock {
    public IFieldSymbol[] Fields { get; }

    public TupleCreationBlock(Location location, IFieldSymbol[] fields) : 
      base(location) {
      Fields = fields ?? throw new ArgumentNullException(nameof(fields));
    }
  }
}
