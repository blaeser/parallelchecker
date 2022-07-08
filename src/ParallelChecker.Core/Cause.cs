using Microsoft.CodeAnalysis;
using System;

namespace ParallelChecker.Core {
  public sealed class Cause {
    public string Description { get; }
    public Location Location { get; }
    public Cause Origin { get; }

    public Cause(string description, Location location, Cause origin) {
      if (string.IsNullOrEmpty(description)) {
        throw new ArgumentException(nameof(description));
      }
      Description = description;
      Location = location;
      Origin = origin;
    }

    public Cause(string description, Location location) 
      : this(description, location, null) {
    }

    // TODO: Think about full equality with corresponding hash code
    public override bool Equals(object obj) {
      return obj is Cause other && Location.Equals(other.Location);
    }

    public override int GetHashCode() {
      return Location.GetHashCode();
    }
  }
}
