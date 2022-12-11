using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class OperatorRule : Rule<OperatorBlock> {
    public override int TimeCost => 10;

    public override void Apply(Program program, OperatorBlock block) {
      if (program.InitializeStatics(block.Method?.ContainingType)) {
        return;
      }
      if (block.Method.IsInLibrary(program)) {
        program.InvokeLibrary(block.Method);
      } else {
        RegularOperator(program, block);
      }
    }

    private static void RegularOperator(Program program, OperatorBlock block) {
      var op = block.Kind;
      try {
        if (IsEventOperator(op)) {
          ApplyEventOperator(program, op);
        } else if (IsPointerOperator(block)) {
          ApplyPointerOperator(program, op);
        } else if (IsIndexOperator(op)) {
          ApplyIndexOperator(program);
        } else if (IsPredefinedOperator(block)) {
          ApplyPrimitiveOperator(program, op);
        } else {
          ApplyOverloadedOperator(program, block.Method);
        }
      } catch (ArithmeticException original) {
        throw new Model.Exception(block.Location, original);
      }
    }

    private static bool IsIndexOperator(SyntaxKind op) {
      return op == SyntaxKind.IndexExpression;
    }

    private static void ApplyIndexOperator(Program program) {
      var method = program.ActiveMethod;
      var value = method.EvaluationStack.Pop();
      var index = new Library.Index(value, true);
      method.EvaluationStack.Push(index);
      program.GoToNextBlock();
    }

    private static bool IsPointerOperator(OperatorBlock block) {
      return block.Method != null && block.Method.ContainingSymbol.Kind == SymbolKind.PointerType;
    }

    private static void ApplyPointerOperator(Program program, SyntaxKind op) {
      // TODO: Support pointer operators
      var method = program.ActiveMethod;
      if (op.IsUnary()) {
        method.EvaluationStack.Pop();
        method.EvaluationStack.Push(Unknown.Value);
      } else if (op.IsBinary()) {
        method.EvaluationStack.Pop();
        method.EvaluationStack.Pop();
        method.EvaluationStack.Push(Unknown.Value);
      } else {
        throw new NotImplementedException();
      }
      program.GoToNextBlock();
    }

    // TODO: Redesign operator logic as ordinary library methods
    private static bool IsPredefinedOperator(OperatorBlock block) {
      if (block.Method == null) {
        return true;
      } else {
        var type = block.Method.ContainingType;
        return type == null || type.Is(Symbols.RootClass) || type.IsPrimitiveType() || 
          type.TypeKind == TypeKind.Delegate || type.TypeKind == TypeKind.Enum;
      }
    }

    private static void ApplyPrimitiveOperator(Program program, SyntaxKind op) {
      var method = program.ActiveMethod;
      if (op.IsUnary()) {
        var operand = method.EvaluationStack.Pop();
        object result;
        if (operand == null) {
          throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
        }
        if (!IsSupportedPrimitiveOperator(op, operand)) {
          result = Unknown.Value;
        } else {
          result = op.Apply(operand);
        }
        method.EvaluationStack.Push(result);
      } else if (op.IsBinary()) {
        var right = method.EvaluationStack.Pop();
        var left = method.EvaluationStack.Pop();
        object result;
        if (left == Unknown.Value || right == Unknown.Value) {
          result = Unknown.Value;
        } else if (op == SyntaxKind.IsExpression || op == SyntaxKind.AsExpression) {
          result = op.ApplyTypeTest(left, right);
        } else if (!IsSupportedPrimitiveOperator(op, left, right)) {
          result = Unknown.Value;
        } else {
          result = op.Apply(left, right);
        }
        method.EvaluationStack.Push(result);
      } else {
        throw new NotImplementedException();
      }
      program.GoToNextBlock();
    }

    private static bool IsSupportedPrimitiveOperator(SyntaxKind op, object operand) {
      return operand is not Model.Object && op.CanApply(operand);
    }

    private static bool IsSupportedPrimitiveOperator(SyntaxKind op, object left, object right) {
      if (op == SyntaxKind.EqualsExpression || op == SyntaxKind.NotEqualsExpression || op == SyntaxKind.AddExpression) {
        return true;
      }
      return !(left is Model.Object || right is Model.Object) && op.CanApply(left, right);
    }

    private static bool IsEventOperator(SyntaxKind op) {
      return
        op == SyntaxKind.SimpleAssignmentExpression ||
        op == SyntaxKind.AddAssignmentExpression ||
        op == SyntaxKind.SubtractAssignmentExpression;
    }

    private static void ApplyEventOperator(Program program, SyntaxKind op) {
      var method = program.ActiveMethod;
      var left = method.EvaluationStack.Pop();
      var right = method.EvaluationStack.Pop();
      if (left != Unknown.Value) {
        ApplyEventOperator(op, left, right);
      }
      program.GoToNextBlock();
    }

    private static void ApplyEventOperator(SyntaxKind op, object left, object right) {
      var target = (Variable)left;
      target.Value ??= new Event();
      var eventNode = (Event)target.Value;
      if (op == SyntaxKind.SimpleAssignmentExpression) {
        eventNode.Handlers.Clear();
        if (right != null) {
          eventNode.Handlers.Add(right);
        }
      } else if (op == SyntaxKind.AddAssignmentExpression) {
        eventNode.Handlers.Add(right);
      } else if (op == SyntaxKind.SubtractAssignmentExpression) {
        eventNode.Handlers.Remove(right);
      } else {
        throw new NotImplementedException();
      }
    }

    private static void ApplyOverloadedOperator(Program program, IMethodSymbol callee) {
      var caller = program.ActiveMethod;
      if (callee.IsOpIncrement() || callee.IsOpDecrement()) {
        caller.IgnoreArguments(1); // constant 1
      }
      var arguments = caller.CollectArguments(callee);
      object thisReference = null;
      program.GoToNextBlock();
      program.InvokeMethod(callee, arguments, thisReference, null);
    }
  }
}
