using Microsoft.CodeAnalysis;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ParallelForRule : Rule<InvocationBlock> {
    public override int TimeCost => 10;

    public override bool Applicable(Program program, InvocationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Method)) {
        return false;
      }
      return block.Method.IsAny(Symbols.ParallelFor, Symbols.ParallelForEach);
    }

    public override void Apply(Program program, InvocationBlock block) {
      if (block.Method.ReturnsVoid) {
        throw new NotImplementedException();
      }
      if (program.ActiveThread.WaitState == null) {
        StartTasks(program);
      } else {
        JoinTasks(program);
      }
    }

    private void StartTasks(Program program) {
      var block = (InvocationBlock)program.ActiveBlock;
      var callee = block.Method;
      if (callee.Is(Symbols.ParallelFor)) {
        StartParallelFor(program, callee);
      } else if (callee.Is(Symbols.ParallelForEach)) {
        StartParallelForEach(program, callee);
      } else {
        throw new NotImplementedException();
      }
    }

    private void StartParallelFor(Program program, IMethodSymbol callee) {
      // TODO: Support extra arguments of overloaded Parallel.For
      var method = program.ActiveMethod;
      var taskDelegate = method.EvaluationStack.Pop();
      method.IgnoreArguments(callee.Parameters.Length - 3);
      var toArgument = method.EvaluationStack.Pop();
      var fromArgument = method.EvaluationStack.Pop();
      if (taskDelegate == Unknown.Value || toArgument == Unknown.Value || fromArgument == Unknown.Value) {
        program.UnknownCall(callee);
        program.GoToNextBlock();
      } else {
        var state = new JoinWaitState();
        var from = (int)fromArgument;
        var to = (int)toArgument;
        to = from + Math.Min(to - from, SimulationBounds.ParallelForLimit); 
        for (int index = from; index < to; index++) {
          StartTask(program, taskDelegate, state, index);
        }
        program.ActiveThread.WaitState = state;
      }
    }

    private void StartParallelForEach(Program program, IMethodSymbol callee) {
      // TODO: Support extra arguments of overloaded Parallel.ForEach
      var method = program.ActiveMethod;
      var ignoreParams = callee.Parameters.Length - 2;
      var instance = method.EvaluationStack.DeepPeek(1 + ignoreParams);
      if (instance is Query lazy && !lazy.TryFetchAll(program)) {
        return;
      }
      var taskDelegate = method.EvaluationStack.Pop();
      method.IgnoreArguments(ignoreParams);
      method.EvaluationStack.Pop(); // skip instance
      if (instance == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      if (taskDelegate == Unknown.Value || instance == Unknown.Value) {
        program.UnknownCall(callee);
        program.GoToNextBlock();
      } else {
        var state = new JoinWaitState();
        var query = instance.ExtractQuery();
        if (query.RaceTarget?.Type != null) {
          program.RecordCall(new Call(query.RaceTarget.Type, query.RaceTarget));
        }
        // TODO: Other approach to limit, e.g. partitioning
        var all = Limit(query.AllItems);
        foreach (var item in all) {
          StartTask(program, taskDelegate, state, item);
        }
        program.ActiveThread.WaitState = state;
      }
    }

    private IEnumerable Limit(IEnumerable enumerable) {
      int count = 0;
      foreach (var item in enumerable) {
        count++;
        if (count > SimulationBounds.ParallelForLimit) {
          break;
        }
        yield return item;
      }
    }

    private void StartTask(Program program, object taskDelegate, JoinWaitState state, object value) {
      if (program.IsThreadStart(taskDelegate)) {
        var childThread = program.CreateThread(taskDelegate);
        childThread.PassSingleParameter(0, value);
        program.StartThread(childThread);
        state.AwaitedThreads.Add(childThread);
      }
    }

    private void JoinTasks(Program program) {
      var parentThread = program.ActiveThread;
      var state = (JoinWaitState)parentThread.WaitState;
      foreach (var childThread in state.AwaitedThreads) {
        if (!program.JoinThread(childThread, true)) {
          return;
        }
      }
      parentThread.WaitState = null;
      program.ActiveMethod.EvaluationStack.Push(null);
      program.GoToNextBlock();
    }
  }
}
