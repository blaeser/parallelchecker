using Microsoft.CodeAnalysis;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Base {
  internal static class VariableLogic {
    public static object ObjectClone(this ITypeSymbol targetType, object value, bool cloneRecord) {
      if (value != null && (cloneRecord || targetType != null && targetType.IsStruct())) {
        var original = (Model.Object)value;
        var clone = new Model.Object(original.Type);
        foreach (var symbol in original.InstanceFields.AllSymbols()) {
          var field = original.InstanceFields[symbol];
          clone.InstanceFields[symbol].Value = field.Type.ObjectClone(field.Value, false);
        }
        return clone;
      } else {
        return value;
      }
    }

    public static bool HasUnknownScope(this Program program, ISymbol variable) {
      return !variable.IsStaticSymbol() && variable.ContainingSymbol is ITypeSymbol &&
        program.ActiveMethod.EvaluationStack.Peek() == Unknown.Value;
    }

    public static bool InExternalScope(this Program program, ISymbol symbol) {
      var model = program.CompilationModel;
      return model.ResolveSyntaxNode<SyntaxNode>(symbol.ContainingType) == null &&
        model.ResolveSyntaxNode<SyntaxNode>(symbol.ContainingSymbol) == null;
    }

    public static void IgnoreVariable(this Program program, ISymbol variable) {
      if (!variable.IsStaticSymbol() && variable.ContainingSymbol is ITypeSymbol) {
        program.ActiveMethod.EvaluationStack.Pop();
      }
    }

    public static bool IsDefinedVariable(this Program program, ISymbol variable) {
      if (variable is IRangeVariableSymbol && !(variable.IsStaticSymbol() || variable.ContainingSymbol is ITypeSymbol || variable.ContainingSymbol is IMethodSymbol)) {
        return false;
      }
      if (!variable.IsStaticSymbol() && variable.ContainingSymbol is ITypeSymbol) {
        var method = program.ActiveMethod;
        return method.EvaluationStack.Peek() != Unknown.Value;
      }
      return true;
    }

    public static void SkipVariableScope(this Program program, ISymbol variable) {
      if (!variable.IsStaticSymbol() && variable.ContainingSymbol is ITypeSymbol) {
        program.ActiveMethod.EvaluationStack.Pop();
      }
    }

    public static Variable GetVariable(this Program program, ISymbol variableSymbol) {
      var scope = GetVariableScope(program, variableSymbol);
      var variable = scope[variableSymbol];
      if (variable.Value is ThreadLocalState threadLocal) {
        variable = threadLocal[program.ActiveThread];
      }
      return variable;
    }

    public static VariableSet GetVariableScope(this Program program, ISymbol variable) {
      var method = program.ActiveMethod;
      if (variable.IsStaticSymbol()) {
        return program.StaticFields;
      } else if (variable.ContainingSymbol is IMethodSymbol) {
        while (method != null && method.Symbol != null && !method.Symbol.Equals(variable.ContainingSymbol, SymbolEqualityComparer.Default) 
          && !method.Symbol.MakeGeneric().Equals(variable.ContainingSymbol, SymbolEqualityComparer.Default)) {
          method = method.Closure;
        }
        if (method == null) {
          throw new System.Exception("Undefined variable scope");
        }
        return method.LocalVariables;
      } else if (variable.ContainingSymbol is ITypeSymbol) {
        var instance = method.EvaluationStack.Pop();
        if (instance == null) {
          throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
        }
        if (instance == Unknown.Value) {
          throw new System.Exception("Invalid object scope");
        }
        if (instance is Model.Object modelObject) {
          return modelObject.InstanceFields;
        } else {
          // TODO: Handle string, Integer etc. as objects
          return new VariableSet();
        }
      } else {
        throw new NotImplementedException();
      }
    }

    public static void SyncVolatileRead(this Program program, Variable variable) {
      var thread = program.ActiveThread;
      thread.Time.SynchronizeWith(variable.VolatileAccess);
      thread.AdvanceTime();
      program.RecordVariations();
    }

    public static void SyncVolatileWrite(this Program program, Variable variable) {
      var thread = program.ActiveThread;
      thread.AdvanceTime();
      variable.VolatileAccess.SynchronizeWith(thread.Time);
      program.RecordVariations();
    }
  }
}
