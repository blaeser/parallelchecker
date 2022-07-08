using ParallelChecker.Core.General;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class VectorTime {
    public Dictionary<Thread, long> TimeComponents { get; } = new();

    public void AdvanceTime(Thread thread) {
      EnsureTimeComponent(thread);
      TimeComponents[thread]++;
    }

    internal void SynchronizeWith(VectorTime other) {
      foreach (var thread in other.TimeComponents.Keys) {
        EnsureTimeComponent(thread);
        TimeComponents[thread] = Math.Max(TimeComponents[thread], other.TimeComponents[thread]);
      }
    }

    private void EnsureTimeComponent(Thread thread) {
      if (!TimeComponents.ContainsKey(thread)) {
        TimeComponents.Add(thread, 0);
      }
    }

    public override bool Equals(object obj) {
      if (obj is not VectorTime) {
        return false;
      }
      var other = (VectorTime)obj;
      return TimeComponents.DictionaryEquals(other.TimeComponents);
    }

    public override int GetHashCode() {
      return TimeComponents.DictionaryHashCode();
    }

    public static bool operator ==(VectorTime first, VectorTime second) {
      return first.Equals(second);
    }

    public static bool operator !=(VectorTime first, VectorTime second) {
      return !first.Equals(second);
    }

    internal bool HappensBefore(VectorTime otherTime) {
      bool smaller = false;
      int intersection = 0;
      foreach (var thread in TimeComponents.Keys) {
        var first = TimeComponents[thread];
        if (otherTime.TimeComponents.TryGetValue(thread, out long second)) {
          intersection++;
        }
        if (first > second) {
          return false;
        } else if (first < second) {
          smaller = true;
        }
      }
      if (otherTime.TimeComponents.Keys.Count > intersection) {
        smaller = true;
      }
      return smaller;
    }

    public bool NeedsSynchronizeWith(Thread thread) {
      return !TimeComponents.TryGetValue(thread, out var time) ||
              time < thread.Time.TimeComponents[thread];
    }
  }
}
