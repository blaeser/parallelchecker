using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading")]
  internal sealed class Timer : Model.Object {
    // TODO: Support synchronization context

    public bool Active { get; set; }
    private bool Repetitive = false;
    private readonly object _callback;
    private readonly object _state;

    [Member]
    public Timer(Program _, object callback, object state) :
      base(typeof(System.Threading.Timer)) {
      _callback = callback;
      _state = state;
    }

    [Member]
    public Timer(Program program, object callback)
      : this(program, callback, null) {
    }

    [Member]
    public Timer(Program program, object callback, object state, object timeout, object period) :
      this(program, callback, state) {
      Activate(program, timeout, period);
    }

    [Member]
    public bool Change(Program program, object timeout, object period) {
      Activate(program, timeout, period);
      return true;
    }

    private void Activate(Program program, object timeout, object period) {
      if (Equals(timeout, -1)) {
        Active = false;
      } else if (!Active) {
        Active = true;
        // timer may run concurrently
        StartTimer(program);
      }
      if (Active && !Repetitive && !Equals(period, -1)) {
        Repetitive = true;
        StartTimer(program);
      }
    }

    public void StartTimer(Program program) {
      if (program.IsThreadStart(_callback)) {
        var timerThread = program.CreateThread(_callback);
        timerThread.Timer = this;
        program.StartThread(timerThread);
        timerThread.PassSingleParameter(0, _state);
      }
    }

    [Member]
    public void Dispose(Program _) {
      Active = false;
      Repetitive = false;
    }

    // TODO: Support wait handle argument
    [Member]
    public void Dispose(Program program, object _) {
      Dispose(program);
    }
  }
}
