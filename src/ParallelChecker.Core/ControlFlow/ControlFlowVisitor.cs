using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.ControlFlow.Routines;
using ParallelChecker.Core.ControlFlow.Scopes;
using ParallelChecker.Core.General;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace ParallelChecker.Core.ControlFlow {
  internal class ControlFlowVisitor : CSharpSyntaxVisitor {
    private readonly CompilationModel _compilationModel;

    public ControlFlowVisitor(CompilationModel compilationModel) {
      _compilationModel = compilationModel;
    }

    private class Branch {
      public ISet<Connection> Front { get; }
      public BasicBlock Target { get; }

      public Branch(ISet<Connection> front, BasicBlock target) {
        Front = front;
        Target = target;
      }
    }

    private delegate void Connection(BasicBlock successor);

    public ControlFlowGraph Graph { get; private set; }
    private ISet<Connection> _front = new HashSet<Connection>();
    private readonly Stack<Scope> _openScopes = new();
    private readonly IDictionary<string, EmptyBlock> _gotoLabels = new Dictionary<string, EmptyBlock>();
    private readonly IList<Branch> _unresolvedBranches = new List<Branch>();

    public void VisitRoutine(Routine routine) {
      var stopwatch = Stopwatch.StartNew();
      Graph = new ControlFlowGraph(routine);
      NewFront(next => Graph.Entry.Successor = next);
      VisitRoutineBody(routine);
      SucceedFront(Graph.Exit);
      ResolveBranches();
    }

    private void VisitRoutineBody(Routine routine) {
      if (routine is StaticRoutine staticRoutine) {
        VisitStaticRoutine(staticRoutine);
      } else if (routine is MethodRoutine methodRoutine) {
        VisitMethodRoutine(methodRoutine);
      } else if (routine is PropertyRoutine) {
        VisitPropertyRoutine(routine);
      } else if (routine is LambdaRoutine lambdaRoutine) {
        VisitLambdaRoutine(lambdaRoutine);
      } else if (routine is InitializerRoutine initializerRoutine) {
        VisitInitializerRoutine(initializerRoutine);
      } else if (routine is ExpressionRoutine expressionRoutine) {
        VisitExpressionRoutine(expressionRoutine);
      } else if (routine is GlobalRoutine globalRoutine) {
        VisitGlobalRoutine(globalRoutine);
      } else {
        throw new NotImplementedException();
      }
    }

    private void VisitGlobalRoutine(GlobalRoutine globalRoutine) {
      foreach (var globalStatement in globalRoutine.TopLevelStatements) {
        Visit(globalStatement.Statement);
      }
    }

    private void VisitInitializerRoutine(InitializerRoutine initializerRoutine) {
      var type = initializerRoutine.Type;
      if (initializerRoutine.StructDefault) {
        VisitStructDefaultDeclarations(type);
      } else {
        VisitInstanceDeclarations(type);
      }
    }

    private void VisitExpressionRoutine(ExpressionRoutine expressionRoutine) {
      var expression = expressionRoutine.Expression;
      Visit(expression);
      if (!_compilationModel.IsVoid(expression) && expressionRoutine.ReturnsVoid) {
        var discard = new DiscardBlock(expression.GetLocation());
        ExpandFront(discard);
      }
    }

    private void VisitMethodRoutine(MethodRoutine routine) {
      var method = routine.Method;
      if (method is ConstructorDeclarationSyntax constructor) {
        if (constructor.Initializer != null) {
          Visit(constructor.Initializer);
        }
      }
      var modifiers = routine.Modifiers;
      if (modifiers.IsAbstract() || modifiers.IsExtern()) {
        throw new NotImplementedException();
      }
      var symbol = (IMethodSymbol)_compilationModel.GetDeclaredSymbol(method);
      if (routine.Body != null) {
        Visit(routine.Body);
        ImplicitYieldReturn(method.GetLocation(), symbol);
      } else if (routine.ExpressionBody != null) {
        var arrow = routine.ExpressionBody;
        VisitLambdaBody(arrow.Expression, symbol);
      } else if (modifiers.IsPartial()) {
        if (!symbol.ReturnsVoid) {
          ExpandFront(new UnknownBlock(method.GetLocation()));
        }
      } else {
        throw new NotImplementedException();
      }
    }

    private void VisitPropertyRoutine(Routine routine) {
      var accessor = ((PropertyRoutine)routine).Accessor;
      var symbol = (IMethodSymbol)_compilationModel.GetDeclaredSymbol(accessor);
      Visit(accessor.Body);
      ImplicitYieldReturn(accessor.GetLocation(), symbol);
    }

    private void VisitLambdaRoutine(LambdaRoutine routine) {
      var body = routine.Lambda.Body;
      var lambda = (IMethodSymbol)_compilationModel.GetReferencedSymbol(routine.Lambda);
      VisitLambdaBody(body, lambda);
    }

    private void VisitLambdaBody(CSharpSyntaxNode body, IMethodSymbol lambda) {
      Visit(body);
      if (body is ExpressionSyntax expression && !_compilationModel.IsVoid(expression) && lambda.ReturnsVoid) {
        var discard = new DiscardBlock(body.GetLocation());
        ExpandFront(discard);
      }
    }

    private void ResolveBranches() {
      foreach (var branch in _unresolvedBranches) {
        NewFront(branch.Front);
        SucceedFront(branch.Target);
      }
      _unresolvedBranches.Clear();
      _front = null;
    }

    private void VisitStaticRoutine(StaticRoutine routine) {
      foreach (var member in routine.Type.GetStaticMembers()) {
        if (member is ConstructorDeclarationSyntax) {
          if (_compilationModel.GetDeclaredSymbol(member) is IMethodSymbol constructor) {
            ExpandFront(new InvocationBlock(member.GetLocation(), constructor, false));
          }
        } else {
          Visit(member);
        }
      }
    }

    private void VisitStructDefaultDeclarations(TypeDeclarationSyntax typeDeclaration) {
      foreach (var variable in typeDeclaration.GetStructDefaultInitializableMembers()) {
        Visit(variable);
      }
    }

    private void VisitInstanceDeclarations(TypeDeclarationSyntax typeDeclaration) {
      foreach (var member in typeDeclaration.GetInstanceInitializableMembers()) {
        Visit(member);
      }
    }

    public override void VisitFieldDeclaration(FieldDeclarationSyntax node) {
      Visit(node.Declaration);
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node) {
      // accessors will be visited separatedly as roots
      if (node.Initializer != null) {
        Visit(node.Initializer);
        CopyStruct(node.Initializer.Value);
        VisitVariableWrite(_compilationModel.GetDeclaredSymbol(node), node.GetLocation());
      }
    }

    public override void VisitBlock(BlockSyntax node) {
      var scope = new BlockScope();
      _openScopes.Push(scope);
      foreach (var statement in node.Statements) {
        Visit(statement);
      }
      _openScopes.Pop();
      HandleDisposes(scope);
    }

    private void HandleDisposes(BlockScope scope) {
      if (scope.EnterTry != null) {
        var location = scope.EnterTry.Location;
        ExpandFront(new ExitTryBlock(location));
        AddCatches(location, scope.EnterTry, null);
        AddFinallyBlock(location, scope.EnterTry, () => {
          foreach (var declaration in scope.Disposables) {
            foreach (var action in PrepareUsingDisposes(declaration, null)) {
              action();
            }
          }
        });
      }
    }

    public override void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node) {
      Visit(node.Expression);
      ImplicitCast(node);
    }

    public override void VisitExpressionStatement(ExpressionStatementSyntax node) {
      Visit(node.Expression);
      if (!_compilationModel.IsVoid(node.Expression)) {
        var block = new DiscardBlock(node.GetLocation());
        ExpandFront(block);
      }
      ImplicitCast(node);
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node) {
      if (IsNameOfExpression(node)) {
        VisitNameOfExpression(node);
      } else {
        var method = (IMethodSymbol)_compilationModel.GetReferencedSymbol(node);
        if (method == null) {
          IgnoreUnknownInvocation(node);
        } else {
          VisitRegularInvocation(node, method);
        }
      }
    }

    private bool IsNameOfExpression(InvocationExpressionSyntax node) {
      return node.Expression is IdentifierNameSyntax identifierName && identifierName.Identifier.Text == "nameof";
    }

    private void VisitNameOfExpression(InvocationExpressionSyntax node) {
      var argument = node.ArgumentList.Arguments[0].Expression;
      var value = argument.GetLastIdentifier();
      ExpandFront(new ConstantBlock(node.GetLocation(), value));
    }

    private void VisitRegularInvocation(InvocationExpressionSyntax node, IMethodSymbol method) {
      bool isVirtual = method.IsVirtual || method.IsAbstract;
      if (method.MethodKind == MethodKind.DelegateInvoke) {
        Visit(node.Expression);
      } else if (!method.IsStaticSymbol()) {
        if (node.Expression is MemberAccessExpressionSyntax access) {
          if (access.Expression is BaseExpressionSyntax) {
            isVirtual = false;
            ExpandFront(new ThisBlock(node.Expression.GetLocation()));
          } else if (HasThisRefParameter(method)) {
            VisitDesignator(access.Expression, access.Expression.GetLocation());
          } else {
            Visit(access.Expression);
          }
        } else if (node.Expression is IdentifierNameSyntax ||
                   node.Expression is GenericNameSyntax) {
          ExpandFront(new ThisBlock(node.GetLocation()));
        } else if (node.Expression is MemberBindingExpressionSyntax) {
          // expression already visited
        } else {
          throw new NotImplementedException();
        }
      }
      if (method.IsOpImplicit()) {
        Visit(node.Expression);
      } else {
        VisitArgumentList(node.ArgumentList, method.Parameters);
      }
      if (method.IsExtensionMethod && method.MethodKind == MethodKind.ReducedExtension) {
        method = method.ReducedFrom;
      }
      var block = new InvocationBlock(node.GetLocation(), method, isVirtual);
      ExpandFront(block);
      ImplicitCast(node);
    }

    private static bool HasThisRefParameter(IMethodSymbol method) {
      return method.IsExtensionMethod && method.MethodKind == MethodKind.ReducedExtension &&
        method.ReducedFrom.Parameters[0].RefKind == RefKind.Ref;
    }

    private void IgnoreUnknownInvocation(InvocationExpressionSyntax node) {
      if (node.Expression != null) {
        Visit(node.Expression);
        ExpandFront(new DiscardBlock(node.GetLocation()));
      }
      foreach (var argument in node.ArgumentList.Arguments) {
        Visit(argument);
        ExpandFront(new DiscardBlock(node.GetLocation()));
      }
      if (!_compilationModel.IsVoid(node)) {
        ExpandFront(new UnknownBlock(node.GetLocation()));
      }
    }

    private void VisitArgumentList(ArgumentListSyntax argumentList, ImmutableArray<IParameterSymbol> parameterList) {
      var index = 0;
      while (index < argumentList.Arguments.Count && !parameterList[index].IsParams && argumentList.Arguments[index].NameColon == null) {
        var argument = argumentList.Arguments[index];
        var parameter = parameterList[index];
        VisitArgument(argument, parameter);
        index++;
      }
      VisitSpecialArguments(argumentList, parameterList, index);
    }

    private void VisitArgument(ArgumentSyntax argument, IParameterSymbol parameter) {
      if (parameter.RefKind == RefKind.None) {
        Visit(argument);
      } else if (parameter.RefKind == RefKind.In && !IsDesignator(argument.Expression)) {
        Visit(argument.Expression);
      } else if (parameter.RefKind == RefKind.In || parameter.RefKind == RefKind.Out || parameter.RefKind == RefKind.Ref) {
        VisitDesignator(argument.Expression, argument.GetLocation());
      } else {
        throw new NotImplementedException();
      }
    }

    private void VisitSpecialArguments(ArgumentListSyntax argumentList, ImmutableArray<IParameterSymbol> parameterList, int index) {
      while (index < parameterList.Length) {
        var parameter = parameterList[index];
        if (parameter.IsParams) {
          VisitOpenParameterList(argumentList, index, parameter);
          index = parameterList.Length;
        } else {
          VisitNamedOrOptionalArgument(argumentList, parameter, index);
          index++;
        }
      }
    }

    private void VisitNamedOrOptionalArgument(ArgumentListSyntax argumentList, IParameterSymbol parameter, int index) {
      var argument = FindNamedArgument(argumentList, parameter.Name);
      if (argument != null) {
        VisitArgument(argument, parameter);
      } else if (parameter.IsOptional) {
        SetOptionalParameter(argumentList.GetLocation(), parameter);
      } else {
        VisitArgument(argumentList.Arguments[index], parameter);
      }
    }

    private void FillInOptionalParameters(Location location, IMethodSymbol method) {
      foreach (var parameter in method.Parameters) {
        if (parameter.IsOptional) {
          SetOptionalParameter(location, parameter);
        } else {
          throw new Exception();
        }
      }
    }

    private void SetOptionalParameter(Location location, IParameterSymbol parameter) {
      var value = parameter.ExplicitDefaultValue;
      if (value == null && parameter.Type.IsValueType && !parameter.Type.Is(Symbols.Nullable)) {
        var type = (INamedTypeSymbol)parameter.Type;
        ExpandFront(new ObjectCreationBlock(location, type, type.GetDefaultConstructor(), 0));
      } else {
        ExpandFront(new ConstantBlock(location, value));
      }
    }

    private ArgumentSyntax FindNamedArgument(ArgumentListSyntax argumentList, string name) {
      return
        (from argument in argumentList.Arguments
         where argument.NameColon != null
         let identifier = argument.NameColon.Name.Identifier
         where MatchingArgumentNames(identifier.Text, name)
         select argument).SingleOrDefault();
    }

    private bool MatchingArgumentNames(string identifier, string name) {
      if (identifier.StartsWith("@")) {
        identifier = identifier.Substring(1);
      }
      return identifier == name;
    }

    private void VisitOpenParameterList(ArgumentListSyntax argumentList, int argumentIndex, IParameterSymbol parameter) {
      var remainder = argumentList.Arguments.Count - argumentIndex;
      var arrayPassed = false;
      var first = true;
      while (argumentIndex < argumentList.Arguments.Count) {
        var argument = argumentList.Arguments[argumentIndex];
        var type = _compilationModel.GetNodeType(argument.Expression);
        Visit(argument);
        if (first && remainder == 1 && type != null && IsArrayTypeCompatible(parameter.Type, type)) {
          arrayPassed = true;
          break;
        }
        first = false;
        argumentIndex++;
      }
      if (!arrayPassed) {
        ExpandFront(new ArrayInitializerBlock(argumentList.GetLocation(), parameter.Type, 1, Math.Max(0, remainder)));
      }
    }

    private bool IsArrayTypeCompatible(ITypeSymbol target, ITypeSymbol source) {
      if (source.Equals(target, SymbolEqualityComparer.Default)) {
        return true;
      }

      var sourceArray = source as IArrayTypeSymbol;
      var targetArray = target as IArrayTypeSymbol;
      if (sourceArray?.ElementType?.IsValueType != false || targetArray?.ElementType?.IsValueType != false) {
        return false;
      }

      return sourceArray.ElementType.IsCompatibleTo(targetArray.ElementType);
    }

    public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node) {
      var type = (ITypeSymbol)_compilationModel.GetReferencedSymbol(node.Type);
      var constructor = (IMethodSymbol)_compilationModel.GetReferencedSymbol(node);
      VisitNewExpression(node, type, constructor, node.ArgumentList, node.Initializer);
    }

    public override void VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node) {
      var constructor = (IMethodSymbol)_compilationModel.GetReferencedSymbol(node);
      var type = constructor?.ContainingType;
      VisitNewExpression(node, type, constructor, node.ArgumentList, node.Initializer);
    }

    private void VisitNewExpression(SyntaxNode node, ITypeSymbol type, IMethodSymbol constructor, ArgumentListSyntax argumentList, InitializerExpressionSyntax initializer) {
      int nofParameters = 0;
      if (argumentList != null) {
        if (constructor == null) {
          Visit(argumentList);
        } else {
          VisitArgumentList(argumentList, constructor.Parameters);
        }
        nofParameters = argumentList.Arguments.Count;
      } else if (constructor != null && constructor.Parameters.Length > 0) {
        FillInOptionalParameters(node.GetLocation(), constructor);
      }
      ExpandFront(new ObjectCreationBlock(node.GetLocation(), type, constructor, nofParameters));
      ImplicitCast(node);
      Visit(initializer);
    }

    public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node) {
      var type = (ITypeSymbol)_compilationModel.GetDeclaredSymbol(node);
      ExpandFront(new ObjectCreationBlock(node.GetLocation(), type, null, 0));
      foreach (var initializer in node.Initializers) {
        Visit(initializer);
      }
    }

    public override void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node) {
      ExpandFront(new DuplicateBlock(node.GetLocation()));
      Visit(node.Expression);
      var property = (IPropertySymbol)_compilationModel.GetDeclaredSymbol(node);
      ExpandFront(new SwapBlock(node.GetLocation()));
      ExpandFront(new PropertySetBlock(node.GetLocation(), property));
    }

    public override void VisitArgumentList(ArgumentListSyntax node) {
      foreach (var argument in node.Arguments) {
        Visit(argument);
      }
    }

    public override void VisitArgument(ArgumentSyntax node) {
      Visit(node.Expression);
      CopyStruct(node.Expression);
    }

    public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node) {
      VisitLambdaExpression(node);
    }

    public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node) {
      VisitLambdaExpression(node);
    }

    public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node) {
      VisitLambdaExpression(node);
    }

    private void VisitLambdaExpression(AnonymousFunctionExpressionSyntax node) {
      // lambda is not part of the control flow, unless it is the root
      var block = new ConstantBlock(node.GetLocation(), node);
      ExpandFront(block);
      ImplicitCast(node);
    }

    public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node) {
      // not part of the control flow, unless it is the root
      _compilationModel.GetDeclaredSymbol(node);
    }

    private readonly IDictionary<SyntaxKind, SyntaxKind> _assignmentOperators = new Dictionary<SyntaxKind, SyntaxKind> {
      { SyntaxKind.AddAssignmentExpression, SyntaxKind.AddExpression },
      { SyntaxKind.SubtractAssignmentExpression, SyntaxKind.SubtractExpression },
      { SyntaxKind.MultiplyAssignmentExpression, SyntaxKind.MultiplyExpression },
      { SyntaxKind.DivideAssignmentExpression, SyntaxKind.DivideExpression },
      { SyntaxKind.ModuloAssignmentExpression, SyntaxKind.ModuloExpression },
      { SyntaxKind.AndAssignmentExpression, SyntaxKind.BitwiseAndExpression },
      { SyntaxKind.OrAssignmentExpression, SyntaxKind.BitwiseOrExpression },
      { SyntaxKind.ExclusiveOrAssignmentExpression, SyntaxKind.ExclusiveOrExpression },
      { SyntaxKind.LeftShiftAssignmentExpression, SyntaxKind.LeftShiftExpression },
      { SyntaxKind.RightShiftAssignmentExpression, SyntaxKind.RightShiftExpression }
    };

    public override void VisitAssignmentExpression(AssignmentExpressionSyntax node) {
      var location = node.GetLocation();
      var kind = node.Kind();
      if (IsEvent(node.Left)) {
        Visit(node.Right);
        VisitDesignator(node.Left, location);
        ExpandFront(new OperatorBlock(location, kind, null));
      } else {
        if (kind == SyntaxKind.SimpleAssignmentExpression) {
          Visit(node.Right);
          if (node.Left is TupleExpressionSyntax tuple) {
            var variableTypes = (from argument in tuple.Arguments
                                 select _compilationModel.GetNodeType(argument.Expression)).ToArray();
            if (_compilationModel.TryGetDeconstructionMethod(variableTypes, node.Right, out var deconstructionMethod)) {
              VisitAnyTypeDeconstruction(node, tuple.Arguments.ToList(), deconstructionMethod);
              return;
            }
          } else if (node.Left is DeclarationExpressionSyntax declaration && declaration.Designation is ParenthesizedVariableDesignationSyntax varList) {
            var variableTypes = (from variable in varList.Variables
                                 select _compilationModel.GetNodeType(variable)).ToArray();
            VisitTupleDeclaration(location, varList);
            return;
          }
        } else if (_assignmentOperators.ContainsKey(kind)) {
          Visit(node.Left);
          Visit(node.Right);
          var newKind = _assignmentOperators[kind];
          var method = (IMethodSymbol)_compilationModel.GetReferencedSymbol(node);
          ExpandFront(new OperatorBlock(location, newKind, method));
          ExtraCastOnIncDec(node);
        } else if (kind == SyntaxKind.CoalesceAssignmentExpression) {
          Visit(node.Left);
          ExpandFront(new DuplicateBlock(node.GetLocation()));
          ExpandFront(new ConstantBlock(node.GetLocation(), null));
          ExpandFront(new OperatorBlock(node.GetLocation(), SyntaxKind.EqualsExpression, null));
          var branch = new BranchBlock(node.GetLocation());
          SucceedFront(branch);
          NewFront(next => branch.SuccessorOnTrue = next);
          ExpandFront(new DiscardBlock(node.GetLocation()));
          Visit(node.Right);
          ExpandFront(new DuplicateBlock(node.GetLocation()));
          VisitWriteBlock(node.Left, node.GetLocation());
          var afterIf = _front;
          NewFront(next => branch.SuccessorOnFalse = next);
          NewFront(_front.Union(afterIf));
        } else {
          throw new NotImplementedException();
        }
        CopyStruct(node.Right);
        ExpandFront(new DuplicateBlock(location));
        VisitWriteBlock(node.Left, location);
        ImplicitCast(node);
      }
    }

    private void VisitAnyTypeDeconstruction(AssignmentExpressionSyntax node, List<ArgumentSyntax> arguments, IMethodSymbol deconstructionMethod) {
      var isVirtual = deconstructionMethod.IsVirtual || deconstructionMethod.IsAbstract;
      for (int index = 0; index < arguments.Count; index++) {
        var argument = arguments[index];
        VisitDesignator(argument.Expression, argument.GetLocation());
      }
      ExpandFront(new InvocationBlock(node.GetLocation(), deconstructionMethod, isVirtual));
      ExpandFront(new ConstantBlock(node.GetLocation(), null));
    }

    private void VisitTupleDeclaration(Location location, ParenthesizedVariableDesignationSyntax list) {
      for (int index = 0; index < list.Variables.Count; index++) {
        ExpandFront(new DuplicateBlock(location));
        ExpandFront(new ConstantBlock(location, index));
        ExpandFront(new ElementSelectionBlock(location, 1));
        ExpandFront(new ReadBlock(location));
        VisitVariableDesignation(list.Variables[index], location);
        ExpandFront(new WriteBlock(location));
      }
    }

    public override void VisitQualifiedName(QualifiedNameSyntax node) {
      VisitNameSyntax(node);
    }

    public override void VisitIdentifierName(IdentifierNameSyntax node) {
      VisitNameSyntax(node);
    }

    private void VisitNameSyntax(NameSyntax node) {
      var symbol = _compilationModel.GetReferencedSymbol(node);
      if (symbol == null) {
        ExpandFront(new UnknownBlock(node.GetLocation()));
      } else if (IsConstant(symbol)) {
        if (IsUnknownConstantValue(symbol)) {
          ExpandFront(new UnknownBlock(node.GetLocation()));
        } else {
          var value = GetConstantValue(symbol);
          ExpandFront(new ConstantBlock(node.GetLocation(), value));
        }
      } else if (IsVariableSymbol(symbol)) {
        if (IsInstanceVariable(symbol)) {
          ExpandFront(new ThisBlock(node.GetLocation()));
        }
        ExpandFront(new VariableSelectionBlock(node.GetLocation(), symbol));
        ExpandFront(new ReadBlock(node.GetLocation()));
      } else if (symbol is IPropertySymbol property) {
        if (!property.IsStatic) {
          ExpandFront(new ThisBlock(node.GetLocation()));
        }
        ExpandFront(new PropertyGetBlock(node.GetLocation(), property));
      } else if (symbol is IMethodSymbol) {
        // delegate
        if (!symbol.IsStaticSymbol()) {
          ExpandFront(new ThisBlock(node.GetLocation()));
        }
        ExpandFront(new ConstantBlock(node.GetLocation(), symbol));
        ImplicitCast(node);
      } else if (symbol is INamespaceOrTypeSymbol) {
        ExpandFront(new ConstantBlock(node.GetLocation(), symbol));
      } else if (symbol is IDiscardSymbol) {
        // TODO: Better support than unknown for discard "_" symbol
        ExpandFront(new UnknownBlock(node.GetLocation()));
      } else {
        throw new NotImplementedException();
      }
    }

    public override void VisitLiteralExpression(LiteralExpressionSyntax node) {
      if (node.Token.IsKind(SyntaxKind.DefaultKeyword)) {
        var type = _compilationModel.GetNodeType(node);
        GenerateDefaultValue(node.GetLocation(), type);
      } else {
        ExpandFront(new ConstantBlock(node.GetLocation(), node.Token.Value));
        ImplicitCast(node);
      }
    }

    public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) {
      Visit(node.Declaration);
      if (node.UsingKeyword.IsKind(SyntaxKind.UsingKeyword)) {
        var block = GetInnermostScope<BlockScope>();
        if (block.EnterTry == null) {
          block.EnterTry = new EnterTryBlock(node.GetLocation());
          ExpandFront(block.EnterTry);
        }
        block.Disposables.Add(node.Declaration);
      }
    }

    public override void VisitVariableDeclaration(VariableDeclarationSyntax node) {
      foreach (var declarator in node.Variables) {
        Visit(declarator);
      }
    }

    public override void VisitVariableDeclarator(VariableDeclaratorSyntax node) {
      if (node.ArgumentList != null) {
        var variable = _compilationModel.GetDeclaredSymbol(node);
        var type = variable.GetVariableType();
        VisitFixedArrayCreation(type, node.ArgumentList.Arguments, node.GetLocation());
        VisitVariableWrite(variable, node.GetLocation());
      }
      if (node.Initializer != null && !IsConstantDeclarator(node)) {
        Visit(node.Initializer);
        CopyStruct(node.Initializer.Value);
        var variable = _compilationModel.GetDeclaredSymbol(node);
        if (((VariableDeclarationSyntax)node.Parent).Type.IsKind(SyntaxKind.RefType)) {
          ExpandFront(new AliasBlock(node.GetLocation(), variable));
        } else {
          VisitVariableWrite(variable, node.GetLocation());
        }
      }
    }

    public override void VisitEqualsValueClause(EqualsValueClauseSyntax node) {
      Visit(node.Value);
    }

    public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node) {
      Visit(node.Expression);
      ExpandFront(new DuplicateBlock(node.GetLocation()));
      ExpandFront(new ConstantBlock(node.GetLocation(), null));
      ExpandFront(new OperatorBlock(node.GetLocation(), SyntaxKind.NotEqualsExpression, null));
      var branch = new BranchBlock(node.GetLocation());
      SucceedFront(branch);
      NewFront(next => branch.SuccessorOnTrue = next);
      Visit(node.WhenNotNull);
      var afterIf = _front;
      NewFront(next => branch.SuccessorOnFalse = next);
      if (node.WhenNotNull is not ConditionalAccessExpressionSyntax &&
          _compilationModel.IsVoid(node.WhenNotNull)) {
        ExpandFront(new DiscardBlock(node.GetLocation()));
      }
      NewFront(_front.Union(afterIf));
    }

    public override void VisitMemberBindingExpression(MemberBindingExpressionSyntax node) {
      var member = _compilationModel.GetReferencedSymbol(node);
      if (member == null) {
        ExpandFront(new DiscardBlock(node.GetLocation()));
        ExpandFront(new UnknownBlock(node.GetLocation()));
      } else if (IsVariableSymbol(member)) {
        ExpandFront(new VariableSelectionBlock(node.GetLocation(), member));
        ExpandFront(new ReadBlock(node.GetLocation()));
      } else if (member is IPropertySymbol property) {
        ExpandFront(new PropertyGetBlock(node.GetLocation(), property));
      } else if (member is IMethodSymbol) {
        // no action
      } else {
        throw new NotImplementedException();
      }
      ImplicitCast(node);
    }

    public override void VisitElementBindingExpression(ElementBindingExpressionSyntax node) {
      Visit(node.ArgumentList);
      var dimensions = node.ArgumentList.Arguments.Count;
      ExpandFront(new ElementSelectionBlock(node.GetLocation(), dimensions));
      ExpandFront(new ReadBlock(node.GetLocation()));
      ImplicitCast(node);
    }

    public override void VisitConditionalExpression(ConditionalExpressionSyntax node) {
      Visit(node.Condition);
      var branch = new BranchBlock(node.GetLocation());
      SucceedFront(branch);
      NewFront(next => branch.SuccessorOnTrue = next);
      Visit(node.WhenTrue);
      var afterIf = _front;
      NewFront(next => branch.SuccessorOnFalse = next);
      Visit(node.WhenFalse);
      NewFront(_front.Union(afterIf));
    }

    public override void VisitIfStatement(IfStatementSyntax node) {
      Visit(node.Condition);
      var branch = new BranchBlock(node.GetLocation());
      SucceedFront(branch);
      NewFront(next => branch.SuccessorOnTrue = next);
      Visit(node.Statement);
      var afterIf = _front;
      NewFront(next => branch.SuccessorOnFalse = next);
      if (node.Else != null) {
        Visit(node.Else);
      }
      NewFront(_front.Union(afterIf));
    }

    public override void VisitElseClause(ElseClauseSyntax node) {
      Visit(node.Statement);
    }

    public override void VisitSwitchStatement(SwitchStatementSyntax node) {
      var breakEnd = new EmptyBlock(node.GetLocation());
      Visit(node.Expression);
      _openScopes.Push(new SwitchScope(breakEnd));
      var onTrue = new HashSet<Connection>();
      foreach (var section in node.Sections) {
        var regularLabels = GetRegularCaseLabels(section);
        if (regularLabels.Any()) {
          foreach (var label in regularLabels) {
            ExpandFront(new DuplicateBlock(node.GetLocation()));
            Visit(label);
            var branch = new BranchBlock(node.GetLocation());
            SucceedFront(branch);
            onTrue.Add(next => branch.SuccessorOnTrue = next);
            NewFront(next => branch.SuccessorOnFalse = next);
          }
          var onFalse = _front;
          NewFront(onTrue);
          ExpandFront(new DiscardBlock(section.GetLocation()));
          SetGotoCaseEntries(regularLabels);
          Visit(section);
          onTrue = new HashSet<Connection>(_front);
          NewFront(onFalse);
        }
      }
      ExpandFront(new DiscardBlock(node.GetLocation()));
      var defaultLabel = GetDefaultSwitchLabel(node);
      if (defaultLabel != null) {
        SetGotoDefaultEntry(defaultLabel);
        var section = (SwitchSectionSyntax)defaultLabel.Parent;
        Visit(section);
      }
      _openScopes.Pop();
      ExpandFront(breakEnd);
    }

    private DefaultSwitchLabelSyntax GetDefaultSwitchLabel(SwitchStatementSyntax statement) {
      return
        (from section in statement.Sections
         from label in section.Labels
         where label is DefaultSwitchLabelSyntax
         select (DefaultSwitchLabelSyntax)label).SingleOrDefault();
    }

    private IEnumerable<SwitchLabelSyntax> GetRegularCaseLabels(SwitchSectionSyntax section) {
      return from label in section.Labels
             where label is not DefaultSwitchLabelSyntax
             select label;
    }

    private void SetGotoCaseEntries(IEnumerable<SwitchLabelSyntax> labels) {
      var entries =
        from label in labels
        where label is CaseSwitchLabelSyntax
        let caseLabel = (CaseSwitchLabelSyntax)label
        let value = _compilationModel.ConstantValue(caseLabel.Value)
        let entry = GetGotoCaseEntry(value, caseLabel.GetLocation())
        where entry != null
        select entry;
      foreach (var entry in entries) {
        ExpandFront(entry);
      }
    }

    private void SetGotoDefaultEntry(DefaultSwitchLabelSyntax label) {
      var entry = GetGotoDefaultEntry(label.GetLocation());
      ExpandFront(entry);
    }

    public override void VisitCaseSwitchLabel(CaseSwitchLabelSyntax node) {
      var value = _compilationModel.ConstantValue(node.Value);
      ExpandFront(new ConstantBlock(node.GetLocation(), value));
      ExpandFront(new OperatorBlock(node.GetLocation(), SyntaxKind.EqualsExpression, null));
    }

    public override void VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node) {
      Visit(node.Pattern);
      if (node.WhenClause != null) {
        var branch = new BranchBlock(node.GetLocation());
        SucceedFront(branch);
        NewFront(next => branch.SuccessorOnTrue = next);
        Visit(node.WhenClause);
        var afterWhen = _front;
        NewFront(next => branch.SuccessorOnFalse = next);
        ExpandFront(new ConstantBlock(node.GetLocation(), false));
        NewFront(_front.Union(afterWhen));
      }
    }

    public override void VisitWhenClause(WhenClauseSyntax node) {
      Visit(node.Condition);
    }

    public override void VisitSwitchSection(SwitchSectionSyntax node) {
      foreach (var statement in node.Statements) {
        Visit(statement);
      }
    }

    public override void VisitVarPattern(VarPatternSyntax node) {
      if (node.Designation is ParenthesizedVariableDesignationSyntax varList) {
        VisitTupleDeclaration(node.GetLocation(), varList);
        ExpandFront(new DiscardBlock(node.GetLocation()));
      } else {
        VisitVariableWrite(_compilationModel.GetDeclaredSymbol(node.Designation), node.GetLocation());
      }
      ExpandFront(new ConstantBlock(node.GetLocation(), true));
    }

    private T GetInnermostScope<T>() where T : Scope {
      foreach (var scope in _openScopes) {
        if (scope is T typedScope) {
          return typedScope;
        }
      }
      throw new NotImplementedException();
    }

    private void ExitScopesUntil(Location location, Scope limit) {
      foreach (var scope in _openScopes) {
        if (scope == limit) {
          return;
        }
        ExitSingleScope(location, scope);
      }
    }

    private void ExitSingleScope(Location location, Scope scope) {
      if (scope is LockScope) {
        ExpandFront(new UnlockBlock(location));
      }
      if (scope is IteratorScope) {
        ExpandFront(new IteratorEndBlock(location));
      }
      if (scope is TryScope) {
        ExpandFront(new ExitTryBlock(location));
      }
      if (scope is BlockScope block) {
        HandleDisposes(block);
      }
      // TODO: Add try-catch block (for finally)
    }

    public override void VisitBreakStatement(BreakStatementSyntax node) {
      var breakScope = GetInnermostScope<BreakScope>();
      ExitScopesUntil(node.GetLocation(), breakScope);
      _unresolvedBranches.Add(new Branch(_front, breakScope.Exit));
      NewFront();
    }

    public override void VisitContinueStatement(ContinueStatementSyntax node) {
      var loopScope = GetInnermostScope<LoopScope>();
      ExitScopesUntil(node.GetLocation(), loopScope);
      _unresolvedBranches.Add(new Branch(_front, loopScope.Entry));
      NewFront();
    }

    public override void VisitWhileStatement(WhileStatementSyntax node) {
      var continueBegin = new EmptyBlock(node.GetLocation());
      ExpandFront(continueBegin);
      Visit(node.Condition);
      var branch = new BranchBlock(node.GetLocation());
      SucceedFront(branch);
      NewFront(next => branch.SuccessorOnTrue = next);
      var breakEnd = new EmptyBlock(node.GetLocation());
      _openScopes.Push(new LoopScope(continueBegin, breakEnd));
      UndeclareLocals(node.Statement);
      UndeclareLocals(node.Condition);
      Visit(node.Statement);
      _openScopes.Pop();
      SucceedFront(continueBegin);
      NewFront(next => branch.SuccessorOnFalse = next);
      ExpandFront(breakEnd);
    }

    public override void VisitDoStatement(DoStatementSyntax node) {
      var entry = new EmptyBlock(node.GetLocation());
      ExpandFront(entry);
      var breakEnd = new EmptyBlock(node.GetLocation());
      var continueBegin = new EmptyBlock(node.GetLocation());
      _openScopes.Push(new LoopScope(continueBegin, breakEnd));
      UndeclareLocals(node.Statement);
      UndeclareLocals(node.Condition);
      Visit(node.Statement);
      _openScopes.Pop();
      ExpandFront(continueBegin);
      Visit(node.Condition);
      var branch = new BranchBlock(node.GetLocation());
      SucceedFront(branch);
      branch.SuccessorOnTrue = entry;
      NewFront(next => branch.SuccessorOnFalse = next);
      ExpandFront(breakEnd);
    }

    public override void VisitForStatement(ForStatementSyntax node) {
      Visit(node.Declaration);
      var entry = new EmptyBlock(node.GetLocation());
      ExpandFront(entry);
      if (node.Condition != null) {
        Visit(node.Condition);
      } else {
        ExpandFront(new ConstantBlock(node.GetLocation(), true));
      }
      var branch = new BranchBlock(node.GetLocation());
      SucceedFront(branch);
      NewFront(next => branch.SuccessorOnTrue = next);
      var breakEnd = new EmptyBlock(node.GetLocation());
      var continueBegin = new EmptyBlock(node.GetLocation());
      _openScopes.Push(new LoopScope(continueBegin, breakEnd));
      UndeclareLocals(node.Statement);
      UndeclareLocals(node.Condition);
      Visit(node.Statement);
      _openScopes.Pop();
      ExpandFront(continueBegin);
      foreach (var incrementor in node.Incrementors) {
        Visit(incrementor);
        if (!_compilationModel.IsVoid(incrementor)) {
          var block = new DiscardBlock(node.GetLocation());
          ExpandFront(block);
        }
      }
      SucceedFront(entry);
      NewFront(next => branch.SuccessorOnFalse = next);
      ExpandFront(breakEnd);
    }

    public override void VisitForEachStatement(ForEachStatementSyntax node) {
      // TODO: Handle asynchronous streams, i.e. await foreach.
      var variable = (ILocalSymbol)_compilationModel.GetDeclaredSymbol(node);
      Visit(node.Expression);
      ExpandFront(new IteratorStartBlock(node.GetLocation()));
      var foreachEnd = new IteratorEndBlock(node.GetLocation());
      var continueBegin = new IteratorNextBlock(node.GetLocation(), variable);
      SucceedFront(continueBegin);
      NewFront(next => continueBegin.SuccessorOnContinue = next);
      _openScopes.Push(new IteratorScope(continueBegin, foreachEnd));
      UndeclareLocals(node.Statement);
      Visit(node.Statement);
      _openScopes.Pop();
      SucceedFront(continueBegin);
      NewFront(next => continueBegin.SuccessorOnFinished = next);
      ExpandFront(foreachEnd);
    }

    public override void VisitConstantPattern(ConstantPatternSyntax node) {
      Visit(node.Expression);
      ExpandFront(new OperatorBlock(node.Parent.GetLocation(), SyntaxKind.EqualsExpression, null));
    }

    public override void VisitForEachVariableStatement(ForEachVariableStatementSyntax node) {
      // TODO: Support foreach variable, currently skipped (imprecise)
    }

    public override void VisitIsPatternExpression(IsPatternExpressionSyntax node) {
      Visit(node.Expression);
      Visit(node.Pattern);
    }

    public override void VisitDeclarationPattern(DeclarationPatternSyntax node) {
      ExpandFront(new DuplicateBlock(node.GetLocation()));
      Visit(node.Type);
      ExpandFront(new OperatorBlock(node.GetLocation(), SyntaxKind.IsExpression, null));
      var branch = new BranchBlock(node.GetLocation());
      SucceedFront(branch);
      NewFront(next => branch.SuccessorOnTrue = next);
      var variable = _compilationModel.GetDeclaredSymbol(node.Designation);
      VisitVariableDesignator(variable, node.GetLocation());
      ExpandFront(new WriteBlock(node.GetLocation()));
      ExpandFront(new ConstantBlock(node.GetLocation(), true));
      var afterIf = _front;
      NewFront(next => branch.SuccessorOnFalse = next);
      ExpandFront(new DiscardBlock(node.GetLocation()));
      ExpandFront(new ConstantBlock(node.GetLocation(), false));
      NewFront(_front.Union(afterIf));
    }

    public override void VisitUnaryPattern(UnaryPatternSyntax node) {
      Visit(node.Pattern);
      if (node.IsKind(SyntaxKind.NotPattern)) {
        ExpandFront(new OperatorBlock(node.GetLocation(), SyntaxKind.LogicalNotExpression, null));
      } else {
        throw new NotImplementedException();
      }
    }

    public override void VisitBinaryPattern(BinaryPatternSyntax node) {
      if (node.IsKind(SyntaxKind.AndPattern)) {
        VisitAndPattern(node);
      } else if (node.IsKind(SyntaxKind.OrPattern)) {
        VisitOrPattern(node);
      } else {
        throw new NotImplementedException();
      }
    }

    private void VisitAndPattern(BinaryPatternSyntax node) {
      ExpandFront(new DuplicateBlock(node.GetLocation()));
      Visit(node.Left);
      var branch = new BranchBlock(node.GetLocation());
      SucceedFront(branch);
      NewFront(next => branch.SuccessorOnTrue = next);
      Visit(node.Right);
      var afterIf = _front;
      NewFront(next => branch.SuccessorOnFalse = next);
      ExpandFront(new DiscardBlock(node.GetLocation()));
      ExpandFront(new ConstantBlock(node.GetLocation(), false));
      NewFront(_front.Union(afterIf));
    }

    private void VisitOrPattern(BinaryPatternSyntax node) {
      ExpandFront(new DuplicateBlock(node.GetLocation()));
      Visit(node.Left);
      var branch = new BranchBlock(node.GetLocation());
      SucceedFront(branch);
      NewFront(next => branch.SuccessorOnFalse = next);
      Visit(node.Right);
      var afterIf = _front;
      NewFront(next => branch.SuccessorOnTrue = next);
      ExpandFront(new DiscardBlock(node.GetLocation()));
      ExpandFront(new ConstantBlock(node.GetLocation(), true));
      NewFront(_front.Union(afterIf));
    }

    public override void VisitTypePattern(TypePatternSyntax node) {
      Visit(node.Type);
      ExpandFront(new OperatorBlock(node.GetLocation(), SyntaxKind.IsExpression, null));
    }

    private readonly IDictionary<SyntaxKind, SyntaxKind> _relationalOperatorMap = new Dictionary<SyntaxKind, SyntaxKind> {
      { SyntaxKind.GreaterThanEqualsToken, SyntaxKind.GreaterThanOrEqualExpression },
      { SyntaxKind.GreaterThanToken, SyntaxKind.GreaterThanExpression },
      { SyntaxKind.LessThanEqualsToken, SyntaxKind.LessThanOrEqualExpression },
      { SyntaxKind.LessThanToken, SyntaxKind.LessThanExpression },
      { SyntaxKind.EqualsEqualsToken, SyntaxKind.EqualsExpression },
      { SyntaxKind.ExclamationEqualsToken, SyntaxKind.NotEqualsExpression }
    };

    public override void VisitRelationalPattern(RelationalPatternSyntax node) {
      if (_relationalOperatorMap.TryGetValue(node.OperatorToken.Kind(), out var kind)) {
        Visit(node.Expression);
        ExpandFront(new OperatorBlock(node.GetLocation(), kind, null));
      } else {
        throw new NotImplementedException();
      }
    }

    public override void VisitParenthesizedPattern(ParenthesizedPatternSyntax node) {
      Visit(node.Pattern);
    }

    private readonly ISet<SyntaxKind> _regularBinaryOperators = new HashSet<SyntaxKind> {
      SyntaxKind.GreaterThanOrEqualExpression,
      SyntaxKind.GreaterThanExpression,
      SyntaxKind.LessThanOrEqualExpression,
      SyntaxKind.LessThanExpression,
      SyntaxKind.EqualsExpression,
      SyntaxKind.NotEqualsExpression,
      SyntaxKind.AddExpression,
      SyntaxKind.SubtractExpression,
      SyntaxKind.MultiplyExpression,
      SyntaxKind.DivideExpression,
      SyntaxKind.ModuloExpression,
      SyntaxKind.ExclusiveOrExpression,
      SyntaxKind.BitwiseOrExpression,
      SyntaxKind.BitwiseAndExpression,
      SyntaxKind.LeftShiftExpression,
      SyntaxKind.RightShiftExpression,
      SyntaxKind.IsExpression,
      SyntaxKind.AsExpression
    };

    public override void VisitBinaryExpression(BinaryExpressionSyntax node) {
      var kind = node.Kind();
      if (_regularBinaryOperators.Contains(kind)) {
        Visit(node.Left);
        Visit(node.Right);
        var method = (IMethodSymbol)_compilationModel.GetReferencedSymbol(node);
        ExpandFront(new OperatorBlock(node.GetLocation(), kind, method));
        ImplicitCast(node);
      } else if (kind == SyntaxKind.LogicalAndExpression) {
        VisitLogicalAnd(node);
      } else if (kind == SyntaxKind.LogicalOrExpression) {
        VisitLogicalOr(node);
      } else if (kind == SyntaxKind.CoalesceExpression) {
        VisitNullCoalesce(node);
      } else {
        throw new NotImplementedException();
      }
    }

    private void VisitNullCoalesce(BinaryExpressionSyntax node) {
      Visit(node.Left);
      ExpandFront(new DuplicateBlock(node.GetLocation()));
      ExpandFront(new ConstantBlock(node.GetLocation(), null));
      ExpandFront(new OperatorBlock(node.GetLocation(), SyntaxKind.EqualsExpression, null));
      var branch = new BranchBlock(node.GetLocation());
      SucceedFront(branch);
      NewFront(next => branch.SuccessorOnTrue = next);
      ExpandFront(new DiscardBlock(node.GetLocation()));
      Visit(node.Right);
      var afterIf = _front;
      NewFront(next => branch.SuccessorOnFalse = next);
      NewFront(_front.Union(afterIf));
    }

    private void VisitLogicalAnd(BinaryExpressionSyntax node) {
      Visit(node.Left);
      var branch = new BranchBlock(node.GetLocation());
      SucceedFront(branch);
      NewFront(next => branch.SuccessorOnTrue = next);
      Visit(node.Right);
      var afterIf = _front;
      NewFront(next => branch.SuccessorOnFalse = next);
      var constant = new ConstantBlock(node.GetLocation(), false);
      ExpandFront(constant);
      NewFront(_front.Union(afterIf));
    }

    private void VisitLogicalOr(BinaryExpressionSyntax node) {
      Visit(node.Left);
      var branch = new BranchBlock(node.GetLocation());
      SucceedFront(branch);
      NewFront(next => branch.SuccessorOnFalse = next);
      Visit(node.Right);
      var afterIf = _front;
      NewFront(next => branch.SuccessorOnTrue = next);
      var constant = new ConstantBlock(node.GetLocation(), true);
      ExpandFront(constant);
      NewFront(_front.Union(afterIf));
    }

    public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node) {
      // TODO: Check for other operators than ++ and --, and ! null warning suppress
      var kind = node.Kind();
      switch (kind) {
        case SyntaxKind.PostIncrementExpression:
        case SyntaxKind.PostDecrementExpression:
          var method = (IMethodSymbol)_compilationModel.GetReferencedSymbol(node);
          SyntaxKind operationKind;
          if (kind == SyntaxKind.PostIncrementExpression) {
            operationKind = SyntaxKind.AddExpression;
          } else if (kind == SyntaxKind.PostDecrementExpression) {
            operationKind = SyntaxKind.SubtractExpression;
          } else {
            throw new NotImplementedException();
          }
          Visit(node.Operand);
          var duplicate = new DuplicateBlock(node.GetLocation());
          ExpandFront(duplicate);
          var constant = new ConstantBlock(node.GetLocation(), 1);
          ExpandFront(constant);
          var operation = new OperatorBlock(node.GetLocation(), operationKind, method);
          ExpandFront(operation);
          VisitWriteBlock(node.Operand, node.GetLocation());
          break;
        case SyntaxKind.SuppressNullableWarningExpression:
          Visit(node.Operand);
          break;
        default:
          throw new NotImplementedException();
      }
      ImplicitCast(node);
    }

    private readonly ISet<SyntaxKind> _regularUnaryOperators = new HashSet<SyntaxKind> {
      SyntaxKind.UnaryMinusExpression,
      SyntaxKind.UnaryPlusExpression,
      SyntaxKind.LogicalNotExpression,
      SyntaxKind.BitwiseNotExpression,
      SyntaxKind.IndexExpression
    };

    // TODO: Support unsafe programming
    private readonly ISet<SyntaxKind> _unsupportedUnaryOperators = new HashSet<SyntaxKind> {
      SyntaxKind.PointerIndirectionExpression
    };

    public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node) {
      var kind = node.Kind();
      var method = (IMethodSymbol)_compilationModel.GetReferencedSymbol(node);
      if (_regularUnaryOperators.Contains(kind)) {
        Visit(node.Operand);
        var block = new OperatorBlock(node.GetLocation(), kind, method);
        ExpandFront(block);
      } else if (kind == SyntaxKind.PreIncrementExpression ||
          kind == SyntaxKind.PreDecrementExpression) {
        SyntaxKind operationKind;
        if (kind == SyntaxKind.PreIncrementExpression) {
          operationKind = SyntaxKind.AddExpression;
        } else if (kind == SyntaxKind.PreDecrementExpression) {
          operationKind = SyntaxKind.SubtractExpression;
        } else {
          throw new NotImplementedException();
        }
        Visit(node.Operand);
        var constant = new ConstantBlock(node.GetLocation(), 1);
        ExpandFront(constant);
        var operation = new OperatorBlock(node.GetLocation(), operationKind, method);
        ExpandFront(operation);
        var duplicate = new DuplicateBlock(node.GetLocation());
        ExpandFront(duplicate);
        VisitWriteBlock(node.Operand, node.GetLocation());
      } else if (kind == SyntaxKind.AddressOfExpression) {
        ExpandFront(new UnknownBlock(node.GetLocation()));
        VisitDesignator(node.Operand, node.GetLocation());
        ExpandFront(new WriteBlock(node.GetLocation()));
        ExpandFront(new UnknownBlock(node.GetLocation()));
      } else if (_unsupportedUnaryOperators.Contains(kind)) {
        Visit(node.Operand);
        ExpandFront(new DiscardBlock(node.GetLocation()));
        ExpandFront(new UnknownBlock(node.GetLocation()));
      } else {
        throw new NotImplementedException();
      }
      ImplicitCast(node);
    }

    public override void VisitLockStatement(LockStatementSyntax node) {
      Visit(node.Expression);
      var lockBlock = new LockBlock(node.GetLocation());
      _openScopes.Push(new LockScope(lockBlock));
      ExpandFront(lockBlock);
      Visit(node.Statement);
      _openScopes.Pop();
      var unlockBlock = new UnlockBlock(node.GetLocation());
      ExpandFront(unlockBlock);
    }

    public override void VisitReturnStatement(ReturnStatementSyntax node) {
      if (node.Expression != null) {
        Visit(node.Expression);
        CopyStruct(node.Expression);
      }
      ExitScopesUntil(node.GetLocation(), null);
      SucceedFront(Graph.Exit);
      NewFront();
    }

    public override void VisitEmptyStatement(EmptyStatementSyntax node) {
    }

    public override void VisitThisExpression(ThisExpressionSyntax node) {
      var block = new ThisBlock(node.GetLocation());
      ExpandFront(block);
    }

    public override void VisitInitializerExpression(InitializerExpressionSyntax node) {
      if (node.IsKind(SyntaxKind.ArrayInitializerExpression)) {
        VisitArrayInitializer(node);
      } else if (node.IsKind(SyntaxKind.ObjectInitializerExpression) || node.IsKind(SyntaxKind.WithInitializerExpression)) {
        VisitObjectInitializer(node);
      } else if (node.IsKind(SyntaxKind.CollectionInitializerExpression)) {
        VisitCollectionInitializer(node);
      } else if (node.IsKind(SyntaxKind.ComplexElementInitializerExpression)) {
        VisitComplexElementInitializer(node);
      } else {
        throw new NotImplementedException();
      }
      ImplicitCast(node);
    }

    private void VisitComplexElementInitializer(InitializerExpressionSyntax node) {
      foreach (var expression in node.Expressions) {
        Visit(expression);
      }
    }

    private void VisitCollectionInitializer(InitializerExpressionSyntax node) {
      foreach (var expression in node.Expressions) {
        Visit(expression);
      }
      var count = node.Expressions.Count;
      var type = _compilationModel.GetNodeType(node.Parent);
      var sizes =
        (from expression in node.Expressions
         select TupleLength(expression)).ToArray();
      var addAll = node.Parent is ObjectCreationExpressionSyntax or ImplicitObjectCreationExpressionSyntax;
      ExpandFront(new CollectionInitializerBlock(node.GetLocation(), sizes, addAll, type));
    }

    private static int TupleLength(ExpressionSyntax expression) {
      if (expression is InitializerExpressionSyntax initializer) {
        return initializer.Expressions.Count;
      } else {
        return 1;
      }
    }

    public override void VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node) {
      Visit(node.Initializer);
    }

    private void VisitArrayInitializer(InitializerExpressionSyntax node) {
      var arrayType = _compilationModel.GetNodeType(node) as IArrayTypeSymbol;
      var elementType = arrayType?.ElementType;
      if (elementType == null && node.Expressions.Count > 0) {
        elementType = _compilationModel.GetNodeType(node.Expressions[0]);
      }
      int rank = arrayType != null ? arrayType.Rank : 1;
      foreach (var expression in node.Expressions) {
        Visit(expression);
      }
      var count = node.Expressions.Count;
      ExpandFront(new ArrayInitializerBlock(node.GetLocation(), elementType, rank, count));
    }

    private void VisitObjectInitializer(InitializerExpressionSyntax node) {
      if (node.Parent is AssignmentExpressionSyntax parent) {
        var type = (INamedTypeSymbol)_compilationModel.GetNodeType(parent.Left);
        ExpandFront(new ObjectCreationBlock(node.GetLocation(), type, type.GetDefaultConstructor(), 0));
        ImplicitCast(node);
      }
      foreach (var expression in node.Expressions) {
        ExpandFront(new DuplicateBlock(expression.GetLocation()));
        var assignment = (AssignmentExpressionSyntax)expression;
        Visit(assignment.Right);
        ExpandFront(new SwapBlock(expression.GetLocation()));
        var symbol = _compilationModel.GetReferencedSymbol(assignment.Left);
        if (symbol == null) {
          ExpandFront(new DiscardBlock(node.GetLocation()));
          ExpandFront(new DiscardBlock(node.GetLocation()));
        } else if (symbol is IPropertySymbol property) {
          if (assignment.Left is ImplicitElementAccessSyntax elemAccess) {
            // This situation occurs when using the assignment based initialization of collections instead
            // of the collection initializer syntax.
            Visit(elemAccess.ArgumentList);
          }
          ExpandFront(new PropertySetBlock(expression.GetLocation(), property));
        } else if (symbol is IFieldSymbol) {
          ExpandFront(new VariableSelectionBlock(assignment.Left.GetLocation(), symbol));
          ExpandFront(new WriteBlock(expression.GetLocation()));
        } else {
          throw new NotImplementedException();
        }
      }
    }

    public override void VisitArrayCreationExpression(ArrayCreationExpressionSyntax node) {
      VisitArrayCreation(node.Type, node.Initializer, node.GetLocation());
      ImplicitCast(node);
    }

    private void VisitFixedArrayCreation(ITypeSymbol type, SeparatedSyntaxList<ArgumentSyntax> sizes, Location location) {
      if (type is IPointerTypeSymbol) {
        foreach (var size in sizes) {
          Visit(size);
          type = (type as IPointerTypeSymbol)?.PointedAtType;
        }
        CreateArray(location, 1, type, sizes.Count);
      } else {
        ExpandFront(new UnknownBlock(location));
      }
    }

    private void VisitArrayCreation(ArrayTypeSyntax type, InitializerExpressionSyntax initializer, Location location) {
      if (initializer != null) {
        Visit(initializer);
      } else {
        var ranks = type.RankSpecifiers;
        foreach (var rank in ranks) {
          Visit(rank);
        }
        var elementType = _compilationModel.GetReferencedSymbol(type.ElementType) as ITypeSymbol;
        var lengths = ranks[0].Sizes.Count;
        CreateArray(location, ranks.Count, elementType, lengths);
      }
    }

    private void CreateArray(Location location, int ranks, ITypeSymbol elementType, int lengths) {
      if (elementType != null) {
        ExpandFront(new ArrayCreationBlock(location, elementType, ranks, lengths));
      } else {
        for (int index = 0; index < lengths; index++) {
          ExpandFront(new DiscardBlock(location));
        }
        ExpandFront(new UnknownBlock(location));
      }
    }

    public override void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node) {
      foreach (var size in node.Sizes) {
        Visit(size);
      }
    }

    public override void VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node) {
      ImplicitCast(node);
    }

    public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node) {
      Visit(node.Expression);
      Visit(node.ArgumentList);
      var element = _compilationModel.GetReferencedSymbol(node);
      if (element is IPropertySymbol property) {
        ExpandFront(new PropertyGetBlock(node.GetLocation(), property));
      } else {
        var dimensions = node.ArgumentList.Arguments.Count;
        ExpandFront(new ElementSelectionBlock(node.GetLocation(), dimensions));
        ExpandFront(new ReadBlock(node.GetLocation()));
      }
      ImplicitCast(node);
    }

    public override void VisitBracketedArgumentList(BracketedArgumentListSyntax node) {
      foreach (var argument in node.Arguments) {
        Visit(argument);
      }
    }

    public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node) {
      var member = _compilationModel.GetReferencedSymbol(node);
      if (node.Expression is BaseExpressionSyntax) {
        ExpandFront(new ThisBlock(node.Expression.GetLocation()));
      } else if (member == null || !(member.IsStaticSymbol()) && member is not INamespaceOrTypeSymbol) {
        Visit(node.Expression);
      }
      if (member == null) {
        ExpandFront(new DiscardBlock(node.GetLocation()));
        ExpandFront(new UnknownBlock(node.GetLocation()));
      } else if (IsConstant(member)) {
        if (IsUnknownConstantValue(member)) {
          ExpandFront(new UnknownBlock(node.GetLocation()));
        } else {
          var constant = GetConstantValue(member);
          ExpandFront(new ConstantBlock(node.GetLocation(), constant));
        }
      } else if (IsVariableSymbol(member)) {
        ExpandFront(new VariableSelectionBlock(node.GetLocation(), member));
        ExpandFront(new ReadBlock(node.GetLocation()));
      } else if (member is IPropertySymbol property) {
        if (property.Parameters.Length == 0) {
          ExpandFront(new PropertyGetBlock(node.GetLocation(), property));
        } // otherwise outer ElementAccessExpression
      } else if (member is IMethodSymbol) {
        // delegate
        ExpandFront(new ConstantBlock(node.GetLocation(), member));
      } else if (member is INamespaceOrTypeSymbol) {
        ExpandFront(new ConstantBlock(node.GetLocation(), member));
      } else {
        throw new NotImplementedException();
      }
      ImplicitCast(node);
    }

    public override void VisitCastExpression(CastExpressionSyntax node) {
      Visit(node.Expression);
      if (_compilationModel.GetReferencedSymbol(node.Type) is ITypeSymbol type) {
        CastType(node, type);
      }
    }

    public override void VisitConstructorInitializer(ConstructorInitializerSyntax node) {
      var constructor = (IMethodSymbol)_compilationModel.GetReferencedSymbol(node);
      if (constructor == null) {
        IgnoreUnknownInitializer(node);
      } else {
        ExpandFront(new ThisBlock(node.GetLocation()));
        VisitArgumentList(node.ArgumentList, constructor.Parameters);
        ExpandFront(new InvocationBlock(node.GetLocation(), constructor, false));
      }
    }

    private void IgnoreUnknownInitializer(ConstructorInitializerSyntax node) {
      foreach (var argument in node.ArgumentList.Arguments) {
        Visit(argument);
        ExpandFront(new DiscardBlock(argument.GetLocation()));
      }
      InitializeUnknownBaseMembers(node);
    }

    private void InitializeUnknownBaseMembers(ConstructorInitializerSyntax node) {
      var type = (ITypeSymbol)_compilationModel.GetDeclaredSymbol(node.Parent.Parent);
      foreach (var member in type.BaseType.GetInstanceFields()) {
        ExpandFront(new UnknownBlock(node.GetLocation()));
        ExpandFront(new ThisBlock(node.GetLocation()));
        ExpandFront(new VariableSelectionBlock(node.GetLocation(), member));
        ExpandFront(new WriteBlock(node.GetLocation()));
      }
    }

    public override void VisitAwaitExpression(AwaitExpressionSyntax node) {
      Visit(node.Expression);
      ExpandFront(new AwaitBlock(node.GetLocation()));
    }

    public override void VisitLabeledStatement(LabeledStatementSyntax node) {
      var labelBlock = GetGotoLabel(node.Identifier);
      ExpandFront(labelBlock);
      Visit(node.Statement);
    }

    public override void VisitGotoStatement(GotoStatementSyntax node) {
      BasicBlock target;
      if (node.Expression is IdentifierNameSyntax identifierName) {
        var identifier = identifierName.Identifier;
        target = GetGotoLabel(identifier);
      } else if (node.CaseOrDefaultKeyword.IsKind(SyntaxKind.CaseKeyword)) {
        var value = _compilationModel.ConstantValue(node.Expression);
        target = GetGotoCaseEntry(value, node.GetLocation());
      } else if (node.CaseOrDefaultKeyword.IsKind(SyntaxKind.DefaultKeyword)) {
        target = GetGotoDefaultEntry(node.GetLocation());
      } else {
        throw new NotImplementedException();
      }
      if (target != null) {
        _unresolvedBranches.Add(new Branch(_front, target));
      }
      NewFront();
    }

    private EmptyBlock GetGotoLabel(SyntaxToken identifier) {
      var key = identifier.Text;
      if (!_gotoLabels.ContainsKey(key)) {
        _gotoLabels.Add(key, new EmptyBlock(identifier.GetLocation()));
      }
      return _gotoLabels[key];
    }

    private StraightBlock GetGotoCaseEntry(object value, Location location) {
      if (value == null) {
        return null;
      }
      var switchScope = GetInnermostScope<SwitchScope>();
      if (!switchScope.CaseLabels.ContainsKey(value)) {
        switchScope.CaseLabels.Add(value, new EmptyBlock(location));
      }
      return switchScope.CaseLabels[value];
    }

    private StraightBlock GetGotoDefaultEntry(Location location) {
      var switchScope = GetInnermostScope<SwitchScope>();
      switchScope.DefaultLabel ??= new EmptyBlock(location);
      return switchScope.DefaultLabel;
    }

    public override void VisitThrowStatement(ThrowStatementSyntax node) {
      VisitThrow(node.Expression, node.GetLocation());
    }

    public override void VisitThrowExpression(ThrowExpressionSyntax node) {
      VisitThrow(node.Expression, node.GetLocation());
    }

    private void VisitThrow(ExpressionSyntax expression, Location location) {
      if (expression != null) {
        Visit(expression);
      }
      SucceedFront(new ThrowBlock(location, expression == null));
      NewFront();
    }

    public override void VisitTryStatement(TryStatementSyntax node) {
      _openScopes.Push(new TryScope());
      var entry = new EnterTryBlock(node.GetLocation());
      ExpandFront(entry);
      Visit(node.Block);
      ExpandFront(new ExitTryBlock(node.GetLocation()));
      VisitCatches(node, entry);
      VisitFinally(node, entry);
      _openScopes.Pop();
    }

    private void VisitCatches(TryStatementSyntax node, EnterTryBlock block) {
      void CatchVisitor(ref ISet<Connection> afterCatch) {
        foreach (var catchClause in node.Catches) {
          VisitCatchCondition(catchClause);
          var branch = new BranchBlock(catchClause.GetLocation());
          SucceedFront(branch);
          NewFront(next => branch.SuccessorOnTrue = next);
          VisitCatchDeclaration(catchClause);
          Visit(catchClause.Block);
          ExpandFront(new CatchBlock(catchClause.GetLocation()));
          ExpandFront(new ExitTryBlock(node.GetLocation()));
          afterCatch = new HashSet<Connection>(afterCatch.Union(_front));
          NewFront(next => branch.SuccessorOnFalse = next);
        }
      };
      AddCatches(node.GetLocation(), block, CatchVisitor);
    }

    private delegate void CatchVisitor(ref ISet<Connection> afterCatch);

    private void AddCatches(Location location, EnterTryBlock entry, CatchVisitor catchVisitor) {
      var afterCatch = _front;
      entry.Catches = new EmptyBlock(location);
      NewFront(entry.Catches);
      catchVisitor?.Invoke(ref afterCatch);
      ExpandFront(new DiscardBlock(location));
      ExpandFront(new ExitTryBlock(location));
      NewFront(afterCatch.Union(_front));
    }

    private void VisitCatchDeclaration(CatchClauseSyntax node) {
      var declaration = node.Declaration;
      ISymbol symbol = null;
      if (declaration != null) {
        symbol = _compilationModel.GetDeclaredSymbol(node.Declaration);
      }
      if (symbol != null) {
        ExpandFront(new VariableSelectionBlock(declaration.GetLocation(), symbol));
        ExpandFront(new WriteBlock(declaration.GetLocation()));
      } else if (declaration != null) {
        ExpandFront(new DiscardBlock(declaration.GetLocation()));
      } else {
        ExpandFront(new DiscardBlock(node.GetLocation()));
      }
    }

    private void VisitCatchCondition(CatchClauseSyntax node) {
      if (node.Declaration != null) {
        VisitDeclaredCatchClause(node);
      } else {
        VisitGeneralCatchClause(node);
      }
    }

    private void VisitGeneralCatchClause(CatchClauseSyntax node) {
      ExpandFront(new ConstantBlock(node.GetLocation(), true));
    }

    private void VisitDeclaredCatchClause(CatchClauseSyntax node) {
      ExpandFront(new DuplicateBlock(node.GetLocation()));
      var type = _compilationModel.GetReferencedSymbol(node.Declaration.Type);
      if (type == null) {
        ExpandFront(new UnknownBlock(node.GetLocation()));
      } else {
        ExpandFront(new ConstantBlock(node.GetLocation(), type));
      }
      ExpandFront(new OperatorBlock(node.GetLocation(), SyntaxKind.IsExpression, null));
      if (node.Filter != null) {
        var branch = new BranchBlock(node.GetLocation());
        SucceedFront(branch);
        NewFront(next => branch.SuccessorOnTrue = next);
        ExpandFront(new DuplicateBlock(node.GetLocation()));
        VisitCatchDeclaration(node);
        Visit(node.Filter.FilterExpression);
        var afterIf = _front;
        NewFront(next => branch.SuccessorOnFalse = next);
        ExpandFront(new ConstantBlock(node.GetLocation(), false));
        NewFront(afterIf.Union(_front));
      }
    }

    private void VisitFinally(TryStatementSyntax node, EnterTryBlock entry) {
      void finallyBlockVisitor() {
        if (node.Finally != null) {
          Visit(node.Finally.Block);
        }
      }
      AddFinallyBlock(node.GetLocation(), entry, finallyBlockVisitor);
    }

    private void AddFinallyBlock(Location location, EnterTryBlock entry, Action finallyBlockVisitor) {
      var afterFinally = _front;
      entry.Finally = new EmptyBlock(location);
      NewFront(entry.Finally);
      finallyBlockVisitor?.Invoke();
      ExpandFront(new ExitTryBlock(location));
      NewFront(afterFinally.Union(_front));
    }

    public override void VisitDefaultExpression(DefaultExpressionSyntax node) {
      var type = (ITypeSymbol)_compilationModel.GetReferencedSymbol(node.Type);
      GenerateDefaultValue(node.GetLocation(), type);
    }

    private void GenerateDefaultValue(Location location, ITypeSymbol type) {
      if (type == null || type.IsTypeParameter()) {
        // TODO: Would need to resolve type arguments per caller or instantiation
        ExpandFront(new UnknownBlock(location));
      } else if (type.IsStruct()) {
        ExpandFront(new ObjectCreationBlock(location, type, null, 0));
      } else {
        var value = type.GetDefaultValue();
        ExpandFront(new ConstantBlock(location, value));
      }
    }

    public override void VisitTypeOfExpression(TypeOfExpressionSyntax node) {
      ExpandFront(new UnknownBlock(node.GetLocation()));
    }

    public override void VisitSizeOfExpression(SizeOfExpressionSyntax node) {
      var type = (ITypeSymbol)_compilationModel.GetReferencedSymbol(node.Type);
      if (type == null || type.IsStruct() || type.IsReferenceType) {
        // TODO: Derive size of these kinds of types
        ExpandFront(new UnknownBlock(node.GetLocation()));
      } else {
        ExpandFront(new ConstantBlock(node.GetLocation(), type.GetSize()));
      }
    }

    public override void VisitUsingStatement(UsingStatementSyntax node) {
      if (node.Expression != null) {
        Visit(node.Expression);
      } else {
        Visit(node.Declaration);
      }
      var disposeExpander = PrepareUsingDisposes(node.Declaration, node.Expression);
      _openScopes.Push(new TryScope());
      var entry = new EnterTryBlock(node.GetLocation());
      ExpandFront(entry);
      Visit(node.Statement);
      ExpandFront(new ExitTryBlock(node.GetLocation()));

      AddCatches(node.GetLocation(), entry, null);

      void finallyBlockVisitor() {
        while (disposeExpander.Count > 0) {
          disposeExpander.Pop()();
        }
      }
      AddFinallyBlock(node.GetLocation(), entry, finallyBlockVisitor);
      _openScopes.Pop();
    }

    private Stack<Action> PrepareUsingDisposes(VariableDeclarationSyntax declaration, ExpressionSyntax expression) {
      var disposeExpander = new Stack<Action>();
      if (expression != null) {
        var type = _compilationModel.GetNodeType(expression);
        if (type != null && type.IsDisposable() && type.TryGetDisposeMethod(out var disposeMethod)) {
          var isVirtual = disposeMethod.IsVirtual || disposeMethod.IsAbstract;
          disposeExpander.Push(CreateNullGuardedDisposeExpander(expression.GetLocation(), disposeMethod));
        } else {
          // TODO: Find an alternative to discarding the disposable object
          ExpandFront(new DiscardBlock(expression.GetLocation()));
        }
      } else {
        // TODO: Also handle variables that are not identified as IDisposable
        foreach (var variable in CollectDeclaredDisposableVariables(declaration)) {
          if (variable.GetVariableType().TryGetDisposeMethod(out var disposeMethod)) {
            ExpandFront(new VariableSelectionBlock(declaration.GetLocation(), variable));
            ExpandFront(new ReadBlock(declaration.GetLocation()));
            disposeExpander.Push(CreateNullGuardedDisposeExpander(declaration.GetLocation(), disposeMethod));
          }
        }
      }
      return disposeExpander;
    }

    private Action CreateNullGuardedDisposeExpander(Location location, IMethodSymbol disposeMethod) {
      return () => {
        ExpandFront(new DuplicateBlock(location));
        ExpandFront(new ConstantBlock(location, null));
        ExpandFront(new OperatorBlock(location, SyntaxKind.EqualsExpression, null));
        var branch = new BranchBlock(location);
        SucceedFront(branch);

        NewFront(next => branch.SuccessorOnTrue = next);
        ExpandFront(new DiscardBlock(location));

        var afterIf = _front;
        NewFront(next => branch.SuccessorOnFalse = next);
        var isVirtual = disposeMethod.IsVirtual || disposeMethod.IsAbstract;
        ExpandFront(new InvocationBlock(location, disposeMethod, isVirtual));
        NewFront(_front.Union(afterIf));
      };
    }

    private IEnumerable<ISymbol> CollectDeclaredDisposableVariables(VariableDeclarationSyntax declaration) {
      return
        from variable in declaration.Variables
        let symbol = _compilationModel.GetDeclaredSymbol(variable)
        where symbol != null
        where symbol.GetVariableType().IsDisposable()
        select symbol;
    }

    public override void VisitFixedStatement(FixedStatementSyntax node) {
      Visit(node.Declaration);
      Visit(node.Statement);
    }

    public override void VisitGenericName(GenericNameSyntax node) {
      VisitConstantSymbol(node);
    }

    public override void VisitArrayType(ArrayTypeSyntax node) {
      VisitConstantSymbol(node);
    }

    private void VisitConstantSymbol(SyntaxNode node) {
      var symbol = _compilationModel.GetReferencedSymbol(node);
      if (symbol == null) {
        ExpandFront(new UnknownBlock(node.GetLocation()));
      } else if (symbol is ITypeSymbol || symbol is IMethodSymbol) {
        if (symbol is IMethodSymbol method && !method.IsStaticSymbol()) {
          ExpandFront(new ThisBlock(node.GetLocation()));
        }
        ExpandFront(new ConstantBlock(node.GetLocation(), symbol));
      } else {
        throw new NotImplementedException();
      }
    }

    public override void VisitPredefinedType(PredefinedTypeSyntax node) {
      var symbol = _compilationModel.GetReferencedSymbol(node);
      ExpandFront(new ConstantBlock(node.GetLocation(), symbol));
    }

    public override void VisitQueryExpression(QueryExpressionSyntax node) {
      Visit(node.FromClause.Expression);
      var parameter = GetLinqParameter(node);
      ExpandFront(new LinqBlock(node.FromClause.GetLocation(), LinqKind.From, parameter, null));
      foreach (var clause in node.Body.Clauses) {
        Visit(clause);
      }
      Visit(node.Body.SelectOrGroup);
    }

    public override void VisitLetClause(LetClauseSyntax node) {
      // TODO: Support let LINQ clause
      ExpandFront(new LinqBlock(node.GetLocation(), LinqKind.Unknown, null, null));
    }

    public override void VisitJoinClause(JoinClauseSyntax node) {
      // TODO: Support join LINQ clause
      ExpandFront(new LinqBlock(node.GetLocation(), LinqKind.Unknown, null, null));
    }

    public override void VisitJoinIntoClause(JoinIntoClauseSyntax node) {
      // TODO: Support join to LINQ clause
      ExpandFront(new LinqBlock(node.GetLocation(), LinqKind.Unknown, null, null));
    }

    public override void VisitFromClause(FromClauseSyntax node) {
      // TODO: Support subsequent from LINQ clause (SelectMany)
      ExpandFront(new LinqBlock(node.GetLocation(), LinqKind.Unknown, null, null));
    }

    public override void VisitOrderByClause(OrderByClauseSyntax node) {
      // TODO: Support orderby LINQ clause
      ExpandFront(new LinqBlock(node.GetLocation(), LinqKind.Unknown, null, null));
    }

    public override void VisitGroupClause(GroupClauseSyntax node) {
      // TODO: Support group LINQ clause
      ExpandFront(new LinqBlock(node.GetLocation(), LinqKind.Unknown, null, null));
    }

    public override void VisitWhereClause(WhereClauseSyntax node) {
      var parameter = GetLinqParameter(node);
      ExpandFront(new LinqBlock(node.GetLocation(), LinqKind.Where, parameter, node.Condition));
    }

    public override void VisitSelectClause(SelectClauseSyntax node) {
      var parameter = GetLinqParameter(node);
      ExpandFront(new LinqBlock(node.GetLocation(), LinqKind.Select, parameter, node.Expression));
    }

    private ISymbol GetLinqParameter(SyntaxNode node) {
      while (node != null && node is not QueryExpressionSyntax) {
        node = node.Parent;
      }
      if (node == null) {
        return null;
      }
      var fromClause = ((QueryExpressionSyntax)node).FromClause;
      return _compilationModel.GetDeclaredSymbol(fromClause);
    }

    public override void VisitYieldStatement(YieldStatementSyntax node) {
      // TODO: Implement yield for iterators
      ExpandFront(new UnknownBlock(node.GetLocation()));
      ExitScopesUntil(node.GetLocation(), null);
      SucceedFront(Graph.Exit);
      NewFront();
    }

    private void ImplicitYieldReturn(Location location, IMethodSymbol method) {
      if (method.ReturnsEnumerator()) {
        // TODO: Support iterators with yield
        ExpandFront(new UnknownBlock(location));
      }
    }

    public override void VisitCheckedExpression(CheckedExpressionSyntax node) {
      Visit(node.Expression);
    }

    public override void VisitCheckedStatement(CheckedStatementSyntax node) {
      Visit(node.Block);
    }

    public override void VisitBaseExpression(BaseExpressionSyntax node) {
      ExpandFront(new ThisBlock(node.GetLocation()));
    }

    public override void VisitNullableType(NullableTypeSyntax node) {
      var symbol = _compilationModel.GetReferencedSymbol(node);
      ExpandFront(new ConstantBlock(node.GetLocation(), symbol));
    }

    public override void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node) {
      if (node.Contents.Count == 0) {
        ExpandFront(new ConstantBlock(node.GetLocation(), string.Empty));
      }
      for (int index = 0; index < node.Contents.Count; index++) {
        Visit(node.Contents[index]);
        if (index > 0) {
          ExpandFront(new OperatorBlock(node.GetLocation(), SyntaxKind.AddExpression, null));
        }
      }
    }

    public override void VisitInterpolatedStringText(InterpolatedStringTextSyntax node) {
      ExpandFront(new ConstantBlock(node.GetLocation(), node.TextToken.Text));
    }

    public override void VisitInterpolation(InterpolationSyntax node) {
      Visit(node.Expression);
    }

    public override void VisitUnsafeStatement(UnsafeStatementSyntax node) {
      Visit(node.Block);
    }

    public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node) {
      VisitArrayCreation((ArrayTypeSyntax)node.Type as ArrayTypeSyntax, node.Initializer, node.GetLocation());
      ImplicitCast(node);
    }

    public override void VisitTupleExpression(TupleExpressionSyntax node) {
      foreach (var argument in node.Arguments) {
        Visit(argument);
      }
      var fields = new IFieldSymbol[node.Arguments.Count];
      for (int index = 0; index < fields.Length; index++) {
        fields[index] = _compilationModel.GetDeclaredSymbol(node.Arguments[index]) as IFieldSymbol;
      }
      ExpandFront(new TupleCreationBlock(node.GetLocation(), fields));
    }

    public override void VisitRefExpression(RefExpressionSyntax node) {
      VisitDesignator(node.Expression, node.GetLocation());
    }

    public override void VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node) {
      Visit(node.Initializer);
      ImplicitCast(node);
    }

    public override void VisitRefValueExpression(RefValueExpressionSyntax node) {
      Visit(node.Expression);
      ExpandFront(new DiscardBlock(node.GetLocation()));
      ExpandFront(new UnknownBlock(node.GetLocation()));
    }

    public override void VisitMakeRefExpression(MakeRefExpressionSyntax node) {
      Visit(node.Expression);
      ExpandFront(new DiscardBlock(node.GetLocation()));
      ExpandFront(new UnknownBlock(node.GetLocation()));
    }

    public override void VisitRefTypeExpression(RefTypeExpressionSyntax node) {
      Visit(node.Expression);
      ExpandFront(new DiscardBlock(node.GetLocation()));
      ExpandFront(new UnknownBlock(node.GetLocation()));
    }

    public override void VisitSwitchExpression(SwitchExpressionSyntax node) {
      Visit(node.GoverningExpression);
      IEnumerable<Connection> afterArm = new HashSet<Connection>();
      foreach (var arm in node.Arms) {
        ExpandFront(new DuplicateBlock(node.GetLocation()));
        Visit(arm.Pattern);
        if (arm.WhenClause != null) {
          VisitArmWhenClause(arm.WhenClause);
        }
        var branch = new BranchBlock(node.GetLocation());
        SucceedFront(branch);
        NewFront(next => branch.SuccessorOnTrue = next);
        ExpandFront(new DiscardBlock(node.GetLocation()));
        Visit(arm.Expression);
        afterArm = afterArm.Union(_front);
        NewFront(next => branch.SuccessorOnFalse = next);
      }
      NewFront(afterArm.Union(_front));
    }

    private void VisitArmWhenClause(WhenClauseSyntax whenClause) {
      var branch = new BranchBlock(whenClause.GetLocation());
      SucceedFront(branch);
      NewFront(next => branch.SuccessorOnTrue = next);
      Visit(whenClause);
      var afterIf = _front;
      NewFront(next => branch.SuccessorOnFalse = next);
      ExpandFront(new ConstantBlock(whenClause.GetLocation(), false));
      NewFront(afterIf.Union(_front));
    }

    public override void VisitDiscardPattern(DiscardPatternSyntax node) {
      ExpandFront(new DiscardBlock(node.GetLocation()));
      ExpandFront(new ConstantBlock(node.GetLocation(), true));
    }

    public override void VisitRecursivePattern(RecursivePatternSyntax node) {
      if (node.PropertyPatternClause != null) {
        Visit(node.PropertyPatternClause);
      }
      if (node.PositionalPatternClause != null) {
        Visit(node.PositionalPatternClause);
      }
    }

    public override void VisitPositionalPatternClause(PositionalPatternClauseSyntax node) {
      VisitSubpatternList(node.GetLocation(), node.Subpatterns);
    }

    public override void VisitPropertyPatternClause(PropertyPatternClauseSyntax node) {
      VisitSubpatternList(node.GetLocation(), node.Subpatterns);
    }

    private void VisitSubpatternList(Location location, SeparatedSyntaxList<SubpatternSyntax> subpatterns) {
      for (int index = 0; index < subpatterns.Count; index++) {
        if (index < subpatterns.Count - 1) {
          ExpandFront(new DuplicateBlock(location));
        }
        var subpattern = subpatterns[index];
        if (index == 0) {
          Visit(subpattern);
        } else {
          var branch = new BranchBlock(location);
          SucceedFront(branch);
          NewFront(next => branch.SuccessorOnTrue = next);
          Visit(subpattern);
          var afterIf = _front;
          NewFront(next => branch.SuccessorOnFalse = next);
          ExpandFront(new DiscardBlock(location));
          ExpandFront(new ConstantBlock(location, false));
          NewFront(afterIf.Union(_front));
        }
      }
    }

    public override void VisitSubpattern(SubpatternSyntax node) {
      if (node.NameColon == null) {
        VisitPositionalSubpattern(node);
      } else {
        VisitPropertySubpattern(node);
      }
    }

    private void VisitPositionalSubpattern(SubpatternSyntax node) {
      if (node.Parent is PositionalPatternClauseSyntax positionalPattern) {
        var position = positionalPattern.Subpatterns.IndexOf(node);
        ExpandFront(new ConstantBlock(node.GetLocation(), position));
        ExpandFront(new ElementSelectionBlock(node.GetLocation(), 1));
        ExpandFront(new ReadBlock(node.GetLocation()));
        Visit(node.Pattern);
      } else {
        VisitUnknownPattern(node.GetLocation());
      }
    }

    private void VisitPropertySubpattern(SubpatternSyntax node) {
      var name = node.NameColon.Name;
      var symbol = _compilationModel.GetReferencedSymbol(name);
      if (symbol is IPropertySymbol property) {
        ExpandFront(new PropertyGetBlock(node.GetLocation(), property));
        Visit(node.Pattern);
      } else {
        VisitUnknownPattern(node.GetLocation());
      }
    }

    public override void VisitRangeExpression(RangeExpressionSyntax node) {
      if (node.LeftOperand == null) {
        ExpandFront(new ConstantBlock(node.GetLocation(), null));
      } else {
        Visit(node.LeftOperand);
      }
      if (node.RightOperand == null) {
        ExpandFront(new ConstantBlock(node.GetLocation(), null));
      } else {
        Visit(node.RightOperand);
      }
      if (_compilationModel.GetNodeType(node) is not INamedTypeSymbol type) {
        ExpandFront(new DiscardBlock(node.GetLocation()));
        ExpandFront(new DiscardBlock(node.GetLocation()));
        ExpandFront(new UnknownBlock(node.GetLocation()));
      } else {
        var constructor = type.GetRangeConstructor();
        ExpandFront(new ObjectCreationBlock(node.GetLocation(), type, constructor, 2));
      }
    }

    private void VisitUnknownPattern(Location location) {
      ExpandFront(new DiscardBlock(location));
      ExpandFront(new UnknownBlock(location));
    }

    public override void VisitWithExpression(WithExpressionSyntax node) {
      Visit(node.Expression);
      ExpandFront(new ObjectCloneBlock(node.GetLocation(), true));
      Visit(node.Initializer);
    }

    // TODO: Implement all necessary visit methods
    public override void DefaultVisit(SyntaxNode node) {
      throw new NotImplementedException();
    }

    private void ExpandFront(StraightBlock block) {
      SucceedFront(block);
      NewFront(block);
    }

    private void SucceedFront(BasicBlock next) {
      foreach (var connect in _front) {
        connect(next);
      }
    }

    private void NewFront(StraightBlock block) {
      NewFront(next => block.Successor = next);
    }

    private void NewFront(IEnumerable<Connection> blocks) {
      _front = new HashSet<Connection>(blocks);
    }

    private void NewFront(Connection block) {
      _front = new HashSet<Connection> { block };
    }

    private void NewFront() {
      _front = new HashSet<Connection>();
    }

    private bool IsConstantDeclarator(VariableDeclaratorSyntax node) {
      var symbol = _compilationModel.GetDeclaredSymbol(node);
      return IsConstant(symbol);
    }

    private bool IsDesignator(ExpressionSyntax node) {
      return node switch {
        IdentifierNameSyntax identifier => IsIdentifierDesignator(identifier),
        ElementAccessExpressionSyntax => true,
        MemberAccessExpressionSyntax member => IsMemberDesignator(member),
        ParenthesizedExpressionSyntax parenthesis => IsDesignator(parenthesis.Expression),
        DeclarationExpressionSyntax declaration => IsDeclarationDesignator(declaration),
        InvocationExpressionSyntax invocation => IsInvocationDesignator(invocation),
        ConditionalExpressionSyntax conditional =>
          IsDesignator(conditional.WhenTrue) && IsDesignator(conditional.WhenFalse),
        RefExpressionSyntax refExpression =>
          IsDesignator(refExpression.Expression),
        RefValueExpressionSyntax => true,
        _ => false
        // TODO: Support unary operator designators and struct constructor this assignment
      };
    }

    private bool IsIdentifierDesignator(IdentifierNameSyntax identifier) {
      var symbol = _compilationModel.GetReferencedSymbol(identifier);
      return symbol is IPropertySymbol property && (property.ReturnsByRef || property.ReturnsByRefReadonly) ||
        symbol is not IPropertySymbol;
    }

    private bool IsMemberDesignator(MemberAccessExpressionSyntax member) {
      var symbol = _compilationModel.GetDeclaredSymbol(member);
      return IsVariableSymbol(symbol);
    }

    private bool IsDeclarationDesignator(DeclarationExpressionSyntax declaration) {
      var symbol = _compilationModel.GetDeclaredSymbol(declaration.Designation);
      return IsInstanceVariable(symbol) || IsVariableSymbol(symbol);
    }

    private bool IsInvocationDesignator(InvocationExpressionSyntax invocation) {
      var target = _compilationModel.GetReferencedSymbol(invocation);
      return target is IMethodSymbol callee && (callee.ReturnsByRef || callee.ReturnsByRefReadonly);
    }

    private void VisitDesignator(ExpressionSyntax node, Location location) {
      switch (node) {
        case IdentifierNameSyntax identifier:
          var symbol = _compilationModel.GetReferencedSymbol(identifier);
          if (symbol is IPropertySymbol property) {
            if (!(property.ReturnsByRef || property.ReturnsByRefReadonly)) {
              throw new NotImplementedException();
            }
            Visit(identifier);
          } else {
            VisitVariableDesignator(symbol, location);
          }
          break;
        case ElementAccessExpressionSyntax element:
          VisitElementDesignator(element, location);
          break;
        case MemberAccessExpressionSyntax member:
          VisitMemberDesignator(member, location);
          break;
        case PrefixUnaryExpressionSyntax unaryExpression:
          if (_unsupportedUnaryOperators.Contains(unaryExpression.Kind())) {
            Visit(unaryExpression.Operand);
            ExpandFront(new DiscardBlock(node.GetLocation()));
            ExpandFront(new UnknownBlock(node.GetLocation()));
          } else {
            throw new NotImplementedException();
          }
          break;
        case ParenthesizedExpressionSyntax parenthesis:
          VisitDesignator(parenthesis.Expression, location);
          break;
        case ThisExpressionSyntax _:
          // TODO: Support struct constructor this assignment
          ExpandFront(new UnknownBlock(node.GetLocation()));
          break;
        case DeclarationExpressionSyntax declaration:
          VisitVariableDesignation(declaration.Designation, location);
          break;
        case InvocationExpressionSyntax invocation:
          var target = _compilationModel.GetReferencedSymbol(invocation);
          if (target is IMethodSymbol callee && !(callee.ReturnsByRef || callee.ReturnsByRefReadonly)) {
            throw new NotImplementedException();
          }
          Visit(invocation);
          break;
        case ConditionalExpressionSyntax conditional:
          VisitConditionalDesignator(conditional, location);
          break;
        case RefExpressionSyntax refExpression:
          VisitDesignator(refExpression.Expression, location);
          break;
        case RefValueExpressionSyntax refValueExpression:
          VisitRefValueExpression(refValueExpression);
          break;
        case PostfixUnaryExpressionSyntax postfixExpression when postfixExpression.IsKind(SyntaxKind.SuppressNullableWarningExpression):
          VisitDesignator(postfixExpression.Operand, location);
          break;
        default:
          throw new NotImplementedException();
      }
    }

    private void VisitConditionalDesignator(ConditionalExpressionSyntax node, Location location) {
      Visit(node.Condition);
      var branch = new BranchBlock(node.GetLocation());
      SucceedFront(branch);
      NewFront(next => branch.SuccessorOnTrue = next);
      VisitDesignator(node.WhenTrue, location);
      var afterIf = _front;
      NewFront(next => branch.SuccessorOnFalse = next);
      VisitDesignator(node.WhenFalse, location);
      NewFront(_front.Union(afterIf));
    }

    public override void VisitDeclarationExpression(DeclarationExpressionSyntax node) {
      VisitVariableDesignation(node.Designation, node.GetLocation());
    }

    private void VisitVariableDesignation(VariableDesignationSyntax designation, Location location) {
      var symbol = _compilationModel.GetDeclaredSymbol(designation);
      VisitVariableDesignator(symbol, location);
    }

    private void VisitVariableDesignator(ISymbol variable, Location location) {
      if (variable == null || variable is IDiscardSymbol) {
        ExpandFront(new UnknownBlock(location));
      } else {
        if (IsInstanceVariable(variable)) {
          ExpandFront(new ThisBlock(location));
        }
        if (IsVariableSymbol(variable)) {
          ExpandFront(new VariableSelectionBlock(location, variable));
        } else {
          throw new NotImplementedException();
        }
      }
    }

    private void VisitElementDesignator(ElementAccessExpressionSyntax access, Location location) {
      Visit(access.Expression);
      Visit(access.ArgumentList);
      var dimensions = access.ArgumentList.Arguments.Count;
      ExpandFront(new ElementSelectionBlock(location, dimensions));
    }

    private void VisitMemberDesignator(MemberAccessExpressionSyntax node, Location location) {
      var member = _compilationModel.GetReferencedSymbol(node);
      if (member == null) {
        ExpandFront(new UnknownBlock(node.GetLocation()));
      } else {
        if (!member.IsStaticSymbol()) {
          Visit(node.Expression);
        }
        if (IsVariableSymbol(member)) {
          ExpandFront(new VariableSelectionBlock(location, member));
        } else {
          // TODO: Support ref property passing (seems to occur in certain cases)
          if (!member.IsStaticSymbol()) {
            ExpandFront(new DiscardBlock(location));
          }
          ExpandFront(new UnknownBlock(location));
        }
      }
    }

    private void VisitWriteBlock(ExpressionSyntax node, Location location) {
      var symbol = _compilationModel.GetReferencedSymbol(node);
      if (symbol is IPropertySymbol property) {
        if (!property.IsStaticSymbol()) {
          if (node is IdentifierNameSyntax) {
            ExpandFront(new ThisBlock(location));
          } else if (node is MemberAccessExpressionSyntax memberAccess) {
            Visit(memberAccess.Expression);
          } else if (node is ElementAccessExpressionSyntax elemAccess) {
            Visit(elemAccess.Expression);
            Visit(elemAccess.ArgumentList);
          } else {
            throw new NotImplementedException();
          }
        }
        ExpandFront(new PropertySetBlock(location, property));
      } else if (node is TupleExpressionSyntax tuple) {
        VisitTupleAssign(tuple);
      } else {
        VisitDesignator(node, location);
        ExpandFront(new WriteBlock(location));
      }
    }

    private void VisitTupleAssign(TupleExpressionSyntax node) {
      for (int index = 0; index < node.Arguments.Count; index++) {
        if (index < node.Arguments.Count - 1) {
          ExpandFront(new DuplicateBlock(node.GetLocation()));
        }
        ExpandFront(new ConstantBlock(node.GetLocation(), index));
        ExpandFront(new ElementSelectionBlock(node.GetLocation(), 1));
        ExpandFront(new ReadBlock(node.GetLocation()));
        VisitWriteBlock(node.Arguments[index].Expression, node.GetLocation());
      }
    }

    private void VisitVariableWrite(ISymbol variable, Location location) {
      if (variable is IPropertySymbol property) {
        if (!property.IsStaticSymbol()) {
          ExpandFront(new ThisBlock(location));
        }
        ExpandFront(new PropertySetBlock(location, property));
      } else {
        VisitVariableDesignator(variable, location);
        ExpandFront(new WriteBlock(location));
      }
    }

    private static readonly HashSet<string> implicitCastOnInc = new() {
      Symbols.CharType,
      Symbols.ByteType,
      Symbols.SignedByteType,
      Symbols.ShortType,
      Symbols.UnsignedShortType
    };

    private void ExtraCastOnIncDec(SyntaxNode node) {
      var type = _compilationModel.GetNodeType(node);
      if (type.IsAny(implicitCastOnInc)) {
        var block = new CastBlock(node.GetLocation(), type);
        ExpandFront(block);
      }
    }

    private void CopyStruct(ExpressionSyntax expression) {
      var type = _compilationModel.GetNodeType(expression);
      if (type != null && type.IsStruct()) {
        ExpandFront(new ObjectCloneBlock(expression.GetLocation(), false));
      }
    }

    private void ImplicitCast(SyntaxNode node) {
      var type = _compilationModel.GetTypeInfo(node);
      if (!SymbolEqualityComparer.Default.Equals(type.Type, type.ConvertedType)) {
        CastType(node, type.ConvertedType);
      }
    }

    private void CastType(SyntaxNode node, ITypeSymbol targetType) {
      if (_compilationModel.GetReferencedSymbol(node) is IMethodSymbol overloader && overloader.MethodKind == MethodKind.Conversion) {
        ExpandFront(new InvocationBlock(node.GetLocation(), overloader, false));
      }
      ExpandFront(new CastBlock(node.GetLocation(), targetType));
    }

    private bool IsEvent(ExpressionSyntax node) {
      return _compilationModel.GetReferencedSymbol(node) is IEventSymbol;
    }

    private static bool IsInstanceVariable(ISymbol symbol) {
      return symbol is IFieldSymbol or IEventSymbol && !symbol.IsStaticSymbol();
    }

    private bool IsConstant(ISymbol symbol) {
      return
        symbol is IFieldSymbol field && field.IsConst ||
        symbol is ILocalSymbol local && local.IsConst;
    }

    private bool IsVariableSymbol(ISymbol symbol) {
      return
        !IsConstant(symbol) &&
        symbol is IFieldSymbol or ILocalSymbol or IParameterSymbol or IEventSymbol or IRangeVariableSymbol;
    }

    private bool IsUnknownConstantValue(ISymbol symbol) {
      return symbol is IFieldSymbol field && field.IsConst && !field.HasConstantValue ||
        symbol is ILocalSymbol local && local.IsConst && !local.HasConstantValue;
    }

    private object GetConstantValue(ISymbol symbol) {
      if (!IsConstant(symbol) || IsUnknownConstantValue(symbol)) {
        throw new Exception();
      }
      if (symbol is IFieldSymbol field) {
        if (!field.HasConstantValue) {
          throw new NotImplementedException();
        }
        return field.ConstantValue;
      }
      if (symbol is ILocalSymbol local) {
        if (!local.HasConstantValue) {
          throw new NotImplementedException();
        }
        return local.ConstantValue;
      }
      throw new NotImplementedException();
    }

    private void UndeclareLocals(SyntaxNode node) {
      if (node != null) {
        foreach (var variable in GetInnerLocalVariables(node)) {
          ExpandFront(new UndeclareBlock(node.GetLocation(), variable));
        }
      }
    }

    private IEnumerable<ILocalSymbol> GetInnerLocalVariables(SyntaxNode node) {
      var localVariables =
        from child in node.DescendantNodesAndSelf(inner => !(inner is WhileStatementSyntax or DoStatementSyntax or ForStatementSyntax or ForEachStatementSyntax))
        where child is LocalDeclarationStatementSyntax
        from declarator in ((LocalDeclarationStatementSyntax)child).Declaration.Variables
        select declarator;
      var outVariables =
        from child in node.DescendantNodesAndSelf()
        where child is DeclarationExpressionSyntax
        let declaration = (DeclarationExpressionSyntax)child
        from variable in (declaration.Designation as ParenthesizedVariableDesignationSyntax)?.Variables.ToArray() ?? new[] { declaration.Designation }
        select variable;
      var patternVariables =
        from child in node.DescendantNodesAndSelf()
        where child is DeclarationPatternSyntax
        select ((DeclarationPatternSyntax)child).Designation;
      return
        from child in localVariables.Cast<SyntaxNode>().
          Union(outVariables).
          Union(patternVariables)
        let symbol = _compilationModel.GetDeclaredSymbol(child)
        where symbol != null
        select (ILocalSymbol)symbol;
    }
  }
}
