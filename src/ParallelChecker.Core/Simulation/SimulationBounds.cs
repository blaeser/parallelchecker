namespace ParallelChecker.Core.Simulation {
  internal class SimulationBounds {
    public const long RecursionBound = 50;
    public const long HeapSizeBound = 2_000_000;
    public const long ThreadAmountBound = 105;
    public const int ParallelForLimit = 50;
    public const long InitializationTimePercent = 25;
    public const long MinimumTimeBound = 5_000;
    public const long MaximumTimeBound = 5_000_000;
    public const long PerRunTimePercent = 10;
    public const long MinimumVariationBound = 1;
    public const long MaximumVariationBound = 100;
    public const long ThresholdGC = 10;
    public const int NestedLockScheduling = 50;
    public const long EntryCombinationLimit = 250;

    public long TimeBound { get; set; } = MaximumTimeBound;
    public long VariationBound { get; set; } = MaximumVariationBound;
    public long TotalTime { get; set; } = 0;
  }
}
