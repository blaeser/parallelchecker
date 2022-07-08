using Microsoft.CodeAnalysis;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class Method {
    public IMethodSymbol Symbol { get; }
    public Method Closure { get; }
    public Cause Cause { get; }
    public VariableSet LocalVariables { get; } = new();
    public Object ThisReference { get; set; }
    public Stack<object> EvaluationStack { get; } = new();
    public BasicBlock ActiveBlock { get; set; }
    public Stack<Object> OpenLocks { get; } = new();
    public Stack<Iterator> OpenIterators { get; } = new();
    public Stack<ExceptionHandler> OpenExceptionHandlers { get; } = new();
    public Action<object> ResultInterceptor { get; set; }

    public Method(IMethodSymbol symbol, BasicBlock entry, Method closure, Cause cause) {
      Symbol = symbol;
      ActiveBlock = entry ?? throw new ArgumentNullException(nameof(entry));
      Closure = closure?.Clone();
      Cause = cause;
    }

    private Method Clone() {
      var clone = new Method(Symbol, ActiveBlock, Closure, Cause);
      foreach (var symbol in LocalVariables.AllSymbols()) {
        clone.LocalVariables.SetExplicit(symbol, LocalVariables[symbol]);
      }
      clone.ThisReference = ThisReference;
      EvaluationStack.CopyTo(clone.EvaluationStack);
      OpenLocks.CopyTo(clone.OpenLocks);
      OpenIterators.CopyTo(clone.OpenIterators);
      OpenExceptionHandlers.CopyTo(clone.OpenExceptionHandlers);
      return clone;
    }
  }
}
