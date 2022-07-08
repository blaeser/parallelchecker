using ParallelChecker.Core.ControlFlow.Blocks;
using System.Collections.Generic;

namespace ParallelChecker.Core.ControlFlow.Scopes {
  internal sealed class SwitchScope : BreakScope {
    public IDictionary<object, StraightBlock> CaseLabels { get; } = new Dictionary<object, StraightBlock>();
    public StraightBlock DefaultLabel { get; set; }

    public SwitchScope(BasicBlock exit) 
      : base(exit) {
    }
  }
}
