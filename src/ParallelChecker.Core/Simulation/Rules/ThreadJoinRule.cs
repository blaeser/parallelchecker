using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ThreadJoinRule : Rule<InvocationBlock> {
    public override int TimeCost => 6;

    public override bool Applicable(Program program, InvocationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Method)) {
        return false;
      }
      return block.Method.IsAny(Symbols.ThreadJoin, Symbols.TaskWait);
    }

    public override void Apply(Program program, InvocationBlock block) {
      var currentThread = program.ActiveThread;
      var method = currentThread.ActiveMethod;
      var arguments = method.CollectArguments(block.Method);
      var value = method.EvaluationStack.Pop();
      if (value == Unknown.Value) {
        currentThread.State = ThreadState.Waiting;
      } else if (value == null) {
        throw new Exception(program.ActiveLocation, new System.NullReferenceException());
      } else if (value is Thread otherThread) { 
        if (program.JoinThread(otherThread, block.Method.Is(Symbols.TaskWait))) {
          NonBlockingJoin(program, block);
        } else {
          method.EvaluationStack.Push(value);
          method.PutBackArguments(arguments);
        }
      } else {
        NonBlockingJoin(program, block);
      }
    }

    private static void NonBlockingJoin(Program program, InvocationBlock block) {
      var method = program.ActiveMethod;
      if (!block.Method.ReturnsVoid) {
        method.EvaluationStack.Push(true);
      }
      program.GoToNextBlock();
    }
  }
}
