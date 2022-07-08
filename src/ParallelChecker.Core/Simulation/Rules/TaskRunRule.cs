using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class TaskRunRule : Rule<InvocationBlock> {
    public override int TimeCost => 10;

    public override bool Applicable(Program program, InvocationBlock block) {
      var method = block.Method;
      if (program.CompilationModel.ContainsSyntaxNode(method)) {
        return false;
      }
      return method.IsAny(Symbols.TaskRun, Symbols.ThreadPoolQueue, Symbols.TaskFactoryStartNew);
    }

    public override void Apply(Program program, InvocationBlock block) {
      var method = program.ActiveMethod;
      var otherArguments = method.CollectArguments(block.Method.Parameters.Length - 1);
      var taskDelegate = method.EvaluationStack.Pop();
      if (!block.Method.IsStaticSymbol()) {
        method.EvaluationStack.Pop();
      }
      var scheduler = otherArguments.FirstOfType<TaskScheduler>();
      if (program.IsThreadStart(taskDelegate)) {
        var newThread = program.CreateThread(taskDelegate);
        program.StartThread(newThread);
        if (block.Method.Is(Symbols.ThreadPoolQueue) && otherArguments.Length > 0) {
          newThread.PassSingleParameter(0, otherArguments[0]);
        }
        scheduler?.Dispatcher?.Register(newThread);
        method.EvaluationStack.Push(newThread);
      } else if (!block.Method.ReturnsVoid) {
        method.EvaluationStack.Push(Unknown.Value);
      }
      program.GoToNextBlock();
    }
  }
}
