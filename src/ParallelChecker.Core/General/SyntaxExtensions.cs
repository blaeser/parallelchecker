using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.General {
  internal static class SyntaxExtensions {
    public static bool InvolvesConcurrency(this CompilationModel compilationModel, SyntaxNode node) {
      return AccessesLock(compilationModel, node) || TriggersConcurrency(compilationModel, node);
    }

    public static bool AccessesLock(this CompilationModel compilationModel, SyntaxNode node) {
      compilationModel.ThrowIfCancellationRequested();
      if (node is BaseMethodDeclarationSyntax or TypeDeclarationSyntax or AccessorDeclarationSyntax or AnonymousFunctionExpressionSyntax) {
        var type = node.ContainingType();
        return type != null && compilationModel.VolatileTypes.Contains(type);
      }
      if (PotentialSymbolMatch(LockSymbols, node) && compilationModel.GetReferencedSymbol(node) is IMethodSymbol method) {
        var type = method.ContainingType;
        return type != null && LockTypes.Contains(type.GetFullGenericName());
      }
      if (node is LockStatementSyntax) {
        return true;
      }
      return false;
    }

    // TODO: Collect lock library methods and types automatically
    public static readonly HashSet<string> LockSymbols = new() {
      "Monitor",
      "Enter",
      "Exit",
      "Wait",
      "Pulse",
      "PulseAll",
      "AsyncLocal",
      "Value",
      "AutoResetEvent",
      "Set",
      "Reset",
      "WaitOne",
      "BackgroundWorker",
      "RunWorkerAsync",
      "Barrier",
      "SignalAndWait",
      "SynchronizeAll",
      "BlockingCollection",
      "Count",
      "BoundedCapacity",
      "Add",
      "Take",
      "CountdownEvent",
      "Reset",
      "Signal",
      "AddCount",
      "Interlocked",
      "CompareExchange",
      "Increment",
      "Decrement",
      "Exchange",
      "MemoryBarrier",
      "ManualResetEvent",
      "ManualResetEventSlim",
      "IsSet",
      "Mutex",
      "ReleaseMutex",
      "ReaderWriterLock",
      "AcquireReaderLock",
      "ReleaseReaderLock",
      "AcquireWriterLock",
      "ReleaseWriterLock",
      "ReaderWriterLockSlim",
      "EnterReadLock",
      "ExitReadLock",
      "EnterUpgradeableReadLock",
      "ExitUpgradeableReadLock",
      "EnterWriteLock",
      "ExitWriteLock",
      "Semaphore",
      "Release",
      "TaskScheduler",
      "FromCurrentSynchronizationContext",
      "ThreadLocal",
      "Timer",
      "Change",
      "Dispose",
      "Volatile",
      "Read",
      "Write"
    };

    public static readonly HashSet<string> LockTypes = new() {
      "System.Threading.Monitor",
      "System.Threading.AsyncLocal`1",
      "System.Threading.AutoResetEvent",
      "System.ComponentModel.BackgroundWorker",
      "System.Threading.Barrier",
      "System.Collections.Concurrent.BlockingCollection`1",
      "System.Threading.CountDownEvent",
      "System.Threading.Interlocked",
      "System.Threading.ManualResetEvent",
      "System.Threading.ManualResetEventSlim",
      "System.Threading.Mutex",
      "System.Threading.ReaderWriterLock",
      "System.Threading.ReaderWriterLockSlim",
      "System.Threading.Semaphore",
      "System.Threading.SemaphoreSlim",
      "System.Threading.Tasks.TaskScheduler",
      "System.Threading.ThreadLocal`1",
      "System.Threading.Timer",
      "System.Threading.Volatile",
    };

    private static readonly HashSet<string> concurrencyTrigger = new() {
      Symbols.ThreadStart,
      Symbols.ThreadJoin,
      Symbols.TaskRun,
      Symbols.TaskStart,
      Symbols.TaskWait,
      Symbols.ThreadPoolQueue,
      Symbols.TaskFactoryStartNew,
      Symbols.TaskWaitAll,
      Symbols.TaskWaitAny,
      Symbols.TaskWhenAll,
      Symbols.TaskWhenAny,
      Symbols.ParallelInvoke,
      Symbols.ParallelFor,
      Symbols.ParallelForEach
    };

    // TODO: Look up all locks, volatile etc. and run corresponding methods concurrently
    public static bool TriggersConcurrency(this CompilationModel compilationModel, SyntaxNode node) {
      compilationModel.ThrowIfCancellationRequested();
      if (PotentialSymbolMatch(Symbols.ConcurrencyIndicators, node) && compilationModel.GetReferencedSymbol(node) is IMethodSymbol method) {
        return method.IsAny(concurrencyTrigger) || method.ContainingType.Is(Symbols.Timer); 
        // include waits to detect self-deadlocks
      }
      if (FinalizableObjectCreation(compilationModel, node)) {
        return true;
      }
      if (node is AwaitExpressionSyntax) {
        return true;
      }
      return false;
    }

    private static bool FinalizableObjectCreation(CompilationModel compilationModel, SyntaxNode node) {
      if (node is ObjectCreationExpressionSyntax creation) {
        var identifier = GetIdentifer(creation.Type);
        return compilationModel.FinalizableTypes.Contains(identifier);
      }
      if (node is AnonymousObjectCreationExpressionSyntax or ImplicitObjectCreationExpressionSyntax) {
        var type = compilationModel.GetReferencedSymbol(node)?.ContainingType;
        return type != null && compilationModel.FinalizableTypes.Contains(type.Name);
      }
      return false;
    }

    private static bool PotentialSymbolMatch(ISet<string> candidateNames, SyntaxNode node) {
      if (node is InvocationExpressionSyntax invocation) {
        var identifier = GetIdentifer(invocation.Expression);
        return candidateNames.Contains(identifier);
      } else if (node is ObjectCreationExpressionSyntax creation) {
        var identifier = GetIdentifer(creation.Type);
        return candidateNames.Contains(identifier);
      } else if (node is AnonymousObjectCreationExpressionSyntax or ImplicitObjectCreationExpressionSyntax) {
        return true;
      }
      return false;
    }

    public static string GetIdentifer(this SyntaxNode node) {
      SyntaxToken? token = node switch {
        TypeDeclarationSyntax type => type.Identifier,
        ConstructorDeclarationSyntax constructor => constructor.Identifier,
        MethodDeclarationSyntax method => method.Identifier,
        PropertyDeclarationSyntax property => property.Identifier,
        LocalFunctionStatementSyntax function => function.Identifier,
        MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier,
        IdentifierNameSyntax identifierName => identifierName.Identifier,
        GenericNameSyntax genericName => genericName.Identifier,
        PredefinedTypeSyntax predefinedType => predefinedType.Keyword,
        MemberBindingExpressionSyntax memberBinding => memberBinding.Name.Identifier,
        QualifiedNameSyntax qualifiedName => qualifiedName.Right.Identifier,
        _ => null
      };
      return token?.ValueText;
    }

    public static object ConstantValue(this CompilationModel compilationModel, ExpressionSyntax expression) {
      if (expression is LiteralExpressionSyntax literal) {
        return literal.Token.Value;
      } else if (expression is CastExpressionSyntax cast) {
        var value = ConstantValue(compilationModel, cast.Expression);
        var target = GetPrimitiveType(cast.Type);
        if (target != null) {
          return value.Convert(target);
        }
        throw new NotImplementedException();
      } else if (expression is PrefixUnaryExpressionSyntax unary) {
        var operand = ConstantValue(compilationModel, unary.Operand);
        var op = unary.Kind();
        return op.Apply(operand);
      } else if (expression is BinaryExpressionSyntax binary) {
        var left = ConstantValue(compilationModel, binary.Left);
        var right = ConstantValue(compilationModel, binary.Right);
        var op = binary.Kind();
        return op.Apply(left, right);
      } else if (expression is ParenthesizedExpressionSyntax parenthesis) {
        return ConstantValue(compilationModel, parenthesis.Expression);
      } else {
        var symbol = compilationModel.GetReferencedSymbol(expression);
        if (symbol == null || symbol is ITypeSymbol) {
          return null;
        }
        if (symbol is IFieldSymbol field) {
          return field.ConstantValue;
        }
        if (symbol is ILocalSymbol local) {
          return local.ConstantValue;
        }
        throw new NotImplementedException();
      }
    }

    public static Type GetPrimitiveType(TypeSyntax typeSyntax) {
      if (typeSyntax is PredefinedTypeSyntax predefined) {
        switch (predefined.Keyword.Kind()) {
          case SyntaxKind.BoolKeyword:
            return typeof(bool);
          case SyntaxKind.ByteKeyword:
            return typeof(byte);
          case SyntaxKind.SByteKeyword:
            return typeof(sbyte);
          case SyntaxKind.CharKeyword:
            return typeof(char);
          case SyntaxKind.ShortKeyword:
            return typeof(short);
          case SyntaxKind.UShortKeyword:
            return typeof(ushort);
          case SyntaxKind.IntKeyword:
            return typeof(int);
          case SyntaxKind.UIntKeyword:
            return typeof(uint);
          case SyntaxKind.LongKeyword:
            return typeof(long);
          case SyntaxKind.ULongKeyword:
            return typeof(ulong);
          case SyntaxKind.FloatKeyword:
            return typeof(float);
          case SyntaxKind.DoubleKeyword:
            return typeof(double);
          case SyntaxKind.DecimalKeyword:
            return typeof(decimal);
        }
      }
      return null;
    }

    public static string GetLastIdentifier(this SyntaxNode node) {
      if (node is MemberAccessExpressionSyntax member) {
        return GetLastIdentifier(member.Name);
      } else if (node is SimpleNameSyntax simple) {
        return simple.Identifier.Text;
      } else {
        throw new NotImplementedException();
      }
    }

    public static bool IsPublic(this SyntaxTokenList tokenList) {
      return HasKeyword(tokenList, SyntaxKind.PublicKeyword);
    }

    public static bool IsPrivate(this SyntaxTokenList tokenList) {
      return HasKeyword(tokenList, SyntaxKind.PrivateKeyword);
    }

    public static bool IsStatic(this SyntaxTokenList tokenList) {
      return HasKeyword(tokenList, SyntaxKind.StaticKeyword);
    }

    public static bool IsAbstract(this SyntaxTokenList tokenList) {
      return HasKeyword(tokenList, SyntaxKind.AbstractKeyword);
    }

    public static bool IsExtern(this SyntaxTokenList tokenList) {
      return HasKeyword(tokenList, SyntaxKind.ExternKeyword);
    }

    public static bool IsPartial(this SyntaxTokenList tokenList) {
      return HasKeyword(tokenList, SyntaxKind.PartialKeyword);
    }

    public static bool IsVolatile(this SyntaxTokenList tokenList) {
      return HasKeyword(tokenList, SyntaxKind.VolatileKeyword);
    }

    private static bool HasKeyword(SyntaxTokenList tokenList, SyntaxKind keyword) {
      return tokenList.Any(token => token.IsKind(keyword));
    }

    public static bool IsVoid(this CompilationModel compilationModel, ExpressionSyntax expression) {
      if (expression is PostfixUnaryExpressionSyntax postfix && postfix.IsKind(SyntaxKind.SuppressNullableWarningExpression)) {
        return IsVoid(compilationModel, postfix.Operand);
      }
      if (expression is ConditionalAccessExpressionSyntax conditional) {
        expression = conditional.WhenNotNull;
      }
      if (expression is ObjectCreationExpressionSyntax or ImplicitObjectCreationExpressionSyntax or AnonymousObjectCreationExpressionSyntax) {
        return false;
      }
      if (expression is AssignmentExpressionSyntax assignment && compilationModel.GetReferencedSymbol(assignment.Left) is IEventSymbol) {
        return true;
      }
      var symbol = compilationModel.GetReferencedSymbol(expression);
      if (symbol == null) {
        return false;
      }
      if (symbol is IMethodSymbol method) {
        return method.ReturnsVoid;
      }
      if (symbol is IPropertySymbol && 
          expression is MemberAccessExpressionSyntax or ElementAccessExpressionSyntax or MemberBindingExpressionSyntax or ElementBindingExpressionSyntax or IdentifierNameSyntax) {
        return false;
      }
      if (symbol is ILocalSymbol or IParameterSymbol or IFieldSymbol or IRangeVariableSymbol) {
        return false;
      }
      throw new NotImplementedException();
    }

    public static bool IsMethod(this SyntaxNode node) {
      return node is MethodDeclarationSyntax or LocalFunctionStatementSyntax;
    }

    public static SyntaxToken MethodIdentifier(this SyntaxNode node) {
      if (node is MethodDeclarationSyntax method) {
        return method.Identifier;
      } else if (node is LocalFunctionStatementSyntax local) {
        return local.Identifier;
      }
      throw new NotImplementedException();
    }

    // ordering for deterministic results in repeated analysis runs

    public static IEnumerable<MethodDeclarationSyntax> GetAllNonPrivateMethods(this CompilationModel compilationModel) {
      return
        from method in compilationModel.AllMethods
        where
          !method.GetModifiers().IsPrivate() &&
          !method.GetModifiers().IsAbstract() &&
          !method.GetModifiers().IsExtern()
        select method;
    }

    public static IEnumerable<MethodDeclarationSyntax> GetAttributedMethods(this TypeDeclarationSyntax type, string[] attributes) {
      return
        from member in type.Members
        where member is MethodDeclarationSyntax
        let method = (MethodDeclarationSyntax)member
        where method.HasAnyAttribute(attributes)
        select method;
    }

    public static bool HasAnyAttribute(this MethodDeclarationSyntax method, string[] names) {
      return
        (from attributeList in method.AttributeLists
         from attribute in attributeList.Attributes
         let identifierName = attribute.Name as IdentifierNameSyntax
         where identifierName != null && names.Contains(identifierName.Identifier.Value)
         select attribute).Any();
    }

    public static bool HasAnyAttribute(this MethodDeclarationSyntax method, CompilationModel compilationModel, string attributeFullName) {
      return
        (from attributeList in method.AttributeLists
         from attribute in attributeList.Attributes
         let symbol = compilationModel.GetReferencedSymbol(attribute)
         where symbol != null && symbol.ContainingType != null && symbol.ContainingType.GetFullGenericName() == attributeFullName
         select attribute).Any();
    }

    private const string ModuleInitializerName = "ModuleInitializer";
    private const string ModuleInitializerAttribute = "System.Runtime.CompilerServices.ModuleInitializerAttribute";
    private const string ReservedValueParameterName = "value";
    private const string TupleDeconstructMethodName = "Deconstruct";
    private const string DisposeMethodName = "Dispose";

    public static IList<MethodDeclarationSyntax> RetrieveModuleInitializers(this CompilationModel compilationModel) {
      var optimizedSearch = new string[] { ModuleInitializerName };
      return
        (from method in compilationModel.GetAllNonPrivateMethods()
         where
          method.HasAnyAttribute(optimizedSearch) && // optimization: avoid symbol loading on all attributes
          IsModuleInitializer(compilationModel, method)
         select method).ToList();
    }

    public static bool IsModuleInitializer(this CompilationModel compilationModel, MethodDeclarationSyntax method) {
      return method.HasAnyAttribute(compilationModel, ModuleInitializerAttribute);
    }

    public static IEnumerable<MethodDeclarationSyntax> GetAllMethods(this CompilationModel compilationModel) {
      return
        from type in compilationModel.AllClassDeclarations()
        from member in type.Members
        where member is MethodDeclarationSyntax
        let method = (MethodDeclarationSyntax)member
        orderby method.Identifier.Text
        select method;
    }

    public static IEnumerable<ConstructorDeclarationSyntax> GetPublicConstructors(this TypeDeclarationSyntax type) {
      return
        from member in type.Members
        where member is ConstructorDeclarationSyntax && member.GetModifiers().IsPublic()
        select (ConstructorDeclarationSyntax)member;
    }

    public static IEnumerable<ClassDeclarationSyntax> AllClassDeclarations(this CompilationModel compilationModel) {
      return
        from type in compilationModel.AllTypeDeclarations()
        where type is ClassDeclarationSyntax
        select (ClassDeclarationSyntax)type;
    }

    public static IEnumerable<TypeDeclarationSyntax> AllTypeDeclarations(this CompilationModel compilationModel) {
      return
        from root in compilationModel.AllRoots()
        from child in root.DescendantNodes(
            n => n is CompilationUnitSyntax or NamespaceDeclarationSyntax or TypeDeclarationSyntax)
        where child is TypeDeclarationSyntax
        let type = (TypeDeclarationSyntax)child
        orderby type.Identifier.Text
        select type;
    }

    public static IEnumerable<MemberDeclarationSyntax> GetStaticMembers(this TypeDeclarationSyntax typeDeclaration) {
      return typeDeclaration.GetStaticInitializableMembers().Union(typeDeclaration.GetStaticConstructors());
    }

    public static MultiDictionary<IEventSymbol, MethodDeclarationSyntax> AllUserInterfaceEvents(this CompilationModel compilationModel) {
      var allEvents = 
        from root in compilationModel.AllRoots()
        from child in root.DescendantNodes()
        where child is AssignmentExpressionSyntax
        let assignment = (AssignmentExpressionSyntax)child
        let target = compilationModel.GetReferencedSymbol(assignment.Left)
        where target is IEventSymbol && target.ContainingType.IsUIControl()
        orderby target.Name
        select new { EventSymbol = (IEventSymbol)target, Expression = assignment.Right };
      var result = new MultiDictionary<IEventSymbol, MethodDeclarationSyntax>();
      foreach (var uiEvent in allEvents) {
        // TODO: Support also mere lambdas as UI events, not only methods
        var uiMethods =
          from method in compilationModel.GetReferencedMethodDeclarations(uiEvent.Expression)
          where method != null && !method.Modifiers.IsAbstract() && !method.Modifiers.IsExtern()
          select method;
        result.Add(uiEvent.EventSymbol, uiMethods);
      }
      return result;
    }

    public static IEnumerable<MethodDeclarationSyntax> GetReferencedMethodDeclarations(this CompilationModel compilationModel, ExpressionSyntax expression) {
      return
         (from child in expression.DescendantNodesAndSelf()
         let symbol = compilationModel.GetReferencedSymbol(child)
         where symbol is IMethodSymbol && !((IMethodSymbol)symbol).IsAbstract && !((IMethodSymbol)symbol).IsExtern
         let declaration = compilationModel.ResolveSyntaxNode<BaseMethodDeclarationSyntax>(symbol)
         where declaration is MethodDeclarationSyntax
         select (MethodDeclarationSyntax)declaration).Distinct();
    }

    public static IEnumerable<SyntaxNode> AllRoots(this CompilationModel compilationModel) {
      return
        from tree in compilationModel.Compilation.SyntaxTrees
        orderby tree.FilePath
        select tree.GetRoot(compilationModel.CancellationToken);
    }

    public static IEnumerable<GlobalStatementSyntax> GetTopLevelStatements(this CompilationUnitSyntax compilationUnit) {
      return
        from member in compilationUnit.Members
        where member is GlobalStatementSyntax
        select (GlobalStatementSyntax)member;
    }

    public static IEnumerable<MemberDeclarationSyntax> GetInitializableMembers(this TypeDeclarationSyntax typeDeclaration) {
      return
        from member in typeDeclaration.Members
        where member is FieldDeclarationSyntax or PropertyDeclarationSyntax
        select member;
    }

    public static IEnumerable<MemberDeclarationSyntax> GetStaticInitializableMembers(this TypeDeclarationSyntax typeDeclaration) {
      return
        from member in typeDeclaration.GetInitializableMembers()
        where member.GetModifiers().IsStatic()
        select member;
    }

    public static IEnumerable<VariableDeclaratorSyntax> GetStructDefaultInitializableMembers(this TypeDeclarationSyntax typeDeclaration) {
      return
        from member in typeDeclaration.Members
        where member is FieldDeclarationSyntax && !member.GetModifiers().IsStatic()
        let field = (FieldDeclarationSyntax)member
        where HasKeyword(field.GetModifiers(), SyntaxKind.FixedKeyword)
        from variable in field.Declaration.Variables
        where variable.ArgumentList != null && variable.ArgumentList.Arguments.Any()
        select variable;
    }

    public static IEnumerable<MemberDeclarationSyntax> GetInstanceInitializableMembers(this TypeDeclarationSyntax typeDeclaration) {
      return
        from member in typeDeclaration.GetInitializableMembers()
        where !member.GetModifiers().IsStatic()
        select member;
    }

    public static SyntaxTokenList GetModifiers(this MemberDeclarationSyntax member) {
      if (member is FieldDeclarationSyntax field) {
        return field.Modifiers;
      }
      if (member is PropertyDeclarationSyntax property) {
        return property.Modifiers;
      }
      if (member is MethodDeclarationSyntax method) {
        return method.Modifiers;
      }
      if (member is ConstructorDeclarationSyntax constructor) {
        return constructor.Modifiers;
      }
      if (member is OperatorDeclarationSyntax operatorDeclaration) {
        return operatorDeclaration.Modifiers;
      }
      if (member is DestructorDeclarationSyntax destructor) {
        return destructor.Modifiers;
      }
      throw new NotImplementedException();
    }

    public static IEnumerable<ConstructorDeclarationSyntax> GetStaticConstructors(this TypeDeclarationSyntax type) {
      return
          from member in type.Members
          where member is ConstructorDeclarationSyntax
          let constructor = (ConstructorDeclarationSyntax)member
          where constructor.Modifiers.IsStatic()
          select constructor;
    }

    public static AccessorDeclarationSyntax FindGetAccessor(this BasePropertyDeclarationSyntax property) {
      return FindAccessor(property, SyntaxKind.GetAccessorDeclaration);
    }

    public static AccessorDeclarationSyntax FindInitOrSetAccesor(this BasePropertyDeclarationSyntax property) {
      var accessor = FindAccessor(property, SyntaxKind.InitAccessorDeclaration);
      if (accessor == null) {
        accessor = FindAccessor(property, SyntaxKind.SetAccessorDeclaration);
      }
      return accessor;
    }

    private static AccessorDeclarationSyntax FindAccessor(BasePropertyDeclarationSyntax property, SyntaxKind kind) {
      if (property.AccessorList == null) {
        return null;
      }
      return
        (from accessor in property.AccessorList.Accessors
         where accessor.IsKind(kind)
         select accessor).SingleOrDefault();
    }

    public static ISymbol GetReservedValueParameter(this CompilationModel compilationModel, SyntaxNode body) {
      // TODO: Nicer solution desired for retrieiving "value" symbol
      return
        (from node in body.DescendantNodes()
         where node is IdentifierNameSyntax
         && ((IdentifierNameSyntax)node).Identifier.Text == ReservedValueParameterName
         select compilationModel.GetReferencedSymbol(node)).FirstOrDefault();
    }

    public static bool TryGetDeconstructionMethod(this ITypeSymbol type, out IMethodSymbol deconstructionMethod) {
      deconstructionMethod =
        (from member in type.GetMembers()
        where member is IMethodSymbol method 
          && member.Name == TupleDeconstructMethodName
          && AllParametersAreRef(method)
        select (IMethodSymbol)member).FirstOrDefault();
      return deconstructionMethod != null;
    }

    private static bool AllParametersAreRef(IMethodSymbol method) {
      foreach (var parameter in method.Parameters) {
        if (parameter.RefKind != RefKind.Out) {
          return false;
        }
      }
      return true;
    }

    public static bool TryGetDeconstructionMethod(this CompilationModel compilationModel, ITypeSymbol[] variableTypes, ExpressionSyntax assignee, out IMethodSymbol deconstructionMethod) {
      deconstructionMethod = null;
      var assigneeType = compilationModel.GetNodeType(assignee);
      if (assigneeType == null) {
        return false;
      }
      if (variableTypes.Any(type => type == null)) {
        return false;
      }
      var refKinds = variableTypes.Select(variableType => RefKind.Out).ToArray();
      return TryGetMethod(assigneeType, TupleDeconstructMethodName, variableTypes, refKinds, out deconstructionMethod);
    }

    public static bool TryGetDisposeMethod(this ITypeSymbol containingType, out IMethodSymbol disposeMethod) {
      return TryGetMethod(containingType, DisposeMethodName, null, null, out disposeMethod);
    }

    private static bool TryGetMethod(ITypeSymbol containingType, string methodName, ITypeSymbol[] parameterTypes, RefKind[] parameterRefKinds, out IMethodSymbol method) {
      if(parameterTypes == null) {
        parameterTypes = new ITypeSymbol[0];
      }
      if(parameterRefKinds == null) {
        parameterRefKinds = new RefKind[0];
      }
      method = null;
      var candidates = (from candidate in containingType.GetMembers().OfType<IMethodSymbol>()
                        where candidate.Name.Equals(methodName)
                        where candidate.Parameters.Length == parameterTypes.Length
                        where AreRefKindsCompatible(candidate.Parameters, parameterRefKinds)
                        where AreArgumentsCompatible(candidate.Parameters, parameterTypes)
                        select candidate).ToArray();

      if(candidates.Length > 1) {
        return false;
      }
      method = candidates.FirstOrDefault();
      return method != null;
    }

    public static bool AreArgumentsCompatible(IEnumerable<IParameterSymbol> parameters, IEnumerable<ITypeSymbol> argumentTypes) {
      return parameters
        .Zip(argumentTypes, (parameter, argumentType) => parameter.Type.IsCompatibleTo(argumentType))
        .All(isCompatible => isCompatible);
    }

    public static bool AreRefKindsCompatible(IEnumerable<IParameterSymbol> parameters, IEnumerable<RefKind> refKinds) {
      return parameters
        .Zip(refKinds, (parameter, refKind) => parameter.RefKind == refKind)
        .All(isCompatible => isCompatible);
    }

    public static bool IsRecord(this CompilationModel compilationModel, ITypeSymbol type) {
      return compilationModel.ResolveSyntaxNode<BaseTypeDeclarationSyntax>(type) is RecordDeclarationSyntax;
    }

    public static bool IsPositionalRecordGetter(this CompilationModel compilationModel, IPropertySymbol property) {
      return compilationModel.ResolveSyntaxNode<SyntaxNode>(property) == null &&
        compilationModel.IsRecord(property.ContainingType);
    }

    public static TypeDeclarationSyntax ContainingType(this SyntaxNode node) {
      while (node != null && node is not TypeDeclarationSyntax) {
        node = node.Parent;
      }
      return node as TypeDeclarationSyntax;
    }
  }
}
