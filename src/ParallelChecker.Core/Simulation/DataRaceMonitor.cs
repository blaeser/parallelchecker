using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation {
  internal static class DataRaceMonitor {
    public static void RecordRead(this Program program, Access access) {
      var thread = program.ActiveThread;
      var location = program.CurrentLocation();
      var cause = new Cause("read", location, program.ActiveCause);
      var variable = access.Variable;
      CheckDataRace(program, access, cause, false);
      variable.Reads.RemoveWhere(epoch => epoch.Thread == thread && (!access.IsVolatile || epoch.IsVolatile));
      variable.Reads.Add(new Epoch(thread, cause, access.IsVolatile));
    }

    public static void RecordWrite(this Program program, Access access) {
      var thread = program.ActiveThread;
      var location = program.CurrentLocation();
      var cause = new Cause("write", location, program.ActiveCause);
      var variable = access.Variable;
      CheckDataRace(program, access, cause, true);
      variable.Writes.RemoveWhere(epoch => !access.IsVolatile || epoch.IsVolatile);
      variable.Reads.RemoveWhere(epoch => !access.IsVolatile || epoch.IsVolatile);
      variable.Writes.Add(new Epoch(thread, cause, access.IsVolatile));
    }

    private static void CleanIrrelevantThreads(Program program, Variable variable) {
      variable.Reads.RemoveWhere(epoch => !program.AllThreads.Contains(epoch.Thread));
      variable.Writes.RemoveWhere(epoch => !program.AllThreads.Contains(epoch.Thread));
    }

    private static void CheckDataRace(Program program, Access access, Cause cause, bool isWrite) {
      CleanIrrelevantThreads(program, access.Variable);
      var currentThread = program.ActiveThread;
      if (program.Options.DetectedIssues.Contains(IssueCategory.DataRace) && currentThread != program.StaticLoader) {
        HashSet<Cause> otherCauses = null;
        var variable = access.Variable;
        ExtractDataRaces(program, access, variable.Writes, ref otherCauses);
        if (isWrite) {
          ExtractDataRaces(program, access, variable.Reads, ref otherCauses);
        }
        if (otherCauses != null) {
          var message = string.Format("Data race on {0}", variable);
          foreach (var other in otherCauses) {
            program.Issues.Add(new Issue(IssueCategory.DataRace, message, new List<Cause>() { cause, other }));
          }
        }
      }
    }

    private static void ExtractDataRaces(Program program, Access access, HashSet<Epoch> target, ref HashSet<Cause> otherCauses) {
      var currentThread = program.ActiveThread;
      foreach (var epoch in target) {
        var otherThread = epoch.Thread;
        if (otherThread != currentThread && !ExcludedThreadPair(program, currentThread, otherThread) && !epoch.HappensBefore(currentThread) && !(epoch.IsVolatile && access.IsVolatile)) {
          LazyAdd(ref otherCauses, epoch.Cause);
        }
      }
    }

    private static bool ExcludedThreadPair(Program program, Thread currentThread, Thread otherThread) {
      return otherThread == program.StaticLoader && currentThread.SyncWithStatic;
    }

    private static void LazyAdd(ref HashSet<Cause> set, Cause cause) {
      if (set == null) {
        set = new HashSet<Cause>();
      }
      set.Add(cause);
    }
  }
}
