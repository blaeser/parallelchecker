using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Base {
  internal static class OperatorLogic {
    public static object ApplyTypeTest(this SyntaxKind op, object operand1, object operand2) {
      // TODO: Model string as objects but consider performance
      var type = (ITypeSymbol)operand2;
      if (type == null) {
        return false;
      }
      if (type.TypeKind == TypeKind.TypeParameter) {
        // TODO: Support generic type information at simulation time
        return false;
      }
      if (operand1 is Model.Delegate delegateOperand) {
        return ApplyDelegateTypeTest(delegateOperand, type);
      }
      if (operand1 is Lambda lambdaOperand) {
        return ApplyLambdaTypeTest(lambdaOperand, type);
      }
      if (operand1 != null && operand1 is not Model.Object) {
        return operand1.GetType().IsCompatibleTo(type);
      }
      var instance = (Model.Object)operand1;
      bool compatible = false;
      if (instance != null && instance.NativeType != null) {
        compatible = instance.NativeType.IsCompatibleTo(type);
      } else if (instance != null && instance.Type != null) {
        compatible = instance.Type.IsCompatibleTo(type);
      }
      return op switch {
        SyntaxKind.IsExpression => compatible,
        SyntaxKind.AsExpression => compatible ? instance : null,
        _ => throw new NotImplementedException(),
      };
    }

    private static bool ApplyLambdaTypeTest(Lambda lambda, ITypeSymbol type) {
      return ApplyDelegateTypeTest(lambda.Symbol, type);
    }

    // TODO: Store effective delegate type in Delegate object
    private static bool ApplyDelegateTypeTest(Model.Delegate del, ITypeSymbol type) {
      return ApplyDelegateTypeTest(del.Method, type);
    }

    private static bool ApplyDelegateTypeTest(IMethodSymbol source, ITypeSymbol type) {
      if (type.TypeKind != TypeKind.Delegate) {
        return false;
      }
      var target = (type as INamedTypeSymbol)?.DelegateInvokeMethod;
      if (target == null) {
        return false;
      }
      if (!source.ReturnType.Equals(target.ReturnType, SymbolEqualityComparer.Default)) {
        return false;
      }
      if (source.Parameters.Length != target.Parameters.Length) {
        return false;
      }
      for (int index = 0; index < source.Parameters.Length; index++) {
        if (!IsCompatible(source.Parameters[index], target.Parameters[index])) {
          return false;
        }
      }
      return true;
    }

    private static bool IsCompatible(IParameterSymbol first, IParameterSymbol second) {
      if (first.RefKind != second.RefKind) {
        return false;
      }
      return first.Type.Equals(second.Type, SymbolEqualityComparer.Default);
    }
  }
}
