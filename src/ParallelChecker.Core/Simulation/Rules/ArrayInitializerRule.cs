using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ArrayInitializerRule : Rule<ArrayInitializerBlock> {
    public override int TimeCost => 10;

    public override void Apply(Program program, ArrayInitializerBlock block) {
      if (program.InitializeStatics(block.ElementType)) {
        return;
      }
      var method = program.ActiveMethod;
      var array = new Array(new int[] { block.Length }, 1, block.ElementType, program);
      for (int index = block.Length - 1; index >= 0; index--) {
        var element = array.GetElement(new int[] { index });
        element.Value = method.EvaluationStack.Pop();
      }
      program.IncreaseHeapSize(block.Length);
      if (block.Rank > 1) {
        array = Flatten(array, program);
      }
      method.EvaluationStack.Push(array);
      program.GoToNextBlock();
    }

    private Array Flatten(Array array, Program program) {
      if (array.Lengths[0] > 0) {
        var first = array.GetElement(new int[] { 0 }).Value;
        if (first is Array sub) {
          int[] lengths = GetFlatLength(array, sub);
          var flat = new Array(lengths, 1, null, program);
          for (int index = 0; index < array.Lengths[0]; index++) {
            var targetIndex = new int[] { index };
            var element = array.GetElement(targetIndex).Value;
            Copy(flat, targetIndex, element, new int[] { });
          }
          return flat;
        }
      }
      return array;
    }

    private static int[] GetFlatLength(Array flat, Array sub) {
      var lengths = new int[sub.Dimensions + 1];
      lengths[0] = flat.Lengths[0];
      for (int dim = 0; dim < sub.Dimensions; dim++) {
        lengths[dim + 1] = sub.Lengths[dim];
      }
      return lengths;
    }

    private void Copy(Array flat, int[] flatIndex, object sub, int[] subIndex) {
      var dim = flatIndex.Length;
      if (dim == flat.Dimensions) {
        object value;
        if (sub == Unknown.Value) {
          value = sub;
        } else {
          var origin = (Array)sub;
          value = origin.GetElement(subIndex).Value;
        }
        flat.GetElement(flatIndex).Value = value;
      } else {
        for (int index = 0; index < flat.Lengths[dim]; index++) {
          Copy(flat, flatIndex.Append(index), sub, subIndex.Append(index));
        }
      }
    }
  }
}
