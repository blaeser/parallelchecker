using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.ControlFlow.Routines;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Base {
  internal static class InitializationLogic {
    public static List<ProgramEntry> GetProgramEntries(this CompilationModel compilationModel) {
      var result = new List<ProgramEntry>();
      var routines = compilationModel.EntryRoutines();
      var mainClass = compilationModel.GetMainClass();
      var concurrent = 
        from routine in routines 
        where IsConcurrent(compilationModel, routine, mainClass) 
        select routine;
      result.AddAll(MultiCallEntries(compilationModel, concurrent, true));
      routines = routines.Minus(concurrent);
      var multiple = 
        from routine in routines 
        where IsMultiple(routine, mainClass) 
        select routine;
      result.AddAll(MultiCallEntries(compilationModel, multiple, false));
      routines = routines.Minus(multiple);
      result.AddAll(SingleCallEntries(compilationModel, routines));
      return result;
    }

    private static IEnumerable<ProgramEntry> MultiCallEntries(CompilationModel compilationModel, IEnumerable<Routine> entryRoutines, bool concurrent) {
      var routinesByType =
              from routine in entryRoutines
              group routine by routine.ContainingType();
      foreach (var groupItem in routinesByType) {
        var group = groupItem.ToList();
        foreach (var first in group) {
          var pairing = group;
          if (group.Count * group.Count > SimulationBounds.EntryCombinationLimit) {
            pairing = new() { first };
          }
          foreach (var second in pairing) {
            compilationModel.ThrowIfCancellationRequested();
            var entry = new ProgramEntry() {
              Concurrent = concurrent
            };
            entry.Routines.Add(first);
            entry.Routines.Add(second);
            yield return entry;
          }
        }
      }
    }

    private static IEnumerable<ProgramEntry> SingleCallEntries(CompilationModel compilationModel, IEnumerable<Routine> entryRoutines) {
      foreach (var routine in entryRoutines) {
        compilationModel.ThrowIfCancellationRequested();
        var entry = new ProgramEntry();
        entry.Routines.Add(routine);
        yield return entry;
      }
    }

    private static bool IsMultiple(Routine routine, TypeDeclarationSyntax mainClass) {
      return !routine.IsPrivate() && routine.ContainingType() != mainClass && routine is not StaticRoutine and not InitializerRoutine;
    }

    private static bool IsConcurrent(CompilationModel compilationModel, Routine routine, TypeDeclarationSyntax mainClass) {
      return
        IsMultiple(routine, mainClass) &&
        (from parent in routine.Nodes
        from node in parent.DescendantNodesAndSelf()
        where compilationModel.AccessesLock(node)
        select node).Any();
    }

    public static bool InitializeStatics(this Program program, ITypeSymbol type) {
      if (type != null && !program.LoadedTypes.Contains(type)) {
        program.LoadedTypes.Add(type);
        if (program.CompilationModel.ResolveSyntaxNode<SyntaxNode>(type) is TypeDeclarationSyntax declaration) {
          var routine = new StaticRoutine(declaration);
          var graph = program.ControlFlowModel.GetGraph(routine);
          var method = new Method(null, graph.Entry, null, program.StaticLoader.Cause);
          program.StaticLoader.CallStack.Push(method);
          program.StaticLoader.State = ThreadState.Runnable;
          program.AllThreads.Add(program.StaticLoader);
          return true;
        }
      }
      return false;
    }

    public static void InitializeEntry(this Program program, ProgramEntry entry) {
      program.ResetThreads();
      RandomInitialize(program, entry);
    }

    private static void SetupInitialThread(Program program, Cause cause) {
      var initMethod = new Method(null, new ExitBlock(Location.None), null, cause);
      var initThread = new Thread(program, cause, initMethod) {
        State = ThreadState.Runnable,
        SyncWithStatic = true
      };
      program.AllThreads.Add(initThread);
      program.ActiveThread = initThread;
    }

    public static void SetupMainExecution(this Program program, ProgramEntry entry) {
      SetupRandomEntry(program, entry);
    }

    private static void RandomInitialize(Program program, ProgramEntry entry) {
      // TODO: Consider other entries such public variables, properties, constructors and indexers
      SetupInitialThread(program, new Cause("initial thread", Location.None));
      if (program.CompilationModel.Compilation.Options.OutputKind == OutputKind.WindowsApplication) {
        entry.Dispatcher = new Dispatcher(program.Random);
        entry.Dispatcher.Register(program.ActiveThread);
      }
      entry.Instances.Clear();
      foreach (var routine in entry.Routines) {
        // TODO: Support other routines like properties etc.
        var method = GetEntryMethod(routine);
        if (method != null) {
          if (!method.Modifiers.IsStatic()) {
            var type = (TypeDeclarationSyntax)method.Parent;
            if (!entry.Instances.ContainsKey(type)) {
              entry.Instances.Add(type, CreateObject(program, type));
            }
          }
        }
      }
    }

    private static void SetupRandomEntry(Program program, ProgramEntry entry) {
      SetupInitialThread(program, new Cause("initial thread", Location.None));
      program.ResetThreads();
      List<Routine> concurrent = new();
      if (entry.Concurrent) {
        concurrent.AddAll(entry.Routines);
      } else {
        var first = entry.Routines.FirstOrDefault();
        if (first != null) {
          concurrent.Add(first);
        }
      }
      bool repeated = false;
      foreach (var routine in concurrent) {
        if (routine is MethodRoutine method) {
          var explanation = repeated ? "second thread (assumed due to lock occurrence)" : "initial thread";
          SetupInitialMethod(program, entry, routine, explanation);
        } else if (routine is GlobalRoutine globalRoutine) {
          SetupGlobalRoutine(program, entry, globalRoutine);
        } else if (routine is StaticRoutine staticRoutine) {
          SetupStaticRoutine(program, entry, staticRoutine);
        }
        repeated = true;
      }
      // TODO: Support other routine types
    }

    private static void SetupStaticRoutine(Program program, ProgramEntry entry, StaticRoutine staticRoutine) {
      if (program.CompilationModel.GetDeclaredSymbol(staticRoutine.Type) is ITypeSymbol typeSymbol) {
         InitializeStatics(program, typeSymbol);
      }
      if (entry.Routines.Count > 1) {
        throw new NotImplementedException();
      }
    }

    private static void SetupGlobalRoutine(Program program, ProgramEntry entry, GlobalRoutine globalRoutine) {
      var cause = new Cause("initial thread", globalRoutine.Location);
      var thread = StartInitialThread(program, cause, globalRoutine);
      entry.Dispatcher?.Register(thread);
      SetupSubsequentRoutines(program, thread, entry);
    }

    private static void SetupInitialMethod(Program program, ProgramEntry entry, Routine routine, string explanation) {
      var method = GetEntryMethod(routine);
      if (method != null) {
        var type = (TypeDeclarationSyntax)method.Parent;
        Model.Object instance = null;
        if (!method.Modifiers.IsStatic()) {
          instance = entry.Instances[type];
        }
        var cause = new Cause(explanation, method.Identifier.GetLocation());
        var thread = StartInitialThread(program, cause, method, instance);
        entry.Dispatcher?.Register(thread);
        if (IsRepeatableEvent(program, method)) {
          cause = new Cause("UI event", method.Identifier.GetLocation());
          thread = StartInitialThread(program, cause, method, instance);
          entry.Dispatcher?.Register(thread);
        }
        SetupSubsequentRoutines(program, thread, entry);
      }
    }

    private static void SetupSubsequentRoutines(Program program, Thread thread, ProgramEntry entry) {
      if (entry.Concurrent) {
        return;
      }
      for (int index = 1; index < entry.Routines.Count; index++) {
        var routine = entry.Routines[index];
        var method = GetEntryMethod(routine);
        if (method != null) {
          var type = (TypeDeclarationSyntax)method.Parent;
          Model.Object instance = null;
          if (!method.Modifiers.IsStatic()) {
            instance = entry.Instances[type];
          }
          var cause = new Cause("potential subsequent call", Location.None, thread.Cause);
          if (program.CompilationModel.GetDeclaredSymbol(method) is IMethodSymbol symbol) {
            if (!symbol.ReturnsVoid) {
              thread.DiscardResult();
            }
            var callee = CreateMethod(program, method, instance, cause);
            thread.CallStack.Push(callee);
          }
        }
        // TODO: Support other routine types
      }
    }

    private static Thread StartInitialThread(Program program, Cause cause, GlobalRoutine routine) {
      var graph = program.ControlFlowModel.GetGraph(routine);
      var model = program.CompilationModel;
      var entrySymbol = model.Compilation.GetEntryPoint(model.CancellationToken);
      var callee = new Method(entrySymbol, graph.Entry, null, new Cause($"top-level statements", Location.None, cause));
      program.Variations++;
      var thread = new Thread(program, cause, callee) {
        State = ThreadState.Runnable,
        SyncWithStatic = true
      };
      program.AllThreads.Add(thread);
      InitializeStatics(program, new MethodDeclarationSyntax[0]);
      return thread;
    }

    private static MethodDeclarationSyntax GetEntryMethod(Routine routine) {
      if (routine is MethodRoutine methodRoutine && methodRoutine.Method is MethodDeclarationSyntax method) {
        return method;
      }
      return null;
    }

    private static bool IsRepeatableEvent(Program program, MethodDeclarationSyntax method) {
      if (program.CompilationModel.Compilation.Options.OutputKind == OutputKind.WindowsApplication) {
        var userInterfaceEvents = program.CompilationModel.AllUserInterfaceEvents().Select(e => !Symbols.NonRepeatedEvents.Contains(e.Name));
        return userInterfaceEvents.Contains(method);
      }
      return false;
    }

    private static Thread StartInitialThread(Program program, Cause cause, MethodDeclarationSyntax methodDeclaration, Model.Object thisReference) {
      return StartInitialThread(program, cause, new MethodDeclarationSyntax[] { methodDeclaration }, thisReference);
    }

    private static Thread StartInitialThread(Program program, Cause cause, MethodDeclarationSyntax[] methodDeclarations, Model.Object thisReference) {
      var methods = methodDeclarations.Reverse().ToArray();
      var callee = CreateMethod(program, methods[0], thisReference, cause);
      var thread = new Thread(program, cause, callee) {
        State = ThreadState.Runnable,
        SyncWithStatic = true
      };
      for (int index = 1; index < methods.Length; index++) {
        callee = CreateMethod(program, methods[index], thisReference, cause);
        thread.CallStack.Push(callee);
      }
      program.AllThreads.Add(thread);
      InitializeStatics(program, methods);
      return thread;
    }

    private static void InitializeStatics(Program program, MethodDeclarationSyntax[] methods) {
      var moduleInitializers = program.CompilationModel.ModuleInitializers;
      InitializeModule(program, moduleInitializers);
      foreach (var method in methods.Union(moduleInitializers)) {
        var symbol = program.CompilationModel.GetDeclaredSymbol(method);
        var type = symbol?.ContainingType;
        if (type != null) {
          program.InitializeStatics(type);
        }
      }
    }

    private static void InitializeModule(Program program, IList<MethodDeclarationSyntax> moduleInitializers) {
      if (!program.ModuleInitialized) {
        program.ModuleInitialized = true;
        foreach (var initializer in moduleInitializers.Reverse()) {
          RunStaticInitializer(program, initializer);
        }
      }
    }

    private static void RunStaticInitializer(Program program, MethodDeclarationSyntax initializer) {
      var cause = new Cause("module initializer", initializer.GetLocation());
      var method = CreateMethod(program, initializer, null, cause);
      program.StaticLoader.CallStack.Push(method);
      program.StaticLoader.State = ThreadState.Runnable;
      program.AllThreads.Add(program.StaticLoader);
    }

    private static Method CreateMethod(Program program, MethodDeclarationSyntax methodDeclaration, Model.Object thisReference, Cause cause) {
      var model = program.CompilationModel;
      var methodSymbol = (IMethodSymbol)model.GetDeclaredSymbol(methodDeclaration);
      var routine = new MethodRoutine(methodDeclaration);
      var graph = program.ControlFlowModel.GetGraph(routine);
      var callee = new Method(methodSymbol, graph.Entry, null, new Cause($"call {methodSymbol}", Location.None, cause));
      var invent = program.Random.Next(2) == 0;
      program.Variations++;
      foreach (var parameter in methodSymbol.Parameters) {
        var genericParam = parameter.MakeGeneric();
        var value = invent && parameter.RefKind == RefKind.None ? InventValue(program, genericParam.Type, 0) : Unknown.Value;
        callee.LocalVariables[genericParam].Value = value;
      }
      if (!methodSymbol.IsStaticSymbol()) {
        callee.ThisReference = thisReference;
      }
      return callee;
    }

    private const int RecursiveInventionBound = 1;

    private static object InventValue(Program program, ITypeSymbol type, int recursion) {
      var random = program.Random;
      program.IncreaseSimulationTime(1);
      if (type.Is(Symbols.BoolType)) {
        return random.Next(1) == 0;
      }
      if (type.IsPrimitiveType()) {
        return random.Next().Convert(type.GetNativeType());
      }
      if (recursion > RecursiveInventionBound) {
        return Unknown.Value;
      }
      if (type is IArrayTypeSymbol arrayType) {
        var lengths = new int[arrayType.Rank];
        for (int dimension = 0; dimension < arrayType.Rank; dimension++) {
          lengths[dimension] = random.Next(100);
        }
        var array = new Model.Array(lengths, 1, arrayType.ElementType, program);
        foreach (var variable in array.AllVariables()) {
          variable.Value = InventValue(program, arrayType.ElementType, recursion + 1);
        }
        return array;
      }
      if (type is INamedTypeSymbol namedType && !namedType.IsInLibray()) {
        var instance = new Model.Object(type);
        foreach (var field in namedType.MakeGeneric().GetInstanceFields()) {
          instance.InstanceFields[field].Value = InventValue(program, field.Type, recursion + 1);
        }
        return instance;
      }
      return Unknown.Value;
    }

    private static Model.Object CreateObject(Program program, TypeDeclarationSyntax typeDeclaration) {
      var typeSymbol = (INamedTypeSymbol)program.CompilationModel.GetDeclaredSymbol(typeDeclaration);
      var instance = new Model.Object(typeSymbol);
      var constructor = typeSymbol.Constructors.FirstOrDefault();
      var constructorDeclaration = typeDeclaration.GetPublicConstructors().FirstOrDefault();
      if (constructorDeclaration != null) {
        constructor = (IMethodSymbol)program.CompilationModel.GetDeclaredSymbol(constructorDeclaration);
      }
      if (constructor != null) {
        var arguments = new object[constructor.Parameters.Length];
        for (var index = 0; index < arguments.Length; index++) {
          var parameter = constructor.Parameters[index];
          arguments[index] = parameter.RefKind == RefKind.None ? InventValue(program, parameter.Type, 0) : Unknown.Value;
        }
        program.InvokeConstructor(instance, constructor, arguments);
        for (var index = 0; index < arguments.Length; index++) {
          var parameter = constructor.Parameters[index];
          var argument = arguments[index] as Model.Object;
          InvokeDefaultConstructor(program, parameter.Type, argument);
        }
      }
      program.ExtraRootSet.Add(instance);
      return instance;
    }

    private static void InvokeDefaultConstructor(Program program, ITypeSymbol type, Model.Object instance) {
      if (instance != null && type is INamedTypeSymbol namedType) {
        var defaultConstructor = namedType.GetDefaultConstructor();
        if (defaultConstructor != null) {
          program.InvokeConstructor(instance, defaultConstructor, new object[] { });
        }
      }
    }
  }
}
