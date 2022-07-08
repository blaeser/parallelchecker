using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class BranchRule : Rule<BranchBlock> {
    public override int TimeCost => 1;

    public override void Apply(Program program, BranchBlock block) {
      var method = program.ActiveMethod;
      var condition = method.EvaluationStack.Pop();
      bool realCondition;
      if (condition == Unknown.Value || !SupportedCondition(condition)) {
        realCondition = program.Random.Next(2) == 1;
        program.Variations += 2;
      } else {
        realCondition = (bool)condition;
      }
      if (realCondition) {
        program.GoToNextBlock(block.SuccessorOnTrue);
      } else {
        program.GoToNextBlock(block.SuccessorOnFalse);
      }
    }

    // TODO: Implement overriden implicit cast operator with non-bool objects/structs, to support all conditions
    // Needed for e.g. System.Management.Automation.SwitchParameter with implicit cast to bool
    private bool SupportedCondition(object condition) {
      return condition is bool;
    }
  }
}
