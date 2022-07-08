using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core.Simulation.Model {
  [Serializable]
  internal sealed class Exception : System.Exception {
    public Object Instance { get; }
    public Location Location { get;  }

    public Exception(Location location, System.Exception innerException) : 
      base(string.Format($"{innerException.GetType().Name}: {innerException.Message}"), innerException) {
      Location = location;
      Instance = new Object(innerException.GetType());
    }

    public Exception(Location location, string message) : 
      base(string.Format($"exception: {message}")) {
      Location = location;
      Instance = new Object(typeof(System.Exception));
    }

    public Exception(Location location, Object instance) :
      base(string.Format($"exception: {instance.Type}")) {
      Location = location;
      Instance = instance;
    }
  }
}
