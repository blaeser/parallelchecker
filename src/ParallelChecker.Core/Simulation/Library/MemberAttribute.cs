using System;

namespace ParallelChecker.Core.Simulation.Library {
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | 
   AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Event)]
  internal sealed class MemberAttribute : Attribute {
    public string Name { get; }

    public MemberAttribute(string name = null) {
      Name = name;
    }
  }
}
