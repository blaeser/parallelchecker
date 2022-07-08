using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class TaskContinueRule : Rule<InvocationBlock> {
    public override int TimeCost => 10;

    public override bool Applicable(Program program, InvocationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Method)) {
        return false;
      }
      return block.Method.Is(Symbols.TaskContinueWith);
    }

    public override void Apply(Program program, InvocationBlock block) {
      var currentThread = program.ActiveThread;
      var method = program.ActiveMethod;
      var otherArguments = method.CollectArguments(block.Method.Parameters.Length - 1);
      var successor = method.EvaluationStack.Pop();
      var predecessor = method.EvaluationStack.Pop();
      var scheduler = otherArguments.FirstOfType<TaskScheduler>();
      if (predecessor == null || successor == null) {
        throw new Exception(program.ActiveLocation, new System.NullReferenceException());
      }
      if (predecessor is Thread previousThread && program.IsThreadStart(successor)) {
        var nextThread = program.CreateThread(successor);
        currentThread.AdvanceTime();
        nextThread.SynchronizeWith(currentThread);
        if (previousThread.ConfigureAwait) {
          nextThread.Dispatcher = scheduler?.Dispatcher;
        }
        previousThread.Continuations.Add(nextThread);
        if (previousThread.State == ThreadState.Terminated) {
          program.RunContinuation(previousThread, nextThread);
        }
        method.EvaluationStack.Push(nextThread);
      } else {
        method.EvaluationStack.Push(Unknown.Value);
      }
      program.GoToNextBlock();
    }
  }
}
