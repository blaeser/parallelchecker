using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Immutable;
using ParallelChecker.Core.General;

namespace ParallelChecker.Core.Simulation.Base {
  internal static class ReflectionLogic {
    private const string _ConstructorIdentifier = ".ctor";
    private const string _GetterPrefix = "get_";
    private const string _SetterPrefix = "set_";
    private const string _IndexerSuffix = "Item";

    public static bool IsConstructor(this IMethodSymbol method) {
      return method.Name == _ConstructorIdentifier;
    }

    public static bool IsConstructor(this MethodBase method) {
      return method.Name == _ConstructorIdentifier;
    }

    public static ConstructorInfo FindDefaultConstructor(this Type type) {
      var candidates =
        from constructor in type.GetConstructors()
        where constructor.GetParameters().Length == 0
        select constructor;
      return candidates.Single();
    }

    public static MethodInfo FindMethod(this Type type, IMethodSymbol callee) {
      var candidates =
        from method in type.GetMethods()
        where method.Name == callee.Name && Matches(callee.Parameters, method.GetParameters())
        select method;
      if (candidates.Count() != 1) {
        throw new AmbiguousMatchException();
      }
      return candidates.Single();
    }

    public static MethodInfo FindAccessor(this Type type, IPropertySymbol property, bool isGetter) {
      var name = isGetter ? _GetterPrefix : _SetterPrefix;
      name += property.IsIndexer ? _IndexerSuffix : property.Name;
      var candidates =
        from method in type.GetMethods()
        where method.Name == name
        select method;
      return candidates.Single();
    }

    private static bool Matches(ImmutableArray<IParameterSymbol> source, ParameterInfo[] target) {
      if (source.Length != target.Length) {
        return false;
      }
      for (int index = 0; index < source.Length; index++) {
        if (!Matches(source[index].Type, target[index].ParameterType)) {
          return false;
        }
      }
      return true;
    }

    private static bool Matches(ITypeSymbol sourceType, Type targetType) {
      // TODO: Support generic type resolution
      if (sourceType.TypeKind == TypeKind.TypeParameter) {
        return true;
      }
      // TODO: Improve array type compatibility (array covariance)
      if (sourceType is IArrayTypeSymbol && targetType == typeof(object[])) {
        return true;
      }
      return sourceType.IsCompatibleTo(targetType);
    }
  }
}
