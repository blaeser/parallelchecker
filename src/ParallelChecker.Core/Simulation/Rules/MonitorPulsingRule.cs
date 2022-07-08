using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class MonitorPulsingRule : Rule<InvocationBlock> {
    public override int TimeCost => 2;

    public override bool Applicable(Program program, InvocationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Method)) {
        return false;
      }
      return block.Method.IsAny(Symbols.MonitorPulse, Symbols.MonitorPulseAll);
    }

    public override void Apply(Program program, InvocationBlock block) {
      if (block.Method.Parameters.Length != 1 || !block.Method.ReturnsVoid) {
        throw new NotImplementedException();
      }
      var method = program.ActiveMethod;
      var instance = method.EvaluationStack.Pop();
      if (instance == null) {
        throw new Model.Exception(program.ActiveLocation, "Monitor pulse on null");
      }
      var monitor = instance as Model.Object ?? Unknown.Value;
      if (monitor != Unknown.Value) {
        if (block.Method.Is(Symbols.MonitorPulse)) {
          monitor.PulseWaiters.NotifySingle();
        } else if (block.Method.Is(Symbols.MonitorPulseAll)) {
          monitor.PulseWaiters.NotifyAll();
        }
      }
      program.GoToNextBlock();
    }
  }
}

