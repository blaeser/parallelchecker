using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class TaskResultRule : Rule<PropertyGetBlock> {
    public override int TimeCost => 2;

    public override bool Applicable(Program program, PropertyGetBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Property)) {
        return false;
      }
      return block.Property.Is(Symbols.TaskResult);
    }

    public override void Apply(Program program, PropertyGetBlock block) {
      // TODO: Eliminate redundancies with thread join
      var currentThread = program.ActiveThread;
      var method = currentThread.ActiveMethod;
      var value = method.EvaluationStack.Pop();
      if (value == null) {
        throw new Exception(program.ActiveLocation, new System.NullReferenceException());
      } else if (value == Unknown.Value) {
        method.EvaluationStack.Push(value);
        currentThread.State = ThreadState.Waiting;
      } else if (value is Thread otherThread) {
        if (program.JoinThread(otherThread, true)) {
          method.EvaluationStack.Push(otherThread.Result);
          program.GoToNextBlock();
        } else {
          method.EvaluationStack.Push(value);
        }
      } else {
        method.EvaluationStack.Push(Unknown.Value);
      }
    }
  }
}
