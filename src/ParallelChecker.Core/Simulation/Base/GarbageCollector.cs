using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelChecker.Core.ControlFlow.Routines;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Base {
  internal static class GarbageCollector {
    private const int _GCTimeCost = 100;
    private const int _GCMarkCost = 10;

    public static void CollectGarbage(this Program program) {
      if (program.Finalizeables.Any()) {
        program.IncreaseSimulationTime(_GCTimeCost);
        program.RecordVariations();
        var rootSet = GetRootSet(program);
        var marked = new HashSet<Object>();
        foreach (var instance in rootSet) {
          Mark(program, instance, marked);
        }
        var garbage = program.Finalizeables.Minus(marked);
        var existing = program.AllThreads.ToList();
        foreach (var instance in garbage) {
          program.Finalizeables.Remove(instance);
          Finalize(program, instance, existing);
        }
      }
    }

    private static void Finalize(Program program, Object instance, IEnumerable<Thread> existing) {
      if (instance.Type != null) {
        foreach (var destructor in instance.Type.GetDestructors()) {
          StartFinalizer(program, instance, destructor, existing);
        }
      }
    }

    private static void StartFinalizer(Program program, Object instance, IMethodSymbol method, IEnumerable<Thread> existing) {
      var declaration = program.CompilationModel.ResolveSyntaxNode<DestructorDeclarationSyntax>(method);
      if (declaration != null) {
        program.IncreaseSimulationTime(_GCTimeCost);
        var routine = new MethodRoutine(declaration);
        var graph = program.ControlFlowModel.GetGraph(routine);
        var cause = new Cause("finalizer", declaration.Identifier.GetLocation());
        var callee = new Method(method, graph.Entry, null, cause) {
          ThisReference = instance
        };
        var thread = new Thread(program, cause, callee) {
          State = ThreadState.Runnable,
          SyncWithStatic = true
        };
        program.AllThreads.Add(thread);
        thread.AdvanceTime();
        SynchronizeToAll(thread, existing);
        program.Finalizer.Register(thread);
      }
    }

    private static void SynchronizeToAll(Thread thread, IEnumerable<Thread> existing) {
      foreach (var other in existing) {
        other.AdvanceTime();
        thread.SynchronizeWith(other);
      }
    }

    private static void Mark(Program program, Object instance, ISet<Object> marked) {
      program.IncreaseSimulationTime(1);
      if (!marked.Contains(instance)) {
        program.IncreaseSimulationTime(_GCMarkCost);
        marked.Add(instance);
        foreach (var reference in GetReferences(instance)) {
          Mark(program, reference, marked);
        }
      }
    }

    private static IEnumerable<Object> GetReferences(Object instance) {
      return
        from variable in instance.InstanceFields.AllVariables()
        let value = variable.Value
        where value is Object
        select (Object)value;
    }

    private static IEnumerable<Object> GetRootSet(Program program) {
      var allMethods =
        from thread in program.AllThreads
        from method in thread.CallStack
        select method;
      var evalValues =
        from method in allMethods
        from value in method.EvaluationStack
        select value;
      var localValues =
        from method in allMethods
        from variable in method.LocalVariables.AllVariables()
        select variable.Value;
      var thisReferences =
        from method in allMethods
        select method.ThisReference;
      var staticValues =
        from variable in program.StaticFields.AllVariables()
        select variable.Value;
      var combinedValues = 
        evalValues.
        Union(localValues).
        Union(thisReferences).
        Union(staticValues).
        Union(program.ExtraRootSet);
      return
        from value in combinedValues
        where value is Object
        select (Object)value;
    }
  }
}
