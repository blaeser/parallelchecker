using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ParallelInvokeRule : Rule<InvocationBlock> {
    public override int TimeCost => 10;

    public override bool Applicable(Program program, InvocationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Method)) {
        return false;
      }
      return block.Method.Is(Symbols.ParallelInvoke);
    }

    public override void Apply(Program program, InvocationBlock block) {
      var thread = program.ActiveThread;
      if (thread.WaitState == null) {
        StartTasks(program, block);
      } else {
        JoinTasks(program);
      }
    }

    private void JoinTasks(Program program) {
      var parentThread = program.ActiveThread;
      var state = (JoinWaitState)parentThread.WaitState;
      foreach (var childThread in state.AwaitedThreads) {
        if (!program.JoinThread(childThread, true)) {
          return;
        }
      }
      parentThread.WaitState = null;
      program.GoToNextBlock();
    }

    private static void StartTasks(Program program, InvocationBlock block) {
      // TODO: Support method overloads
      var method = program.ActiveMethod;
      var argument = method.EvaluationStack.Pop();
      method.IgnoreArguments(block.Method.Parameters.Length - 1);
      if (argument == Unknown.Value) {
        program.GoToNextBlock();
      } else {
        var state = new JoinWaitState();
        var array = (Array)argument;
        foreach (var threadStart in array.AllValues()) {
          if (program.IsThreadStart(threadStart)) {
            var childThread = program.CreateThread(threadStart);
            program.StartThread(childThread);
            state.AwaitedThreads.Add(childThread);
          }
        }
        program.ActiveThread.WaitState = state;
      }
    }
  }
}
