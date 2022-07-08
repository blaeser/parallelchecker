using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ParallelChecker.Core.General {
  internal class CompilationModel {
    public Compilation Compilation { get; }
    private readonly HashSet<Compilation> _allCompilations = new();
    private readonly Dictionary<SyntaxTree, SemanticModel> _semanticModels = new();
    private readonly Dictionary<ISymbol, SyntaxNode> _syntaxNodes = new(SymbolEqualityComparer.Default);
    private readonly List<ClassDeclarationSyntax> _finalizableClasses = new();
    public List<TypeDeclarationSyntax> VolatileTypes { get; } = new();
    // Optimization
    private List<MethodDeclarationSyntax> _allMethods = null;
    private HashSet<string> _finalizableNames = null;
    private IList<MethodDeclarationSyntax> _moduleInitializers = null;
    
    public CancellationToken CancellationToken { get; }

    public CompilationModel(Compilation compilation, CancellationToken cancellationToken) {
      Compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
      CancellationToken = cancellationToken;
      RegisterAllCompilations();
      RegisterSemanticModels();
      RegisterInitialSyntaxNodes();
    }

    private void RegisterAllCompilations() {
      var pending = new Queue<Compilation>();
      pending.Enqueue(Compilation);
      while (pending.Count > 0) {
        var current = pending.Dequeue();
        if (!_allCompilations.Contains(current)) {
          _allCompilations.Add(current);
          foreach (var reference in GetReferences(current)) {
            CancellationToken.ThrowIfCancellationRequested();
            pending.Enqueue(reference);
          }
        }
      }
    }

    private IEnumerable<Compilation> GetReferences(Compilation source) {
      return
        from reference in source.References
        where reference is CompilationReference
        let target = ((CompilationReference)reference).Compilation
        where target.Language == LanguageNames.CSharp
        select target;
    }

    private void RegisterSemanticModels() {
      foreach (var compilation in _allCompilations) {
        foreach (var tree in compilation.SyntaxTrees) {
          CancellationToken.ThrowIfCancellationRequested();
          var model = compilation.GetSemanticModel(tree);
          _semanticModels.Add(tree, model);
        }
      }
    }

    private void RegisterInitialSyntaxNodes() {
      foreach (var compilation in _allCompilations) {
        CancellationToken.ThrowIfCancellationRequested();
        var nodes =
         from tree in compilation.SyntaxTrees
         from child in tree.GetRoot(CancellationToken).DescendantNodesAndSelf(
           n => n is CompilationUnitSyntax or NamespaceDeclarationSyntax or TypeDeclarationSyntax or MemberDeclarationSyntax or VariableDeclarationSyntax or VariableDeclaratorSyntax)
         select child;
        foreach (var node in nodes) {
          CancellationToken.ThrowIfCancellationRequested();
          GetDeclaredSymbol(node);
          if (node is DestructorDeclarationSyntax) {
            _finalizableClasses.Add((ClassDeclarationSyntax)node.Parent);
          }
          if (node is FieldDeclarationSyntax field && field.Modifiers.IsVolatile()) {
            VolatileTypes.Add((TypeDeclarationSyntax)field.Parent);
          }
        }
      }
    }

    public SemanticModel GetSemanticModel(SyntaxNode node) {
      CancellationToken.ThrowIfCancellationRequested();
      return _semanticModels[node.SyntaxTree];
    }

    public T ResolveSyntaxNode<T>(ISymbol symbol) where T : SyntaxNode {
      CancellationToken.ThrowIfCancellationRequested();
      _syntaxNodes.TryGetValue(symbol, out var result);
      if (result == null) {
        _syntaxNodes.TryGetValue(symbol.MakeGeneric(), out result);
      }
      return (T)result;
    }

    public bool ContainsSyntaxNode(ISymbol symbol) {
      CancellationToken.ThrowIfCancellationRequested();
      if (symbol == null) {
        return false;
      }
      if (_syntaxNodes.ContainsKey(symbol)) {
        return true;
      }
      if (symbol is INamedTypeSymbol namedType) {
        var generic = namedType.ConstructedFrom;
        return _syntaxNodes.ContainsKey(generic);
      }
      return false;
    }

    public ISymbol GetReferencedSymbol(SyntaxNode node) {
      var model = GetSemanticModel(node);
      var symbolInfo = model.GetSymbolInfo(node, CancellationToken);
      return symbolInfo.Symbol;
    }

    public ISymbol GetDeclaredSymbol(SyntaxNode declaration) {
      var model = GetSemanticModel(declaration);
      var symbol = model.GetDeclaredSymbol(declaration, CancellationToken);
      if (symbol != null) {
        _syntaxNodes[symbol] = declaration;
      }
      return symbol;
    }

    public ITypeSymbol GetNodeType(SyntaxNode node) {
      CancellationToken.ThrowIfCancellationRequested();
      var typeInfo = GetTypeInfo(node);
      return typeInfo.Type ?? typeInfo.ConvertedType;
    }

    public TypeInfo GetTypeInfo(SyntaxNode node) {
      var model = GetSemanticModel(node);
      return model.GetTypeInfo(node, CancellationToken);
    }

    public void ThrowIfCancellationRequested() {
      CancellationToken.ThrowIfCancellationRequested();
    }

    // TODO: Introduce a lazy load abstraction

    public ICollection<MethodDeclarationSyntax> AllMethods {
      get {
        if (_allMethods == null) {
          _allMethods = this.GetAllMethods().ToList();
        }
        return _allMethods;
      }
    }

    public ISet<string> FinalizableTypes {
      get {
        if (_finalizableNames == null) {
          _finalizableNames = new HashSet<string>(
            from node in _finalizableClasses
            select node.Identifier.ValueText);
        }
        return _finalizableNames;
      }
    }

    public IList<MethodDeclarationSyntax> ModuleInitializers {
      get {
        if (_moduleInitializers == null) {
          _moduleInitializers = this.RetrieveModuleInitializers();
        }
        return _moduleInitializers;
      }
    }
  }
}
