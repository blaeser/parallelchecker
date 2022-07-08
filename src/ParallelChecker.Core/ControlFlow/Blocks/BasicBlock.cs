using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal abstract class BasicBlock {
    public Location Location { get; }

    public BasicBlock(Location location) {
      Location = location ?? throw new ArgumentNullException(nameof(location));
    }
  }
}
