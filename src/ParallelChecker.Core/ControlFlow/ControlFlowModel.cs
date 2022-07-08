using ParallelChecker.Core.ControlFlow.Routines;
using ParallelChecker.Core.General;
using System.Collections.Generic;

namespace ParallelChecker.Core.ControlFlow {
  internal class ControlFlowModel {
    public CompilationModel CompilationModel { get; }
    private readonly Dictionary<Routine, ControlFlowGraph> _graphs = new();
    
    public ControlFlowModel(CompilationModel compilationModel) {
      CompilationModel = compilationModel;
    }

    public ControlFlowGraph GetGraph(Routine routine) {
      if (!_graphs.ContainsKey(routine)) {
        var visitor = new ControlFlowVisitor(CompilationModel);
        visitor.VisitRoutine(routine);
        _graphs.Add(routine, visitor.Graph);
      }
      return _graphs[routine];
    }
  }
}
