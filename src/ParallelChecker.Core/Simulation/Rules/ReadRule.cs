using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ReadRule : Rule<ReadBlock> {
    public override int TimeCost => 5;
    
    public override void Apply(Program program, ReadBlock block) {
      var method = program.ActiveMethod;
      var designator = method.EvaluationStack.Pop();
      object value;
      if (designator == Unknown.Value) {
        value = Unknown.Value;
      } else if (designator is Variable variable) {
        if (variable.IsVolatile) {
          program.SyncVolatileRead(variable);
        }
        var access = new Access(variable, variable.IsVolatile);
        program.RecordRead(access);
        value = variable.Value;
      } else {
        throw new NotImplementedException();
      }
      method.EvaluationStack.Push(value);
      program.GoToNextBlock();
    }
  }
}
