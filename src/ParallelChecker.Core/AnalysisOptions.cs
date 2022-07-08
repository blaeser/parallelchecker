using System.Collections.Generic;

namespace ParallelChecker.Core {
  public sealed class AnalysisOptions {
    public ISet<IssueCategory> DetectedIssues { get; } = new HashSet<IssueCategory>();
    public bool AllowExternalLocations { get; set; }

    public static AnalysisOptions Default { get; } = new AnalysisOptions {
      DetectedIssues = {
        IssueCategory.DataRace,
        IssueCategory.Deadlock,
        IssueCategory.UnsafeCalls
      }
    };
  }
}
