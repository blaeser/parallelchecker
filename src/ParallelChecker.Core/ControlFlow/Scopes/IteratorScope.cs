using ParallelChecker.Core.ControlFlow.Blocks;

namespace ParallelChecker.Core.ControlFlow.Scopes {
  internal sealed class IteratorScope : LoopScope {
    public IteratorScope(IteratorNextBlock entry, IteratorEndBlock exit) : 
      base(entry, exit) {
    }
  }
}
