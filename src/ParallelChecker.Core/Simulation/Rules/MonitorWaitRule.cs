using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class MonitorWaitRule : Rule<InvocationBlock> {
    public override int TimeCost => 5;

    public override bool Applicable(Program program, InvocationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Method)) {
        return false;
      }
      return block.Method.Is(Symbols.MonitorWait);
    }

    public override void Apply(Program program, InvocationBlock block) {
      var thread = program.ActiveThread;
      if (thread.WaitState == null) {
        MonitorReleaseAndWait(program, block);
      } else {
        MonitorReacquire(program);
      }
    }

    private void MonitorReleaseAndWait(Program program, InvocationBlock block) {
      var method = program.ActiveMethod;
      var arguments = method.CollectArguments(block.Method.Parameters.Length);
      var instance = arguments[0];
      if (instance == null) {
        throw new Exception(program.ActiveLocation, "Monitor wait on null");
      }
      var monitor = instance as Object ?? Unknown.Value;
      if (monitor != Unknown.Value) {
        var thread = program.ActiveThread;
        MonitorRelease(thread, monitor);
        thread.State = ThreadState.Waiting;
        monitor.PulseWaiters.Enqueue(thread);
      } else {
        method.EvaluationStack.Push(true);
        program.GoToNextBlock();
      }
    }

    private static void MonitorRelease(Thread thread, Object monitor) {
      if (!monitor.LockHolders.Contains(thread)) {
        var block = thread.ActiveMethod.ActiveBlock;
        throw new Exception(block.Location, "Thread does not hold monitor lock");
      }
      thread.WaitState = new MonitorWaitState(monitor, monitor.LockCounter);
      monitor.LockHolders.Remove(thread);
      monitor.LockCounter = 0;
      thread.AdvanceTime();
      monitor.UnlockTime.SynchronizeWith(thread.Time);
      monitor.LockWaiters.NotifySingle();
    }

    private static void MonitorReacquire(Program program) {
      var thread = program.ActiveThread;
      var state = (MonitorWaitState)thread.WaitState;
      var monitor = state.LockTarget;
      if (monitor.LockHolders.Count > 0) {
        thread.State = ThreadState.Waiting;
        monitor.LockWaiters.Enqueue(thread);
        thread.WaitingOn = monitor;
        program.CheckForDeadlock();
      } else {
        monitor.LockHolders.Add(thread);
        monitor.LockCounter = state.LockCount;
        thread.WaitState = null;
        thread.Time.SynchronizeWith(monitor.UnlockTime);
        thread.AdvanceTime();
        program.ActiveMethod.EvaluationStack.Push(true);
        program.GoToNextBlock();
      }
    }
  }
}
