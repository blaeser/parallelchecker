using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading", "AsyncLocal`1")]
  internal sealed class AsyncLocal : Model.Object {
    // TODO: Support proper default value for generic type
    private static readonly ThreadLocalState _state = new(null, Unknown.Value);

    [Member]
    public AsyncLocal() {
    }

    [Member]
    public AsyncLocal(object _) {
      // TODO: Support value changed handler
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
