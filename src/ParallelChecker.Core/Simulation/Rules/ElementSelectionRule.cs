using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Rules {
    internal sealed class ElementSelectionRule : Rule<ElementSelectionBlock> {
    public override int TimeCost => 5; 

    public override void Apply(Program program, ElementSelectionBlock block) {
      var method = program.ActiveMethod;
      var indices = new object[block.Dimensions];
      for (int dim = 0; dim < block.Dimensions; dim++) {
        indices[block.Dimensions - 1 - dim] = method.EvaluationStack.Pop();
      }
      var instance = method.EvaluationStack.Pop();
      if (TrySelectTupleElement(program, instance, indices)) {
        return;
      }
      if (instance == Unknown.Value || UnsupportedIndices(indices) || instance != null && instance is not Model.Array) {
        method.EvaluationStack.Push(Unknown.Value);
      } else {
        var realArray = (Model.Array)instance;
        if (instance == null) {
          throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
        }
        Variable element;
        if (IsRangeAccess(indices)) {
          element = RangeArrayAccess(program, realArray, indices);
        } else {
          element = RegularArrayAccess(program, realArray, indices);
        }
        method.EvaluationStack.Push(element);
      }
      program.GoToNextBlock();
    }

    private static Variable RangeArrayAccess(Program program, Model.Array array, object[] indices) {
      var range = (Library.Range)indices[0];
      var start = (Library.Index)range.GetStart();
      var end = (Library.Index)range.GetEnd();
      var length = array.Lengths[0];
      Model.Array subArray;
      try {
        subArray = array.GetSubRange(program, AbsoluteIndex(start, length), AbsoluteIndex(end, length));
      } catch (IndexOutOfRangeException exception) {
        throw new Model.Exception(program.ActiveLocation, exception);
      }
      return new ImplicitVariable("array", subArray.Type, subArray);
    }

    private static Variable RegularArrayAccess(Program program, Model.Array array, object[] indices) {
      var realIndices =
                  (from index in indices
                   select (int)index.Convert(typeof(int))).ToArray();
      try {
        return array.GetElement(realIndices);
      } catch (IndexOutOfRangeException exception) {
        throw new Model.Exception(program.ActiveLocation, exception);
      }
    }

    private bool TrySelectTupleElement(Program program, object instance, object[] indices) {
      if (indices.Length != 1 || indices[0] is not int) {
        return false;
      }
      var position = (int)indices[0];
      var method = program.ActiveMethod;
      if (instance is Model.ValueTuple tuple) {
        method.EvaluationStack.Push(tuple.Items[position]);
        program.GoToNextBlock();
        return true;
      } else if (instance is not Model.Array && instance is Model.Object obj && obj.Type != null && obj.Type.TryGetDeconstructionMethod(out var deconstruct)) {
        Deconstruct(program, instance, deconstruct, position);
        return true;
      } else {
        return false;
      }
    }

    private static void Deconstruct(Program program, object instance, Microsoft.CodeAnalysis.IMethodSymbol deconstruct, int parameterIndex) {
      var variables = new ExplicitVariable[deconstruct.Parameters.Count()];
      for (int index = 0; index < deconstruct.Parameters.Count(); index++) {
        variables[index] = new ExplicitVariable(deconstruct.Parameters[index]);
      }
      program.ActiveMethod.EvaluationStack.Push(variables[parameterIndex]);
      program.GoToNextBlock();
      program.InvokeMethod(deconstruct, variables, instance, null);
    }

    private static bool UnsupportedIndices(object[] indices) {
      return !IsRangeAccess(indices) &&
        (from index in indices
         where index is Model.Object
         select index).Any();
    }

    private static bool IsRangeAccess(object[] indices) {
      return indices.Length == 1 && indices[0] is Library.Range range && range.IsConcrete;
    }

    private static int AbsoluteIndex(Library.Index index, int length) {
      int value = (int)index.GetValue();
      bool isFromEnd = (bool)index.GetIsFromEnd();
      return isFromEnd ? length - value : value;
    }
  }
}
