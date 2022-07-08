using Microsoft.CodeAnalysis;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class TaskConfigureAwaitRule : Rule<InvocationBlock> {
    public override int TimeCost => 5;

    public override bool Applicable(Program program, InvocationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Method)) {
        return false;
      }
      return block.Method.IsAny(Symbols.TaskConfigureAwait, Symbols.ValueTaskConfigureAwait);
    }

    public override void Apply(Program program, InvocationBlock block) {
      var thread = program.ActiveThread;
      var method = thread.ActiveMethod;
      var value = method.EvaluationStack.Pop();
      if (value != Unknown.Value && !(bool)value) {
        var instance = method.EvaluationStack.Pop();
        if (instance is Thread previousTask) {
          var empty = new Method(null, new ExitBlock(Location.None), null, program.ActiveCause);
          var nextTask = new Thread(program, previousTask.Cause, empty) {
            ConfigureAwait = false
          };
          previousTask.Continuations.Add(nextTask);
          if (previousTask.State == ThreadState.Terminated) {
            program.RunContinuation(previousTask, nextTask);
          }
          instance = nextTask;
        }
        method.EvaluationStack.Push(instance);
      }
      program.GoToNextBlock();
    }
  }
}
