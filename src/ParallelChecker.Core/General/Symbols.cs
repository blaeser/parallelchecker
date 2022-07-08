using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.General {
  internal static class Symbols {
    private const string System = "System";
    private const string Threading = $"{System}.Threading";
    private const string Tasks = $"{Threading}.Tasks";
    private const string Windows = $"{System}.Windows";

    public const string Thread = $"{Threading}.Thread";
    public const string Timer = $"{Threading}.Timer";
    public const string Task = $"{Tasks}.Task";
    private const string ValueTask = $"{Tasks}.ValueTask";
    private const string TaskFactory = $"{Tasks}.TaskFactory";
    private const string Parallel = $"{Tasks}.Parallel";
    private const string Monitor = $"{Threading}.Monitor";
    public const string FormsControl = $"{Windows}.Forms.Control";
    public const string UIElement = $"{Windows}.UIElement";

    public const string SystemIndex = "System.Index";
    public const string ThreadStart = $"{Thread}.Start";
    public const string ThreadJoin = $"{Thread}.Join";
    public const string SemaphoreSlimWait = $"{Threading}.SemaphoreSlim.Wait";
    public const string SemaphoreWaitOne = $"{Threading}.Semaphore.WaitOne";
    public const string MutexWaitOne = $"{Threading}.Mutex.WaitOne";
    public const string MonitorEnter = $"{Monitor}.Enter";
    public const string MonitorExit = $"{Monitor}.Exit";
    public const string MonitorWait = $"{Monitor}.Wait";
    public const string MonitorPulse = $"{Monitor}.Pulse";
    public const string MonitorPulseAll = $"{Monitor}.PulseAll";
    public const string ThreadPoolQueue = $"{Threading}.ThreadPool.QueueUserWorkItem";
    public const string TaskStart = $"{Task}.Start";
    public const string TaskWait = $"{Task}.Wait";
    public const string TaskContinueWith = $"{Task}.ContinueWith";
    public const string TaskRun = $"{Task}.Run";
    public const string TaskResult = $"{Task}.Result";
    public const string TaskDelay = $"{Task}.Delay";
    public const string TaskConfigureAwait = $"{Task}.ConfigureAwait";
    public const string ValueTaskConfigureAwait = $"{ValueTask}.ConfigureAwait";
    public const string ValueTaskAsTask = $"{ValueTask}.AsTask";
    public const string TaskFactoryStartNew = $"{TaskFactory}.StartNew";
    public const string TaskFactoryContinueWhenAll = $"{TaskFactory}.ContinueWhenAll";
    public const string TaskFactoryContinueWhenAny = $"{TaskFactory}.ContinueWhenAny";
    public const string TaskWaitAll = $"{Task}.WaitAll";
    public const string TaskWaitAny = $"{Task}.WaitAny";
    public const string TaskWhenAll = $"{Task}.WhenAll";
    public const string TaskWhenAny = $"{Task}.WhenAny";
    public const string ParallelInvoke = $"{Parallel}.Invoke";
    public const string ParallelFor = $"{Parallel}.For";
    public const string ParallelForEach = $"{Parallel}.ForEach";
    public const string ThreadStaticAttribute = $"{System}.ThreadStaticAttribute";

    public const string CharType = $"{System}.Char";
    public const string BoolType = $"{System}.Boolean";
    public const string ByteType = $"{System}.Byte";
    public const string SignedByteType = $"{System}.SByte";
    public const string ShortType = $"{System}.Int16";
    public const string UnsignedShortType = $"{System}.UInt16";
    public const string IntType = $"{System}.Int32";
    public const string UnsignedIntType = $"{System}.UInt32";
    public const string LongType = $"{System}.Int64";
    public const string UnsignedLongType = $"{System}.UInt64";
    public const string FloatType = $"{System}.Single";
    public const string DoubleType = $"{System}.Double";
    public const string DecimalType = $"{System}.Decimal";
    public const string IntPtrType = $"{System}.IntPtr";
    public const string RootClass = $"{System}.Object";
    public const string Nullable = $"{System}.Nullable";

    private const string OpIncrement = "op_Increment";
    private const string OpDecrement = "op_Decrement";
    private const string OpImplicit = "op_Implicit";
    private const string Dispose = "Dispose";

    public static HashSet<string> NonRepeatedEvents { get; } = new() {
      "Loaded",
      "Load",
      "Initialized",
      "Activated",
      "Unloaded",
      "Closing",
      "Deactivate",
      "Closed"
    };

    public static HashSet<string> ConcurrencyIndicators { get; } = new() {
      "Start",
      "Run",
      "StartNew",
      "For",
      "ForEach",
      "Invoke",
      "QueueUserWorkItem",
      "Join",
      "Wait",
      "WaitAll",
      "WaitAny",
      "WhenAll",
      "WhenAny",
      "Timer"
    };

    public static IMethodSymbol GetRangeConstructor(this INamedTypeSymbol type) {
      return (from candidate in type.Constructors
              let parameters = candidate.Parameters
              where parameters.Length == 2 &&
              parameters[0].Type.Is(SystemIndex) &&
              parameters[1].Type.Is(SystemIndex)
              select candidate).SingleOrDefault();
    }

    public static bool Is(this ISymbol symbol, string fullName) => symbol.GetFullName() == fullName;

    public static bool IsAny(this ISymbol symbol, params string[] fullNames) {
      foreach (var name in fullNames) { 
        if (symbol.GetFullName() == name) {
          return true;
        }
      }
      return false;
    }

    public static bool IsAny(this ISymbol symbol, ISet<string> fullNames) => fullNames.Contains(symbol.GetFullName());

    public static bool IsOpImplicit(this IMethodSymbol symbol) {
      return symbol.Name == OpImplicit;
    }

    public static bool IsOpIncrement(this IMethodSymbol symbol) {
      return symbol != null && symbol.Name == OpIncrement;
    }

    public static bool IsOpDecrement(this IMethodSymbol symbol) {
      return symbol != null && symbol.Name == OpDecrement;
    }

    public static bool IsThreadStaticVariable(this ISymbol symbol) {
      return
        symbol.IsStatic &&
        (from attribute in symbol.GetAttributes()
         where attribute.AttributeClass.Is(ThreadStaticAttribute)
         select attribute).Any();
    }

    public static bool IsUIControl(this ITypeSymbol symbol) {
      while (symbol != null) {
        if (symbol.Is(UIElement) || symbol.Is(FormsControl)) {
          return true;
        }
        symbol = symbol.BaseType;
      }
      return false;
    }

    public static ITypeSymbol UnwrapNullable(this ITypeSymbol symbol) {
      return ((INamedTypeSymbol)symbol).TypeArguments[0];
    }

    public static bool IsEnum(this ITypeSymbol symbol) {
      return symbol.TypeKind == TypeKind.Enum;
    }

    private static readonly HashSet<string> primitiveTypes = new() {
      CharType,
      BoolType,
      ByteType,
      SignedByteType,
      ShortType,
      UnsignedShortType,
      IntType,
      UnsignedIntType,
      LongType,
      UnsignedLongType,
      FloatType,
      DoubleType,
      DecimalType
    };

    public static bool IsPrimitiveType(this ITypeSymbol symbol) => symbol.IsAny(primitiveTypes);

    public static bool IsStruct(this ITypeSymbol symbol) {
      return symbol.TypeKind == TypeKind.Struct &&
        !IsPrimitiveType(symbol) && !symbol.Is(Nullable);
    }

    public static bool IsTypeParameter(this ITypeSymbol symbol) {
      return symbol.TypeKind == TypeKind.TypeParameter;
    }

    public static bool IsInterface(this ITypeSymbol symbol) {
      return symbol.TypeKind == TypeKind.Interface;
    }

    public static bool IsDynamic(this ITypeSymbol symbol) {
      return symbol.TypeKind == TypeKind.Dynamic;
    }

    public static bool IsVolatile(this ISymbol symbol) {
      return symbol is IFieldSymbol field && field.IsVolatile;
    }

    public static bool IsDisposable(this ITypeSymbol type) {
      if (type.IsRefLikeType && type.TypeKind == TypeKind.Struct) {
        var candidates =
          from member in type.GetMembers()
          where member is IMethodSymbol method
          && method.ReturnsVoid && method.Parameters.Count() == 0
          && method.Name == Dispose
          select member;
        return candidates.Any();
      }
      return type.IsCompatibleTo(typeof(IDisposable));
    }

    public static bool IsStaticSymbol(this ISymbol symbol) {
      if (symbol is IMethodSymbol method) {
        while (method.MethodKind == MethodKind.LocalFunction) {
          if (method.ContainingSymbol is IMethodSymbol inner) {
            method = inner;
          } else if (method.OriginalDefinition != null && method.OriginalDefinition.ContainingSymbol is IMethodSymbol originalInner) {
            method = originalInner;
          } else {
            throw new NotImplementedException();
          }
        }
        return method.IsStatic;
      } else {
        return symbol.IsStatic;
      }
    }

    public static IMethodSymbol GetDefaultConstructor(this INamedTypeSymbol type) {
      return
        (from constructor in type.Constructors
         where constructor.Parameters.Length == 0 && !constructor.IsStaticSymbol()
         select constructor).SingleOrDefault();
    }

    public static IMethodSymbol ResolveDynamicDispatch(this ITypeSymbol type, IMethodSymbol method) {
      while (type != null && !SymbolEqualityComparer.Default.Equals(type, method.ContainingType)) {
        var candidates =
          (from member in type.GetMembers()
           where member is IMethodSymbol
           let classMethod = (IMethodSymbol)member
           where method.IsOverridenBy(classMethod) ||
                 classMethod.IsImplementedBy(method)
           select classMethod);
        if (candidates.Count() > 1) {
          // TODO: Generics currently leads to ambiguities
          return method;
        }
        var match = candidates.SingleOrDefault();
        if (match != null) {
          return match;
        }
        type = type.BaseType;
      }
      return method;
    }

    public static string GetFullGenericName(this ISymbol symbol) => GetFullName(symbol, true);

    private static string GetFullName(this ISymbol symbol, bool keepGeneric = false) {
      if (symbol == null) {
        return string.Empty;
      } else {
        var name = keepGeneric ? AdjustGenericName(symbol) : symbol.Name;
        if (symbol.ContainingType != null) {
          return GetFullName(symbol.ContainingType, keepGeneric) + "." + name;
        } else if (symbol.ContainingNamespace != null && !symbol.ContainingNamespace.IsGlobalNamespace) {
          return GetFullName(symbol.ContainingNamespace, keepGeneric) + "." + name;
        } else {
          return name;
        }
      }
    }

    private static string AdjustGenericName(ISymbol symbol) {
      var name = symbol.Name;
      if (symbol is INamedTypeSymbol namedType) {
        var genericLength = namedType.TypeParameters.Length;
        if (genericLength > 0) {
          name += "`" + genericLength;
        }
      }
      return name;
    }

    public static string GetFullGenericName(this Type type) {
      var name = type.Name;
      if (!string.IsNullOrEmpty(type.Namespace)) {
        name = type.Namespace + "." + type.Name;
      }
      return name;
    }

    public static bool IsCompatibleTo(this ITypeSymbol source, Type target) {
      if (IsErrorCompatible(source, target)) {
        return true;
      }
      if (target == typeof(object)) {
        return true;
      }
      if (source.IsDynamic()) {
        return target == typeof(object);
      }
      if (source.TypeKind == TypeKind.Error) {
        return true;
      }
      if (target.IsPrimitive) {
        return CompatiblePrimitiveTypes(source, target);
      } else if (target.IsInterface) {
        var names = from entry in source.AllInterfaces()
                    select entry.GetFullGenericName();
        return names.Contains(target.GetFullGenericName());
      } else {
        while (source != null && source.GetFullGenericName() != target.GetFullGenericName()) {
          source = source.BaseType;
        }
        return source?.GetFullGenericName() == target.GetFullGenericName();
      }
    }

    private static bool IsErrorCompatible(ITypeSymbol source, Type target) {
      return source.TypeKind == TypeKind.Error && AdjustGenericName(source) == target.Name;
    }

    public static bool IsCompatibleTo(this Type source, ITypeSymbol target) {
      if (IsErrorCompatible(target, source)) {
        return true;
      }
      if (target.Is(RootClass)) {
        return true;
      }
      if (target.IsDynamic()) {
        return true;
      }
      if (target.TypeKind == TypeKind.Error) {
        return true;
      }
      if (target.TypeKind == TypeKind.TypeParameter) {
        // TODO: Support exact generic type information
        return true;
      }
      if (target.IsPrimitiveType()) {
        return CompatiblePrimitiveTypes(source, target);
      } else if (target.IsInterface()) {
        var names = from entry in source.GetInterfaces()
                    select entry.GetFullGenericName();
        return names.Contains(target.GetFullGenericName());
      } else {
        while (source != null && source.GetFullGenericName() != target.GetFullGenericName()) {
          source = source.BaseType;
        }
        return source?.GetFullGenericName() == target.GetFullGenericName();
      }
    }

    public static bool IsCompatibleTo(this ITypeSymbol source, ITypeSymbol target) {
      if (target.Is(RootClass)) {
        return true;
      }
      if (target.IsDynamic()) {
        return true;
      }
      if (target.TypeKind == TypeKind.Error) {
        return true;
      }
      if (target.TypeKind == TypeKind.TypeParameter) {
        // TODO: Support exact generic type information
        return true;
      }
      if (target.IsPrimitiveType()) {
        return CompatiblePrimitiveTypes(source, target);
      } else if (target.IsInterface()) {
        return source.AllInterfaces().Contains(target);
      } else if (target is IArrayTypeSymbol targetArray) {
        return source is IArrayTypeSymbol sourceArray &&
          sourceArray.ElementType.IsCompatibleTo(targetArray.ElementType);
      } else {
        while (source != null && !SymbolEqualityComparer.Default.Equals(source, target)) {
          source = source.BaseType;
        }
        return SymbolEqualityComparer.Default.Equals(source, target);
      }
    }

    private static readonly Type[][] implicitConversions = {
        new Type[] { typeof(byte), typeof(sbyte) },
        new Type[] { typeof(short), typeof(ushort) },
        new Type[] { typeof(int), typeof(uint) },
        new Type[] { typeof(long), typeof(ulong) },
        new Type[] { typeof(float) },
        new Type[] { typeof(double) },
        new Type[] { typeof(decimal) }
    };

    private static ISet<Type> ImplicitConversionTargets(Type type) {
      var candidates = new HashSet<Type>();
      if (type == typeof(char)) {
        candidates.Add(type);
        type = typeof(ushort);
      }
      int level = 0;
      while (level < implicitConversions.Length && !implicitConversions[level].Contains(type)) {
        level++;
      }
      level++;
      candidates.Add(type);
      while (level < implicitConversions.Length) {
        candidates.AddAll(implicitConversions[level]);
        level++;
      }
      return candidates;
    }

    private static bool CompatiblePrimitiveTypes(ITypeSymbol sourceSymbol, ITypeSymbol targetSymbol) {
      // TODO: Primitive type tests from reference type should not have implicit conversion
      if (SymbolEqualityComparer.Default.Equals(sourceSymbol, targetSymbol)) {
        return true;
      }
      if (nativeTypes.TryGetValue(sourceSymbol.GetFullName(), out var sourceType)) {
        return CompatiblePrimitiveTypes(sourceType, targetSymbol);
      }
      return false;
    }

    private static bool CompatiblePrimitiveTypes(Type sourceType, ITypeSymbol targetSymbol) {
      if (nativeTypes.TryGetValue(targetSymbol.GetFullName(), out var targetType)) {
        return CompatiblePrimitiveTypes(sourceType, targetType);
      }
      return false;
    }

    private static bool CompatiblePrimitiveTypes(ITypeSymbol sourceSymbol, Type targetType) {
      // TODO: Primitive type tests from reference type should not have implicit conversion
      if (nativeTypes.TryGetValue(sourceSymbol.GetFullName(), out var sourceType)) {
        return CompatiblePrimitiveTypes(sourceType, targetType);
      }
      return false;
    }

    private static bool CompatiblePrimitiveTypes(Type sourceType, Type targetType) {
      return ImplicitConversionTargets(sourceType).Contains(targetType);
    }

    public static IEnumerable<IMethodSymbol> AllMethods(this ITypeSymbol type) {
      var methods =
        from member in type.GetMembers()
        where member is IMethodSymbol
        select (IMethodSymbol)member;
      if (type.BaseType != null) {
        methods = methods.Union(AllMethods(type.BaseType), (IEqualityComparer<IMethodSymbol>)SymbolEqualityComparer.Default);
      }
      return methods;
    }

    public static ISet<ITypeSymbol> AllInterfaces(this ITypeSymbol type) {
      var result = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
      if (type.IsInterface()) {
        result.Add(type);
      }
      while (type != null) {
        result.AddAll(type.Interfaces);
        type = type.BaseType;
      }
      return result;
    }

    public static bool ReturnsEnumerator(this IMethodSymbol method) {
      if (method.ReturnsVoid) {
        return false;
      }
      var type = method.ReturnType;
      return
        type.IsCompatibleTo(typeof(System.Collections.IEnumerable)) ||
        type.IsCompatibleTo(typeof(System.Collections.IEnumerator)) ||
        type.IsCompatibleTo(typeof(IEnumerable<>)) ||
        type.IsCompatibleTo(typeof(IEnumerator<>));
    }

    public static IEnumerable<IMethodSymbol> GetDestructors(this ITypeSymbol type) {
      return
        from member in type.GetMembers()
        where member is IMethodSymbol
        let method = (IMethodSymbol)member
        where
          method.MethodKind == MethodKind.Destructor &&
          !method.ContainingType.Is(RootClass)
        select method;
    }

    public static bool HasDestructor(this ITypeSymbol type) {
      return type.GetDestructors().Any();
    }

    public static bool IsImplementedBy(this IMethodSymbol classMethod, IMethodSymbol interfaceMethod) {
      if (!interfaceMethod.ContainingType.IsInterface()) {
        return false;
      }
      if (classMethod.ExplicitInterfaceImplementations.Contains(interfaceMethod)) {
        return true;
      }
      if (classMethod.Name != interfaceMethod.Name) {
        return false;
      }
      if (classMethod.Parameters.Length != interfaceMethod.Parameters.Length) {
        return false;
      }
      for (int index = 0; index < classMethod.Parameters.Length; index++) {
        var param1 = classMethod.Parameters[index];
        var param2 = interfaceMethod.Parameters[index];
        if (!SymbolEqualityComparer.Default.Equals(param1.Type, param2.Type)) {
          return false;
        }
      }
      return true;
    }

    public static bool IsOverridenBy(this IMethodSymbol baseMethod, IMethodSymbol subMethod) {
      while (subMethod != null && !SymbolEqualityComparer.Default.Equals(subMethod, baseMethod)) {
        subMethod = subMethod.OverriddenMethod;
      }
      return SymbolEqualityComparer.Default.Equals(subMethod, baseMethod);
    }

    public static ITypeSymbol GetVariableType(this ISymbol symbol) {
      if (symbol is IFieldSymbol field) {
        return field.Type;
      } else if (symbol is ILocalSymbol local) {
        return local.Type;
      } else if (symbol is IParameterSymbol parameter) {
        return parameter.Type;
      } else if (symbol is IPropertySymbol property) {
        return property.Type;
      } else if (symbol is IEventSymbol eventSymbol) {
        return eventSymbol.Type;
      } else if (symbol is IRangeVariableSymbol) {
        // TODO: range variables have no type, try to infer from from clause expression
        return null;
      } else {
        throw new NotImplementedException();
      }
    }

    private static readonly Dictionary<string, Type> nativeTypes = new() {
      { CharType, typeof(char) },
      { BoolType, typeof(bool) },
      { ByteType, typeof(byte) },
      { SignedByteType, typeof(sbyte) },
      { ShortType, typeof(short) },
      { UnsignedShortType, typeof(ushort) },
      { IntType, typeof(int) },
      { UnsignedIntType, typeof(uint) },
      { LongType, typeof(long) },
      { UnsignedLongType, typeof(ulong) },
      { FloatType, typeof(float) },
      { DoubleType, typeof(double) },
      { DecimalType, typeof(decimal) }
    };

    public static Type GetNativeType(this ITypeSymbol type) {
      if (type.Is(Nullable)) {
        var nativeArgument = GetNativeType(type.UnwrapNullable());
        if (nativeArgument != null) {
          return typeof(Nullable<>).MakeGenericType(new Type[] { nativeArgument });
        } else {
          return null;
        }
      }
      if (type.IsEnum()) {
        return null;
      }
      if (nativeTypes.TryGetValue(type.GetFullName(), out var result)) {
        return result;
      }
      if (type.IsStruct()) {
        return null;
      }
      if (type.TypeKind == TypeKind.Pointer) {
        return null;
      }
      if (type.TypeKind == TypeKind.TypeParameter) {
        return null;
      }
      throw new NotImplementedException();
    }

    private static readonly Dictionary<string, object> defaultValues = new() {
      { CharType, default(char) },
      { BoolType, default(bool) },
      { ByteType, default(byte) },
      { SignedByteType, default(sbyte) },
      { ShortType, default(short) },
      { UnsignedShortType, default(ushort) },
      { IntType, default(int) },
      { UnsignedIntType, default(uint) },
      { LongType, default(long) },
      { UnsignedLongType, default(ulong) },
      { FloatType, default(float) },
      { DoubleType, default(double) },
      { DecimalType, default(decimal) }
    };

    public static object GetDefaultValue(this ITypeSymbol type) {
      if (defaultValues.TryGetValue(type.GetFullName(), out var result)) {
        return result;
      }
      if (type.IsReferenceType) {
        return null;
      }
      if (type.Is(Nullable)) {
        return null;
      }
      if (type.TypeKind == TypeKind.Enum) {
        return default(int);
      }
      if (type.TypeKind == TypeKind.Pointer) {
        return null;
      }
      throw new NotImplementedException();
    }

    private static readonly Dictionary<string, int> typeSizes = new() {
      { CharType, sizeof(char) },
      { BoolType, sizeof(bool) },
      { ByteType, sizeof(byte) },
      { SignedByteType, sizeof(sbyte) },
      { ShortType, sizeof(short) },
      { UnsignedShortType, sizeof(ushort) },
      { IntType, sizeof(int) },
      { UnsignedIntType, sizeof(uint) },
      { LongType, sizeof(long) },
      { UnsignedLongType, sizeof(ulong) },
      { FloatType, sizeof(float) },
      { DoubleType, sizeof(double) },
      { DecimalType, sizeof(decimal) }
    };

    public static int GetSize(this ITypeSymbol type) {
      if (typeSizes.TryGetValue(type.GetFullName(), out var result)) {
        return result;
      }
      if (type.TypeKind == TypeKind.Enum) {
        return sizeof(int);
      }
      if (type.TypeKind == TypeKind.Pointer) {
        // TODO: Analyze 32/64-bit setting of assembly
        return 8;
      }
      throw new NotImplementedException();
    }

    public static IEnumerable<IFieldSymbol> GetInstanceFields(this ITypeSymbol type) {
      var fields =
             from member in type.GetMembers()
             where member is IFieldSymbol
             let field = (IFieldSymbol)member
             where !field.IsStaticSymbol()
             select field;
      if (type.BaseType != null && !type.BaseType.Is(RootClass)) {
        fields = fields.Union(GetInstanceFields(type.BaseType), (IEqualityComparer<IFieldSymbol>)SymbolEqualityComparer.Default);
      }
      return fields;
    }

    public static ITypeSymbol MakeGeneric(this ITypeSymbol type) {
      if (type is INamedTypeSymbol named && named.ConstructedFrom != null) {
        return named.ConstructedFrom;
      }
      return type;
    }

    public static IMethodSymbol MakeGeneric(this IMethodSymbol method) {
      if (method == null) {
        return null;
      }
      var outer = MakeGeneric(method.ContainingSymbol);
      if (outer is ITypeSymbol type) {
        if (!method.IsGenericMethod && SymbolEqualityComparer.Default.Equals(type, method.ContainingSymbol)) {
          return method;
        }
        var exacts =
          (from member in type.GetMembers()
           where member is IMethodSymbol
           let candidate = (IMethodSymbol)member
           where ExactSignatureMatch(method, candidate)
           select candidate);
        if (exacts.Count() == 1) {
          return exacts.Single();
        }
        // TODO: avoid ambiguous matches by using accurate generic types
        var matches =
          (from member in type.GetMembers()
           where member is IMethodSymbol
           let candidate = (IMethodSymbol)member
           where GenericSignatureMatch(method, candidate)
           select candidate);
        if (matches.Count() == 1) {
          return matches.Single();
        }
      }
      return method;
    }

    public static IParameterSymbol MakeGeneric(this IParameterSymbol parameter) {
      var outer = MakeGeneric(parameter.ContainingSymbol);
      if (outer is IMethodSymbol callee) {
        var exacts =
          (from candiate in callee.Parameters
           where candiate.Name == parameter.Name
           select candiate);
        if (exacts.Count() == 1) {
          return exacts.Single();
        }
      }
      return parameter;
    }

    public static ISymbol MakeGeneric(this ISymbol symbol) {
      if (symbol is ITypeSymbol type) {
        return type.MakeGeneric();
      }
      if (symbol is IMethodSymbol method) {
        return method.MakeGeneric();
      }
      if (symbol is IParameterSymbol parameter) {
        return parameter.MakeGeneric();
      }
      return symbol;
    }

    private static bool ExactSignatureMatch(IMethodSymbol actualMethod, IMethodSymbol expectedMethod) {
      if (MatchingMethodName(actualMethod, expectedMethod)) {
        for (int index = 0; index < actualMethod.Parameters.Length; index++) {
          var actualType = actualMethod.Parameters[index]?.Type;
          var expectedType = expectedMethod.Parameters[index]?.Type;
          if (!SymbolEqualityComparer.Default.Equals(actualType, expectedType)) {
            return false;
          }
        }
        return true;
      }
      return false;
    }

    private static bool MatchingMethodName(IMethodSymbol actualMethod, IMethodSymbol expectedMethod) {
      return actualMethod.Name == expectedMethod.Name && actualMethod.Parameters.Length == expectedMethod.Parameters.Length;
    }

    private static bool GenericSignatureMatch(IMethodSymbol actualMethod, IMethodSymbol expectedMethod) {
      if (MatchingMethodName(actualMethod, expectedMethod)) {
        for (int index = 0; index < actualMethod.Parameters.Length; index++) {
          var actualType = actualMethod.Parameters[index]?.Type.MakeGeneric();
          var expectedType = expectedMethod.Parameters[index]?.Type.MakeGeneric() as ITypeSymbol;
          // TODO: Exact match of type parameter would be preferred
          if (!SymbolEqualityComparer.Default.Equals(actualType, expectedType) && (expectedType == null || !expectedType.IsTypeParameter())) {
            return false;
          }
        }
        return true;
      }
      return false;
    }

    public static IPropertySymbol GetProperty(this ITypeSymbol type, string name) {
      return
          (from member in type.GetMembers(name)
           where member is IPropertySymbol
           select (IPropertySymbol)member).SingleOrDefault();
    }
  }
}
