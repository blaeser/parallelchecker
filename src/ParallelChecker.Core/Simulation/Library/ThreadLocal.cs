using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading", "ThreadLocal`1")]
  internal sealed class ThreadLocal : Model.Object {
    // TODO: Support proper default value for generic type
    private static readonly ThreadLocalState _state = new(null, Unknown.Value);

    [Member]
    public ThreadLocal() {
    }

    [Member]
    public ThreadLocal(object _) {
      // TODO: Support track all values or valueFactory
    }

    [Member]
    public ThreadLocal(object _, object _2) {
      // TODO: Support track all values or valueFactory
    }

    [Member]
    public object GetValue(Program program) {
      return _state[program.ActiveThread].Value;
    }

    [Member]
    public void SetValue(Program program, object value) {
      _state[program.ActiveThread].Value = value;
    }
  }
}
