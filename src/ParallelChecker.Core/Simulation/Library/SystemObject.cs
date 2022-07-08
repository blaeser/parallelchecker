using Microsoft.CodeAnalysis;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ParallelChecker.Core.Simulation.Library {
  internal class SystemObject : Model.Object {
    public object NativeInstance { get; }

    private SystemObject(ITypeSymbol typeSymbol, object nativeInstance)
      : base(typeSymbol) {
      NativeInstance = nativeInstance ?? throw new ArgumentNullException(nameof(nativeInstance));
    }

    // TODO: Better implement own collection logic, makes it safer against capacity excess and other issues
    // TODO: Instead of concurrent collections, use internally conventional collections

    private static readonly Type[] _types = {
      typeof(ArrayList),
      typeof(TimeSpan),
      typeof(Dictionary<,>),
      typeof(HashSet<>),
      typeof(Hashtable),
      typeof(List<>),
      typeof(LinkedList<>),
      typeof(Queue<>),
      typeof(Queue),
      typeof(SortedDictionary<,>),
      typeof(SortedList<,>),
      typeof(SortedList),
      typeof(SortedSet<>),
      typeof(Stack<>),
      typeof(Stack),
      typeof(ICollection<>),
      typeof(IList<>),
      typeof(ISet<>),
      typeof(IDictionary<,>),
      // concurrent collections must be non-blocking here
      typeof(ConcurrentBag<>),
      typeof(ConcurrentQueue<>),
      typeof(ConcurrentStack<>)
    };

    private static readonly Dictionary<Type, Type> _defaultImplementations = new() {
      { typeof(ICollection<>), typeof(List<>) },
      { typeof(IList<>), typeof(List<>) },
      { typeof(ISet<>), typeof(HashSet<>) },
      { typeof(IDictionary<,>), typeof(Dictionary<,>) },
    };

    private static readonly Dictionary<string, Type> _registeredTypes = new();

    static SystemObject() {
      RegisterTypes();
    }

    public static bool IsSystemDefined(ITypeSymbol type) {
      return _registeredTypes.ContainsKey(type.GetFullGenericName());
    }

    public static bool IsSystemDefined(IMethodSymbol method) {
      return IsSystemDefined(method.ContainingType) && !HasRefOrOutParameters(method);
    }

    // TODO: support ref and out parameters for API calls
    private static bool HasRefOrOutParameters(IMethodSymbol method) {
      return
        (from parameter in method.Parameters
         where parameter.RefKind != RefKind.None
         select parameter).Any();
    }

    public static bool IsSystemDefined(IPropertySymbol property) {
      return IsSystemDefined(property.ContainingType);
    }

    public static SystemObject Create(ITypeSymbol symbol) {
      var type = _registeredTypes[symbol.GetFullGenericName()];
      if (type.IsInterface) {
        type = _defaultImplementations[type];
      }
      type = MakeGenericType(type);
      return new SystemObject(symbol, Activator.CreateInstance(type, new object[0]));
    }

    private static void RegisterTypes() {
      foreach (var type in _types) {
        _registeredTypes.Add(type.GetFullGenericName(), type);
      }
    }

    internal static void Invoke(Program program, IPropertySymbol property, bool isGetter) {
      var type = _registeredTypes[property.ContainingType.GetFullGenericName()];
      type = MakeGenericType(type);
      var caller = program.ActiveMethod;
      var target = type.FindAccessor(property, isGetter);
      var nofParameters = target.GetParameters().Length;
      if (!isGetter) {
        nofParameters--;
      }
      var arguments = caller.CollectArguments(nofParameters);
      Model.Object thisReference = null;
      if (!property.IsStaticSymbol()) {
        thisReference = (Model.Object)caller.EvaluationStack.Pop();
      }
      program.RecordCall(new Call(property.ContainingType, thisReference));
      if (!isGetter) {
        var value = caller.EvaluationStack.Pop();
        arguments = arguments.Append(value);
      }
      var result = GuardedInvoke(program, target, thisReference, arguments);
      if (isGetter) {
        caller.EvaluationStack.Push(result);
      }
      program.GoToNextBlock();
    }

    internal static void Invoke(Program program, IMethodSymbol callee) {
      var typeSymbol = callee.ContainingType;
      var type = _registeredTypes[typeSymbol.GetFullGenericName()];
      type = MakeGenericType(type);
      var caller = program.ActiveMethod;
      var arguments = caller.CollectArguments(callee);
      if (callee.IsConstructor()) {
        var result = GuardedCreate(program, typeSymbol, type, arguments);
        caller.EvaluationStack.Push(result);
      } else {
        Model.Object thisReference = null;
        if (!callee.IsStaticSymbol()) {
          thisReference = (Model.Object)caller.EvaluationStack.Pop();
        }
        program.RecordCall(new Call(callee.ContainingType, thisReference));
        object result;
        try {
          var target = type.FindMethod(callee);
          result = GuardedInvoke(program, target, thisReference, arguments);
        } catch (AmbiguousMatchException) {
          result = Unknown.Value;
        }
        if (!callee.ReturnsVoid) {
          caller.EvaluationStack.Push(result);
        }
      }
      program.GoToNextBlock();
    }

    private static Type MakeGenericType(Type type) {
      if (type.IsGenericType) {
        var typeInfo = (System.Reflection.TypeInfo)type;
        var typeArguments = new Type[typeInfo.GenericTypeParameters.Length];
        for (int index = 0; index < typeArguments.Length; index++) {
          typeArguments[index] = typeof(object);
        }
        type = type.MakeGenericType(typeArguments);
      }
      return type;
    }

    private static Model.Object GuardedCreate(Program program, ITypeSymbol typeSymbol, Type type, object[] arguments) {
      arguments = UnwrapSystemArguments(arguments);
      // TODO: Support construction of collections with LINQ query as argument
      if (!FeasibleArguments(arguments)) {
        return Unknown.Value;
      }
      AvoidCollectionOversize(program, typeSymbol, arguments);
      try {
        return new SystemObject(typeSymbol, Activator.CreateInstance(type, arguments));
      } catch (AmbiguousMatchException) {
        // TODO: Support generic information to avoid ambiguous constructors
        return Unknown.Value;
      } catch (MissingMethodException) {
        return Unknown.Value;
      } catch (TargetInvocationException exception) {
        throw new Model.Exception(program.ActiveLocation, exception.InnerException);
      }
    }

    private static readonly Type[] _collectionWithCapacityConstructor = {
      typeof(ArrayList),
      typeof(List<>),
      typeof(Queue<>),
      typeof(SortedList<,>),
      typeof(Stack<>),
      typeof(Dictionary<,>)
    };

    private const int _MaxCollectionCapacity = 100;

    private static void AvoidCollectionOversize(Program program, ITypeSymbol typeSymbol, object[] arguments) {
      var match =
        (from type in _collectionWithCapacityConstructor
         where typeSymbol.IsCompatibleTo(type)
         select type).Any();
      if (match && arguments.Length > 0 && arguments[0] is int capacity) {
        if (capacity > _MaxCollectionCapacity) {
          arguments[0] = _MaxCollectionCapacity;
        }
        program.IncreaseHeapSize(capacity);
      }
    }

    private static object GuardedInvoke(Program program, MethodInfo target, Model.Object thisReference, object[] arguments) {
      arguments = UnwrapSystemArguments(arguments);
      if (!CompatibleThisReference(target, thisReference) || !CompatibleArguments(target.GetParameters(), arguments)) {
        if (target.ReturnType != typeof(void)) {
          return Unknown.Value;
        } else {
          return null;
        }
      } else {
        try {
          object instance = null;
          if (!target.IsStatic) {
            instance = ((SystemObject)thisReference).NativeInstance;
          }
          var result = target.Invoke(instance, arguments);
          if (result != null && CompatibleToSystemType(result.GetType())) {
            // TODO: Provide identical system object for identical result
            // TODO: Associate type symbol of result object
            return new SystemObject(null, result);
          } else {
            return result;
          }
        } catch (TargetInvocationException exception) {
          throw new Model.Exception(program.ActiveLocation, exception.InnerException);
        } catch (ArgumentException exception) {
          throw new Model.Exception(program.ActiveLocation, exception);
        }
      }
    }

    private static bool CompatibleThisReference(MethodInfo target, Model.Object thisReference) {
      if (target.IsStatic) {
        return true;
      }
      if (thisReference is not SystemObject) {
        return false;
      }
      var instance = ((SystemObject)thisReference).NativeInstance;
      // TODO: Need exact generic type information to ensure compatibility in all cases
      return instance != null && target.DeclaringType.IsAssignableFrom(instance.GetType());
    }

    private static object[] UnwrapSystemArguments(object[] arguments) {
      var output = new object[arguments.Length];
      for (int index = 0; index < arguments.Length; index++) {
        output[index] = UnwrapSystemArgument(arguments[index]);
      }
      return output;
    }

    private static object UnwrapSystemArgument(object argument) {
      if (argument is SystemObject systemObject) {
        return systemObject.NativeInstance;
      }
      // TODO: Unwrap query to IEnumerable, needs to trigger full evaluation
      return argument;
    }

    private static bool CompatibleArguments(ParameterInfo[] parameters, object[] arguments) {
      if (parameters.Length != arguments.Length) {
        throw new System.Exception("Argument and parameter lengths differ");
      }
      for (int index = 0; index < parameters.Length; index++) {
        if (!CompatibleArgument(parameters[index].ParameterType, arguments[index])) {
          return false;
        }
      }
      return true;
    }

    private static bool CompatibleArgument(Type parameterType, object argument) {
      if (argument is Model.Object or Lambda or Model.Delegate) {
        return parameterType == typeof(object);
      }
      return true;
    }

    private static bool FeasibleArguments(object[] arguments) {
      return
          (from argument in arguments
           where argument is Lambda or Model.Object or Query
           select argument).Count() == 0;
    }

    private static bool CompatibleToSystemType(Type type) {
      var matchingInterfaces =
        from interfaceType in type.GetInterfaces()
        where IsSystemDefined(interfaceType)
        select interfaceType;
      if (matchingInterfaces.Any()) {
        return true;
      }
      while (type != typeof(object)) {
        if (IsSystemDefined(type)) {
          return true;
        }
        type = type.BaseType;
      }
      return false;
    }

    private static bool IsSystemDefined(Type type) {
      return 
        _types.Contains(type) || 
        type.IsGenericType && _types.Contains(type.GetGenericTypeDefinition());
    }
  }
}
