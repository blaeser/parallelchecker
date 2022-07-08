using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelChecker.Core.ControlFlow.Routines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.General {
  internal sealed class CallGraph {
    private readonly CompilationModel _compilationModel;
    private readonly ISymbol _scope;
    private readonly OrderedSet<Routine> _routines = new();
    private readonly MultiDictionary<Routine, Routine> _incomingCalls = new();
    // Optimization
    private readonly HashSet<string> _potentialCallees;

    public CallGraph(CompilationModel compilationModel, TypeDeclarationSyntax root) {
      _compilationModel = compilationModel;
      _scope = compilationModel.GetDeclaredSymbol(root);
      _potentialCallees = new(GetCalleeNames(root));
      PopulateNodes(root);
      PopulateEdges();
    }

    private IEnumerable<string> GetCalleeNames(TypeDeclarationSyntax root) {
      return
        from node in root.DescendantNodesAndSelf(item => item is TypeDeclarationSyntax or BaseMethodDeclarationSyntax or PropertyDeclarationSyntax)
        where node is TypeDeclarationSyntax or BaseMethodDeclarationSyntax or PropertyDeclarationSyntax
        select node.GetIdentifer();
    }

    private void PopulateNodes(TypeDeclarationSyntax root) {
      foreach (var routine in CollectAllRoutines(root)) {
        _compilationModel.ThrowIfCancellationRequested();
        _routines.Add(routine);
      }
    }

    private void PopulateEdges() {
      foreach (var caller in _routines) {
        foreach (var callee in ExtractCalls(caller)) {
          _compilationModel.ThrowIfCancellationRequested();
          _incomingCalls.Add(callee, caller);
        }
      }
    }

    public ISet<Routine> TransitiveCallers(IEnumerable<Routine> targets) {
      var initials = new HashSet<Routine>();
      var visited = new HashSet<Routine>();
      var pending = new Queue<Routine>();
      foreach (var target in targets) {
        pending.Enqueue(target);
      }
      while (pending.Count > 0) {
        _compilationModel.ThrowIfCancellationRequested();
        var current = pending.Dequeue();
        int count = 0;
        if (_incomingCalls.ContainsKey(current)) {
          foreach (var incoming in _incomingCalls[current]) {
            if (!visited.Contains(incoming)) {
              count++;
              visited.Add(incoming);
              pending.Enqueue(incoming);
            }
          }
        }
        if (count == 0) {
          initials.Add(current);
        }
      }
      return initials;
    }

    private IEnumerable<Routine> CollectAllRoutines(TypeDeclarationSyntax root) {
      return
        from node in root.DescendantNodesAndSelf(item => item is TypeDeclarationSyntax or BaseMethodDeclarationSyntax or BasePropertyDeclarationSyntax)
        from routine in ExtractRoutines(node)
        select routine;
    }

    // TODO: Dynamic calls (delegates etc.) are not considered
    private IEnumerable<Routine> ExtractCalls(Routine method) {
      return
        from body in GetSyntaxNodes(method)
        from caller in body.DescendantNodes()
        from callee in ExtractCalls(caller)
        select callee;
    }

    // TODO: Simplify all these routine types, especially property accesses
    private IEnumerable<Routine> ExtractCalls(SyntaxNode callerNode) {
      if (IsPotentialCall(callerNode) && _compilationModel.GetReferencedSymbol(callerNode) is ISymbol symbol && symbol.ContainingType == _scope) {
        var calleeNode = _compilationModel.ResolveSyntaxNode<SyntaxNode>(symbol);
        if (calleeNode is BasePropertyDeclarationSyntax property) {
          if (property is PropertyDeclarationSyntax fullProperty && fullProperty.ExpressionBody != null) {
            yield return new ExpressionRoutine(fullProperty.ExpressionBody.Expression, false);
          } else if (property is IndexerDeclarationSyntax indexer && indexer.ExpressionBody != null) {
            yield return new ExpressionRoutine(indexer.ExpressionBody.Expression, false);
          } else {
            var isGetter = callerNode.Parent is not AssignmentExpressionSyntax;
            var routine = GetAccessorRoutine(property, isGetter);
            if (routine != null) {
              yield return routine;
            }
          }
        } else if (calleeNode is BaseMethodDeclarationSyntax) {
          yield return new MethodRoutine(calleeNode);
        } else if (calleeNode is AnonymousFunctionExpressionSyntax lambda) {
          yield return new LambdaRoutine(lambda);
        }
        if (callerNode is ObjectCreationExpressionSyntax or AnonymousObjectCreationExpressionSyntax or ImplicitObjectCreationExpressionSyntax) {
          var typeNode = _compilationModel.ResolveSyntaxNode<TypeDeclarationSyntax>(symbol.ContainingType);
          foreach (var callee in ExtractRoutines(typeNode)) {
            yield return callee;
          }
        }
      }
    }

    private bool IsPotentialCall(SyntaxNode callerNode) {
      return callerNode is ThisExpressionSyntax ||
        callerNode is IdentifierNameSyntax identifier &&
        _potentialCallees.Contains(identifier.Identifier.Value);
    }

    private IEnumerable<SyntaxNode> GetSyntaxNodes(Routine routine) {
      if (routine is MethodRoutine method) {
        if (method.Body != null) {
          yield return method.Body;
        }
        if (method.ExpressionBody != null) {
          yield return method.ExpressionBody;
        }
      } else if (routine is GlobalRoutine globalRoutine) {
        foreach (var statement in globalRoutine.TopLevelStatements) {
          yield return statement;
        }
      } else if (routine is InitializerRoutine initializer) {
        foreach (var member in initializer.Type.GetInitializableMembers()) {
          yield return member;
        }
      } else if (routine is StaticRoutine staticRoutine) {
        foreach (var member in staticRoutine.Type.GetStaticMembers()) {
          yield return member;
        }
      } else if (routine is LambdaRoutine lambda) {
        yield return lambda.Lambda;
      } else if (routine is ExpressionRoutine expression) {
        yield return expression.Expression;
      } else if (routine is PropertyRoutine property) {
        yield return property.Accessor;
      }
    }

    private IEnumerable<Routine> ExtractRoutines(SyntaxNode node) {
      _compilationModel.CancellationToken.ThrowIfCancellationRequested();
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

    private Routine GetAccessorRoutine(AccessorDeclarationSyntax accessor) {
      var isGetter = accessor.Kind() == SyntaxKind.GetAccessorDeclaration;
      if (accessor.ExpressionBody != null) {
        return new ExpressionRoutine(accessor.ExpressionBody.Expression, isGetter);
      } else if (accessor.Body != null) {
        return new PropertyRoutine(accessor);
      } else {
        return null;
      }
    }

    private Routine GetAccessorRoutine(BasePropertyDeclarationSyntax property, bool isGetter) {
      var accessor =
        (from node in property.AccessorList.Accessors
         where isGetter && node.Kind() == SyntaxKind.GetAccessorDeclaration ||
           !isGetter && (node.Kind() == SyntaxKind.SetAccessorDeclaration || node.Kind() == SyntaxKind.InitAccessorDeclaration)
         select node).SingleOrDefault();
      if (accessor != null) {
        return GetAccessorRoutine(accessor);
      } else {
        return null;
      }
    }
  }
}
