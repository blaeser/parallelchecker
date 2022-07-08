using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation {
  internal static class DeadlockMonitor {
    public static void CheckForDeadlock(this Program program) {
      if (program.Options.DetectedIssues.Contains(IssueCategory.Deadlock)) {
        var causes = new List<Cause>();
        var threads = new HashSet<Thread>();
        var first = program.ActiveThread;
        if (first.WaitingOn != null) {
          threads.Add(first);
          causes.Add(new Cause("wait", program.CurrentLocation(first), program.ActiveCause));
          foreach (var next in WaitTargets(first)) {
            CheckWaitCycle(program, first, next, threads, causes);
          }
        }
      }
    }

    private static IEnumerable<Thread> WaitTargets(Thread origin) {
      if (origin.WaitingOn is Thread other) {
        yield return other;
      }
      foreach (var next in origin.WaitingOn.LockHolders) {
        yield return next;
      }
    }

    private static void CheckWaitCycle(Program program, Thread first, Thread next, HashSet<Thread> threads, List<Cause> causes) {
      if (first == next) {
        var message = string.Format("Deadlock with {0} threads", causes.Count);
        program.Issues.Add(new Issue(IssueCategory.Deadlock, message, causes));
      } else if (next.WaitingOn != null && !threads.Contains(next)) {
        threads.Add(next);
        causes.Add(new Cause("wait", program.CurrentLocation(next), next.Cause));
        foreach (var overNext in WaitTargets(next)) {
          CheckWaitCycle(program, first, overNext, threads, causes);
        }
        causes.RemoveAt(causes.Count - 1);
      }
    }
  }
}
