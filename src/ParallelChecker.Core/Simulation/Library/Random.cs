using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System")]
  internal sealed class Random : Model.Object {
    private readonly System.Random _random;

    [Member]
    public Random() :
      base(typeof(System.Random)) {
      _random = new System.Random(4711);
    }

    [Member]
    public Random(int seed) :
      base(typeof(System.Random)) {
      _random = new System.Random(seed);
    }

    [Member]
    public object Next(Program program) {
      if (UseUnknown(program)) {
        return Unknown.Value;
      }
      return _random.Next();
    }

    [Member]
    public object Next(Program program, int upper) {
      if (UseUnknown(program)) {
        return Unknown.Value;
      }
      return _random.Next(upper);
    }

    [Member]
    public object Next(Program program, int lower, int upper) {
      if (UseUnknown(program)) {
        return Unknown.Value;
      }
      if (lower > upper) {
        throw new Model.Exception(program.ActiveLocation, new ArgumentOutOfRangeException());
      }
      return _random.Next(lower, upper);
    }

    [Member]
    public object NextDouble(Program program) {
      if (UseUnknown(program)) {
        return Unknown.Value;
      }
      return _random.NextDouble();
    }

    private bool UseUnknown(Program program) {
      return program.Round % 2 > 0;
    }
  }
}
