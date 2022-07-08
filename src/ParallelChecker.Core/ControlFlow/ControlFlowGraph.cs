using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.ControlFlow.Routines;
using System;

namespace ParallelChecker.Core.ControlFlow {
  internal class ControlFlowGraph {
    public Routine Routine { get; }
    public EntryBlock Entry { get; }
    public ExitBlock Exit { get; }

    public ControlFlowGraph(Routine routine) {
      Routine = routine ?? throw new ArgumentNullException(nameof(routine));
      Entry = new EntryBlock(routine.Location);
      Exit = new ExitBlock(routine.Location);
    }
  }
}
