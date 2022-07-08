using System;

namespace ParallelChecker.Core.Simulation.Library {
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Enum)]
  internal sealed class TypeAttribute : Attribute {
    public string Namespace { get; }
    public string TypeName { get; }

    public TypeAttribute(string namespaceName, string typeName = null) {
      if (string.IsNullOrEmpty(namespaceName)) {
        throw new ArgumentException(nameof(namespaceName));
      }
      Namespace = namespaceName;
      TypeName = typeName;
    }
  }
}
