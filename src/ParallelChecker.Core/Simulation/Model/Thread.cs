using ParallelChecker.Core.Simulation.Library;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class Thread : Object {
    private readonly Program _program;
    private ThreadState _state = ThreadState.Created;

    public Cause Cause { get; }
    public Stack<Method> CallStack { get; } = new();
    public VectorTime Time { get; } = new();
    public Queue<Thread> WaitersForJoin { get; } = new();
    public Object WaitingOn { get; set; }
    public WaitState WaitState { get; set; }
    public object Result { get; set; }
    public Dispatcher Dispatcher { get; set; }
    public List<Thread> Continuations { get; } = new();
    public Timer Timer { get; set; }
    public int NestedLocks { get; set; }
    public bool SyncWithStatic { get; set; }
    public bool ConfigureAwait { get; set; } = true;
    
    public Thread(Program program, Cause cause, Method initialMethod) : 
      this(program, cause) {
      CallStack.Push(initialMethod);
    }

    public Thread(Program program, Cause cause) {
      _program = program;
      Cause = cause ?? throw new ArgumentNullException(nameof(cause));
      Time.AdvanceTime(this);
      if (program.ActiveThread != null) {
        SyncWithStatic = program.ActiveThread.SyncWithStatic;
      }
    }

    public void SynchronizeWith(Thread thread) {
      if (Time.NeedsSynchronizeWith(thread)) {
        Time.SynchronizeWith(thread.Time);
      }
    }

    public void AdvanceTime() {
      Time.AdvanceTime(this);
    }

    public Method ActiveMethod {
      get { return CallStack.Peek(); }
    }

    public ThreadState State {
      get { return _state; }
      set {
        _state = value;
        if (_state == ThreadState.Runnable) {
          _program.RunnableThreads.Add(this);
        } else {
          _program.RunnableThreads.Remove(this);
        }
      }
    }
  }
}
