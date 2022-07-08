using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelChecker.Core.ControlFlow.Routines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ParallelChecker.Core.General {
  internal static class AnalysisInitialization {
    public static IEnumerable<Routine> EntryRoutines(this CompilationModel compilationModel) {
      var watch = Stopwatch.StartNew();
      var entries = new OrderedSet<Routine>();
      var routines = ConcurrentRoutines(compilationModel);
      routines.AddAll(TransitiveCallers(compilationModel, routines));
      foreach (var routine in routines) {
        AddEntry(compilationModel, entries, routine);
      }
      if (entries.Any()) {
        var main = GetMainEntry(compilationModel);
        if (main != null) {
          AddEntry(compilationModel, entries, main);
        }
      }
      Debug.WriteLine($"Analysis entries collected in {watch.ElapsedMilliseconds} ms");
      return entries;
    }

    public static bool IsPrivate(this Routine routine) {
      return routine switch {
        MethodRoutine methodRoutine => methodRoutine.Modifiers.IsPrivate(),
        _ => true
      };
    }

    private static IList<Routine> TransitiveCallers(CompilationModel compilationModel, IEnumerable<Routine> routines) {
      var watch = Stopwatch.StartNew();
      var routinesByType =
        from routine in routines
        group routine by ContainingType(routine);
      var callers =
        (from routineGroup in routinesByType
         let type = routineGroup.Key
         where type != null
         let graph = new CallGraph(compilationModel, type)
         from method in graph.TransitiveCallers(routineGroup)
         select method).ToList();
      Debug.WriteLine($"Transitive callers computed in {watch.ElapsedMilliseconds} ms");
      return callers;
    }

    public static TypeDeclarationSyntax ContainingType(this Routine routine) {
      SyntaxNode node = routine switch {
        ExpressionRoutine expressionRoutine => expressionRoutine.Expression,
        GlobalRoutine => null,
        InitializerRoutine initializerRoutine => initializerRoutine.Type,
        LambdaRoutine lambdaRoutine => lambdaRoutine.Lambda,
        MethodRoutine methodRoutine => methodRoutine.Method,
        PropertyRoutine propertyRoutine => propertyRoutine.Accessor,
        StaticRoutine staticRoutine => staticRoutine.Type,
        _ => throw new NotImplementedException(),
      };
      return node.ContainingType();
    }

    private static IList<Routine> ConcurrentRoutines(CompilationModel compilationModel) {
      var watch = Stopwatch.StartNew();
      var routines =
        (from root in compilationModel.AllRoots()
         from node in root.DescendantNodesAndSelf()
         where compilationModel.InvolvesConcurrency(node)
         from routine in GetRoutines(compilationModel, node)
         select routine).ToList();
      Debug.WriteLine($"Concurrent routines collected in {watch.ElapsedMilliseconds} ms");
      return routines;
    }

    private static IEnumerable<Routine> GetRoutines(CompilationModel compilationModel, SyntaxNode node) {
      var routines = ExtractRoutines(compilationModel, node);
      while (node != null && !routines.Any()) {
        node = node.Parent;
        routines = ExtractRoutines(compilationModel, node);
      }
      return routines;
    }

    private static void AddEntry(CompilationModel compilationModel, OrderedSet<Routine> entries, Routine routine) {
      if (IsModuleInitializer(compilationModel, routine)) {
        var main = GetMainEntry(compilationModel);
        if (main != null) {
          routine = main;
        }
      }
      entries.Add(routine);
    }

    public static TypeDeclarationSyntax GetMainClass(this CompilationModel compilationModel) {
      var entrySymbol = compilationModel.Compilation.GetEntryPoint(compilationModel.CancellationToken);
      if (entrySymbol != null) {
        var typeNode = compilationModel.ResolveSyntaxNode<SyntaxNode>(entrySymbol.ContainingType);
        return typeNode as TypeDeclarationSyntax;
      }
      return null;
    }

    public static Routine GetMainEntry(this CompilationModel compilationModel) {
      var entrySymbol = compilationModel.Compilation.GetEntryPoint(compilationModel.CancellationToken);
      if (entrySymbol != null) {
        var entryNode = compilationModel.ResolveSyntaxNode<SyntaxNode>(entrySymbol);
        if (entryNode != null) {
          return ExtractRoutines(compilationModel, entryNode).FirstOrDefault();
        }
      }
      return null;
    }

    private static bool IsModuleInitializer(CompilationModel compilationModel, Routine routine) {
      return routine is MethodRoutine methodRoutine &&
        methodRoutine.Method is MethodDeclarationSyntax method &&
        compilationModel.IsModuleInitializer(method);
    }

    private static IEnumerable<Routine> ExtractRoutines(CompilationModel compilationModel, SyntaxNode node) {
      compilationModel.ThrowIfCancellationRequested();
      if (node is BaseMethodDeclarationSyntax method) {
        yield return new MethodRoutine(method);
      } else if (node is CompilationUnitSyntax compilationUnit) {
        var topLevelStatements = compilationUnit.GetTopLevelStatements().ToArray();
        if (topLevelStatements.Any()) {
          yield return new GlobalRoutine(topLevelStatements);
        }
      } else if (node is TypeDeclarationSyntax type) {
        yield return new InitializerRoutine(type, false);
        yield return new StaticRoutine(type);
      } else if (node is AnonymousFunctionExpressionSyntax lambda) {
        yield return new LambdaRoutine(lambda);
      } else if (node is AccessorDeclarationSyntax accessor) {
        var routine = GetAccessorRoutine(accessor);
        if (routine != null) {
          yield return routine;
        }
      }
    }

    private static Routine GetAccessorRoutine(AccessorDeclarationSyntax accessor) {
      var isGetter = accessor.Kind() == SyntaxKind.GetAccessorDeclaration;
      if (accessor.ExpressionBody != null) {
        return new ExpressionRoutine(accessor.ExpressionBody.Expression, isGetter);
      } else if (accessor.Body != null) {
        return new PropertyRoutine(accessor);
      } else {
        return null;
      }
    }
  }
}
