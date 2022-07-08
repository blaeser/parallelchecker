using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Model {
  internal class Object {
    public ITypeSymbol Type { get; }
    public Type NativeType { get; }
    public int LockCounter { get; set; }
    public HashSet<Thread> LockHolders { get; } = new();
    public VectorTime UnlockTime { get; } = new();
    public Queue<Thread> LockWaiters { get; } = new();
    public Queue<Thread> PulseWaiters { get; } = new();
    public VariableSet InstanceFields { get; } = new();
    public HashSet<Epoch> UnsafeCalls { get; } = new();

    public Object(ITypeSymbol type) {
      Type = type;
    }

    public Object(Type nativeType) {
      NativeType = nativeType;
    }

    public Object() {
    }
  }
}
