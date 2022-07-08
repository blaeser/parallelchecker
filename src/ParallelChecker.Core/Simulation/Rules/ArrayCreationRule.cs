using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ArrayCreationRule : Rule<ArrayCreationBlock> {
    private const int _ElementTime = 10;

    public override int TimeCost => 100;

    public override void Apply(Program program, ArrayCreationBlock block) {
      if (program.InitializeStatics(block.ElementType)) {
        return;
      }
      var method = program.ActiveMethod;
      var lengths = new object[block.Lengths];
      for (int dim = 0; dim < block.Lengths; dim++) {
        lengths[block.Lengths - 1 - dim] = method.EvaluationStack.Pop();
      }
      if (lengths.Contains(Unknown.Value)) {
        method.EvaluationStack.Push(Unknown.Value);
      } else {
        var realLengths = (from entry in lengths select (int)entry.Convert(typeof(int))).ToArray();
        if ((from length in realLengths where length < 0 select length).Any()) {
          throw new Model.Exception(program.ActiveLocation, "Negative array length");
        }
        program.IncreaseSimulationTime(_ElementTime * realLengths.Product());
        try {
          var array = new Model.Array(realLengths, block.Ranks, block.ElementType, program);
          method.EvaluationStack.Push(array);
        } catch (OverflowException exception) {
          throw new Model.Exception(program.ActiveLocation, exception);
        }
      }
      program.GoToNextBlock();
    }
  }
}
