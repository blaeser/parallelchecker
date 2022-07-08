using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ThreadStartRule : Rule<InvocationBlock> {
    public override int TimeCost => 10;

    public override bool Applicable(Program program, InvocationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Method)) {
        return false;
      }
      return block.Method.IsAny(Symbols.ThreadStart, Symbols.TaskStart);
    }

    public override void Apply(Program program, InvocationBlock block) {
      var method = program.ActiveMethod;
      var arguments = method.CollectArguments(block.Method.Parameters.Length);
      var value = method.EvaluationStack.Pop();
      var scheduler = arguments.FirstOfType<TaskScheduler>();
      if (value != Unknown.Value) {
        var otherThread = (Thread)value;
        if (block.Method.Is(Symbols.ThreadStart) && arguments.Length > 0) {
          otherThread.PassSingleParameter(0, arguments[0]);
        }
        program.StartThread(otherThread);
        scheduler?.Dispatcher?.Register(otherThread);
      }
      program.GoToNextBlock();
    }
  }
}
