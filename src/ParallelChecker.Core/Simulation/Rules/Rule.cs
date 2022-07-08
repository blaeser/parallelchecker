using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Rules {
  internal abstract class Rule {
    public Type BlockType { get; }

    public Rule(Type blockType) {
      BlockType = blockType ?? throw new ArgumentNullException(nameof(blockType));
    }

    public abstract int TimeCost { get; }

    public abstract bool Applicable(Program program);

    public abstract void Apply(Program program);
  }

  internal abstract class Rule<T> : Rule where T: BasicBlock {
    public Rule() : 
      base(typeof(T)) {
    }

    public virtual bool Applicable(Program program, T block) {
      return true;
    }

    public abstract void Apply(Program program, T block);

    public override sealed bool Applicable(Program program) {
      return Applicable(program, (T)program.ActiveBlock);
    }

    public override sealed void Apply(Program program) {
      Apply(program, (T)program.ActiveBlock);
    }
  }
}
