using Microsoft.CodeAnalysis;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class VariableSelectionRule : Rule<VariableSelectionBlock> {
    public override int TimeCost => 5;

    public override void Apply(Program program, VariableSelectionBlock block) {
      var type = block.Variable.ContainingType;
      if (program.InitializeStatics(type)) {
        return;
      }
      if (type != null && type.IsTupleType) {
        TupleItemSelection(program, (IFieldSymbol)block.Variable);
      } else if ((block.Variable as IFieldSymbol).IsInLibrary(program)) {
        program.SelectLibraryField((IFieldSymbol)block.Variable);
      } else if ((block.Variable as IEventSymbol).IsInLibrary(program)) {
        program.SelectLibraryEvent((IEventSymbol)block.Variable);
      } else {
        RegularVariableSelection(program, block);
      }
    }

    private void TupleItemSelection(Program program, IFieldSymbol field) {
      var method = program.ActiveMethod;
      var instance = method.EvaluationStack.Pop();
      var number = GetTupleElementNumber(field);
      if (instance is ValueTuple tuple && number >= 0 && number < tuple.Items.Length) {
        method.EvaluationStack.Push(tuple.Items[number.Value]);
      } else {
        method.EvaluationStack.Push(Unknown.Value);
      }
      program.GoToNextBlock();
    }

    private int? GetTupleElementNumber(IFieldSymbol field) {
      const string ItemName = "Item";
      if (field.Name.StartsWith(ItemName)) {
        var part = field.Name.Substring(ItemName.Length);
        if (int.TryParse(part, out var number) && number > 0) {
          return number - 1;
        }
      }
      var type = field.ContainingType;
      if (type.TupleElements != null) {
        for (int index = 0; index < type.TupleElements.Length; index++) {
          if (type.TupleElements[index].Name == field.Name) {
            return index;
          }
        }
      }
      return null;
    }

    private static void RegularVariableSelection(Program program, VariableSelectionBlock block) {
      var method = program.ActiveMethod;
      ISymbol variableSymbol = block.Variable;
      if (program.HasUnknownScope(variableSymbol) ||
          program.InExternalScope(variableSymbol)) {
        program.IgnoreVariable(variableSymbol);
        method.EvaluationStack.Push(Unknown.Value);
      } else if (program.IsDefinedVariable(variableSymbol)) {
        var variable = program.GetVariable(variableSymbol);
        method.EvaluationStack.Push(variable);
      } else {
        method.EvaluationStack.Push(Unknown.Value);
      }
      program.GoToNextBlock();
    }
  }
}
