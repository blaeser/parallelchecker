using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class WriteRule : Rule<WriteBlock> {
    public override int TimeCost => 5; 
    
    public override void Apply(Program program, WriteBlock block) {
      var method = program.ActiveMethod;
      var designator = method.EvaluationStack.Pop();
      var value = method.EvaluationStack.Pop();
      if (designator == Unknown.Value) {
      } else if (designator is Variable variable) {
        if (variable.Value is Model.Object) {
          program.CollectGarbage();
        }
        if (variable.IsVolatile) {
          program.SyncVolatileWrite(variable);
        }
        var access = new Access(variable, variable.IsVolatile);
        program.RecordWrite(access);
        variable.Value = value;
      } else {
        throw new NotImplementedException();
      }
      program.GoToNextBlock();
    }
  }
}
