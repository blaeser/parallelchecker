using Microsoft.CodeAnalysis;

namespace ParallelChecker.Core.ControlFlow.Blocks {
  internal sealed class BranchBlock : BasicBlock {
    public BasicBlock SuccessorOnTrue { get; set; }
    public BasicBlock SuccessorOnFalse { get; set; }

    public BranchBlock(Location location) : 
      base(location) {
    }
  }
}
