namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class Epoch {
    public Thread Thread { get; }
    public long TimePoint { get; }
    public Cause Cause { get; }
    public bool IsVolatile { get; }

    public Epoch(Thread thread, Cause cause, bool isVolatile) {
      Thread = thread;
      TimePoint = thread.Time.TimeComponents[thread];
      Cause = cause;
      IsVolatile = isVolatile;
    }

    public override bool Equals(object obj) {
      return obj is Epoch other &&
        other.Thread == Thread &&
        other.TimePoint == TimePoint &&
        other.IsVolatile == IsVolatile;
    }

    public override int GetHashCode() {
      return
        Thread.GetHashCode() * 31 +
        TimePoint.GetHashCode();
    }

    public bool HappensBefore(Thread otherThread) {
      var otherTime = otherThread.Time.TimeComponents;
      return otherTime.ContainsKey(Thread) && TimePoint < otherTime[Thread];
    }
  }
}
