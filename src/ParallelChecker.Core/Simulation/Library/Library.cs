using Microsoft.CodeAnalysis;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ParallelChecker.Core.Simulation.Library {
  internal static class Library {
    private static readonly Type[] _types = {
      typeof(AutoResetEvent),
      typeof(AsyncLocal),
      typeof(Array),
      typeof(BackgroundWorker),
      typeof(Barrier),
      typeof(BlockingCollection),
      typeof(CountdownEvent),
      typeof(Index),
      typeof(Interlocked),
      typeof(ManualResetEvent),
      typeof(ManualResetEventSlim),
      typeof(Mutex),
      typeof(Random),
      typeof(Range),
      typeof(ReaderWriterLock),
      typeof(ReaderWriterLockSlim),
      typeof(Semaphore),
      typeof(SemaphoreSlim),
      typeof(String),
      typeof(TaskScheduler),
      typeof(ThreadLocal),
      typeof(Timer),
      typeof(Volatile),
      typeof(Enumerable),
      typeof(ParallelEnumerable),
      typeof(NativeObject),
      typeof(NativeThread)
    };

    private static readonly Dictionary<string, Type> _typeRegistry = new();
    private static readonly Dictionary<string, MethodBase> _methodRegistry = new();
    private static readonly Dictionary<string, FieldInfo> _fieldRegistry = new();

    static Library() {
      RegisterTypes();
    }

    private static void RegisterTypes() {
      foreach (var type in _types) {
        var attribute = (TypeAttribute)type.GetCustomAttribute(typeof(TypeAttribute));
        var typeIdentifier = attribute.Namespace + "." + (attribute.TypeName ?? type.Name);
        _typeRegistry.Add(typeIdentifier, type);
        RegisterMethods(type, typeIdentifier);
        RegisterFields(type, typeIdentifier);
      }
    }

    private static void RegisterMethods(Type type, string typeIdentifier) {
      var allMethods = type.GetMethods().Cast<MethodBase>().Union(type.GetConstructors());
      foreach (var method in allMethods) {
        var attribute = (MemberAttribute)method.GetCustomAttribute(typeof(MemberAttribute));
        if (attribute != null) {
          var nofParameters = method.GetParameters().Length;
          if (HasProgramParameter(method)) {
            nofParameters--;
          }
          if (!method.IsStatic && !method.IsConstructor()) {
            nofParameters++;
          }
          var methodIdentifier = typeIdentifier + "." + (attribute.Name ?? method.Name) + "(" + nofParameters + ")";
          _methodRegistry.Add(methodIdentifier, method);
        }
      }
    }

    private static void RegisterFields(Type type, string typeIdentifier) {
      foreach (var field in type.GetFields()) {
        var attribute = (MemberAttribute)field.GetCustomAttribute(typeof(MemberAttribute));
        if (attribute != null) {
          var fieldIdentifier = typeIdentifier + "." + (attribute.Name ?? field.Name);
          _fieldRegistry.Add(fieldIdentifier, field);
        }
      }
    }

    public static bool IsInLibray(this ITypeSymbol type) {
      return _typeRegistry.ContainsKey(type.GetFullGenericName());
    }

    public static bool IsInLibrary(this IEventSymbol symbol, Program program) {
      if (symbol == null || program.CompilationModel.ContainsSyntaxNode(symbol)) {
        return false;
      }
      return _fieldRegistry.ContainsKey(GetFullGenericName(symbol));
    }

    public static bool IsInLibrary(this IFieldSymbol symbol, Program program) {
      if (symbol == null || program.CompilationModel.ContainsSyntaxNode(symbol)) {
        return false;
      }
      return _fieldRegistry.ContainsKey(GetFullGenericName(symbol));
    }

    public static bool IsInLibrary(this IMethodSymbol method, Program program) {
      if (method == null || program.CompilationModel.ContainsSyntaxNode(method)) {
        return false;
      }
      return SystemObject.IsSystemDefined(method) ||
        _methodRegistry.ContainsKey(GetFullGenericName(program, method)) &&
        !IsOverridden(program, method);
    }

    private static bool IsOverridden(Program program, IMethodSymbol method) {
      if (method.IsStaticSymbol() || method.IsConstructor()) {
        return false;
      }
      var targetMethod = _methodRegistry[GetFullGenericName(program, method)];
      if (targetMethod.IsStatic) {
        return false;
      }
      var evaluationStack = program.ActiveMethod.EvaluationStack.ToList();
      var thisReference = evaluationStack[method.Parameters.Length];
      return thisReference != null && thisReference != Unknown.Value && thisReference.GetType() != targetMethod.DeclaringType;
    }

    public static bool IsInLibrary(this IPropertySymbol property, bool isGetter, Program program) {
      if (property == null || program.CompilationModel.ContainsSyntaxNode(property)) {
        return false;
      }
      return SystemObject.IsSystemDefined(property) ||
        _methodRegistry.ContainsKey(GetFullName(property, isGetter));
    }

    public static void SelectLibraryField(this Program program, IFieldSymbol symbol) {
      SelectLibraryField(program, symbol, GetFullGenericName(symbol));
    }

    public static void SelectLibraryEvent(this Program program, IEventSymbol symbol) {
      SelectLibraryField(program, symbol, GetFullGenericName(symbol));
    }

    private static void SelectLibraryField(Program program, ISymbol symbol, string fullName) {
      object instance = null;
      if (!symbol.IsStaticSymbol()) {
        instance = program.ActiveMethod.EvaluationStack.Pop();
      }
      var target = _fieldRegistry[fullName];
      var variable = new ExplicitVariable(symbol) {
        Value = target.GetValue(instance)
      };
      program.ActiveMethod.EvaluationStack.Push(variable);
      program.GoToNextBlock();
    }

    public static void InvokeLibrary(this Program program, IPropertySymbol property, bool isGetter) {
      if (SystemObject.IsSystemDefined(property)) {
        SystemObject.Invoke(program, property, isGetter);
      } else {
        InvokeProperty(program, property, isGetter);
      }
    }

    private static void InvokeProperty(Program program, IPropertySymbol property, bool isGetter) {
      var target = _methodRegistry[GetFullName(property, isGetter)];
      var caller = program.ActiveMethod;
      object thisReference = null;
      var arguments = new object[0];
      if (!property.IsStaticSymbol()) {
        thisReference = PrepareThisReference(program, target, ref arguments);
      }
      if (thisReference is Model.Object thisObject) {
        program.RecordCall(new Call(property.ContainingType, thisObject));
      }
      if (!isGetter) {
        var value = caller.EvaluationStack.Pop();
        arguments = arguments.AppendArray(new object[] { value });
      }
      var result = Invoke(program, target, thisReference, arguments);
      ApplyResult(program, !isGetter, thisReference, arguments, result);
    }

    public static void InvokeLibrary(this Program program, IMethodSymbol callee) {
      if (SystemObject.IsSystemDefined(callee)) {
        SystemObject.Invoke(program, callee);
      } else {
        InvokeMethod(program, callee);
      }
    }

    private static void InvokeMethod(Program program, IMethodSymbol callee) {
      if (IsLockEntry(callee)) {
        if (program.ActiveThread.NestedLocks > 0 && program.Random.Next(SimulationBounds.NestedLockScheduling) > 0) {
          return;
        }
      }
      var target = _methodRegistry[GetFullGenericName(program, callee)];
      var caller = program.ActiveMethod;
      var arguments = caller.CollectArguments(callee);
      object thisReference = null;
      if (!callee.IsStaticSymbol() && !callee.IsConstructor()) {
        thisReference = PrepareThisReference(program, target, ref arguments);
      }
      if (thisReference is Model.Object thisObject) {
        program.RecordCall(new Call(callee.ContainingType, thisObject));
      }
      var isVoid = callee.ReturnsVoid && !callee.IsConstructor();
      try {
        WaitOnUnknownLocks(program, callee, thisReference, arguments);
        var result = Invoke(program, target, thisReference, arguments);
        ApplyResult(program, isVoid, thisReference, arguments, result);
      } catch (RetryException) {
        caller.PutBackArguments(arguments);
      }
    }

    private static void WaitOnUnknownLocks(Program program, IMethodSymbol callee, object thisReference, object[] arguments) {
      if (IsLockEntry(callee) && (thisReference == Unknown.Value || arguments.Contains(Unknown.Value))) {
        program.ActiveThread.State = ThreadState.Waiting;
      }
    }

    private static readonly HashSet<string> _LockEntryMethods = new() {
      Symbols.SemaphoreSlimWait,
      Symbols.SemaphoreWaitOne,
      Symbols.MutexWaitOne,
      Symbols.MonitorEnter
    };

    private static bool IsLockEntry(IMethodSymbol callee) => callee.IsAny(_LockEntryMethods);

    private static object PrepareThisReference(Program program, MethodBase target, ref object[] arguments) {
      var caller = program.ActiveMethod;
      var thisReference = caller.EvaluationStack.Pop();
      if (thisReference == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (target.IsStatic) {
        arguments = new object[] { thisReference }.AppendArray(arguments);
        thisReference = null;
      }
      return thisReference;
    }

    private static void ApplyResult(Program program, bool isVoid, object thisReference, object[] arguments, object result) {
      var thread = program.ActiveThread;
      var caller = program.ActiveMethod;
      if (thread.State == ThreadState.Runnable) {
        if (!isVoid) {
          caller.EvaluationStack.Push(result);
        }
        program.GoToNextBlock();
      } else {
        if (thisReference != null) {
          caller.EvaluationStack.Push(thisReference);
        }
        foreach (var argument in arguments) {
          caller.EvaluationStack.Push(argument);
        }
      }
    }

    private static object Invoke(Program program, MethodBase target, object thisReference, object[] arguments) {
      if (HasProgramParameter(target)) {
        arguments = new object[] { program }.AppendArray(arguments);
      }
      object result;
      if (thisReference == Unknown.Value || arguments.Contains(Unknown.Value)) {
        result = Unknown.Value;
      } else if (target is ConstructorInfo constructor) {
        result = Activator.CreateInstance(constructor.DeclaringType, arguments);
      } else {
        try {
          result = target.Invoke(thisReference, arguments);
        } catch (ArgumentException) {
          // TODO: Ensure matching library calls in all cases
          result = Unknown.Value;
        } catch (TargetInvocationException exception) {
          var inner = exception.InnerException;
          if (inner is Model.Exception || inner is RetryException) {
            throw inner;
          }
          throw;
        }
      }
      return result;
    }

    private static bool HasProgramParameter(MethodBase target) {
      return target.GetParameters().Length > 0 && 
        target.GetParameters()[0].ParameterType == typeof(Program);
    }

    private static string GetFullName(IPropertySymbol symbol, bool isGetter) {
      var nofParameters = 0;
      if (!symbol.IsStaticSymbol()) {
        nofParameters++;
      }
      var name = symbol.Name;
      if (isGetter) {
        name = "Get" + name;
      } else {
        name = "Set" + name;
        nofParameters++;
      }
      return symbol.ContainingType.GetFullGenericName() + "." + name + "(" + nofParameters + ")";
    }

    private static string GetFullGenericName(IEventSymbol symbol) {
      var typeName = symbol.ContainingType.GetFullGenericName();
      return typeName + "." + symbol.Name;
    }

    private static string GetFullGenericName(IFieldSymbol field) {
      var typeName = field.ContainingType.GetFullGenericName();
      return typeName + "." + field.Name;
    }

    private static string GetFullGenericName(Program program, IMethodSymbol symbol) {
      var typeName = symbol.ContainingType.GetFullGenericName();
      var nofParameters = symbol.Parameters.Length;
      if (!symbol.IsStaticSymbol() && !symbol.IsConstructor()) {
        nofParameters++;
        var dynamicTypeName = InferDynamicTypeName(program);
        if (dynamicTypeName != null) {
          typeName = dynamicTypeName;
        }
      }
      return typeName + "." + symbol.Name + "(" + nofParameters + ")";
    }

    private static string InferDynamicTypeName(Program program) {
      if (program.ActiveMethod.EvaluationStack.Peek() is Model.Object thisReference) {
        var type = thisReference.GetType();
        var attribute = (TypeAttribute)type.GetCustomAttribute(typeof(TypeAttribute));
        if (attribute != null) {
          return attribute.Namespace + "." + (attribute.TypeName ?? type.Name);
        }
      }
      return null;
    }
  }
}
