namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class Unknown : Object {
    public static Unknown Value { get; } = new();

    private Unknown() 
      : base() {
    }

    // TODO: Remove ToString() implementation later
    public override string ToString() {
      return nameof(Unknown);
    }
  }
}
