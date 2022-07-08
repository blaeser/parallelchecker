using Microsoft.CodeAnalysis;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation {
  internal static class UnsafeCallMonitor {
    internal static string[] _generallyUnsafe = {
      "System.Collections",
      "System.Random"
    };

    internal static string[] _exceptionallySafe = {
      "System.Collections.Concurrent",
      "System.Collections.Immutable",
      "System.Collections.Generic.Synchronized"
    };

    public static void RecordCall(this Program program, Call call) {
      if (IsUnsafe(call.Type) && DefinedInstanceType(call.Instance)) {
        var thread = program.ActiveThread;
        var cause = new Cause("Thread-unsafe call", program.CurrentLocation(), program.ActiveCause);
        CheckUnsafeCall(program, call, cause);
        var calls = call.Instance == null ? program.UnsafeCalls.GetValueSet(call.Type) : call.Instance.UnsafeCalls;
        calls.RemoveWhere(epoch => epoch.Thread == thread);
        calls.Add(new Epoch(thread, cause, false));
      }
    }

    private static bool DefinedInstanceType(Object instance) {
      return instance != Unknown.Value && (instance == null || instance.Type != null || instance.NativeType != null);
    }

    private static void CheckUnsafeCall(Program program, Call call, Cause cause) {
      var currentThread = program.ActiveThread;
      if (program.Options.DetectedIssues.Contains(IssueCategory.UnsafeCalls) && currentThread != program.StaticLoader) {
        var otherCalls = GetUnsafeCalls(program, call);
        if (otherCalls != null) {
          CleanIrrelevantThreads(program, otherCalls);
          foreach (var epoch in otherCalls) {
            var otherThread = epoch.Thread;
            if (otherThread != currentThread && !ExcludedThreadPair(program, currentThread, otherThread) && !epoch.HappensBefore(currentThread)) {
              var message = string.Format("Race condition on {0}", call.Type);
              program.Issues.Add(new Issue(IssueCategory.UnsafeCalls, message, new List<Cause>() { cause, epoch.Cause }));
            }
          }
        }
      }
    }
       
    private static bool ExcludedThreadPair(Program program, Thread currentThread, Thread otherThread) {
      return otherThread == program.StaticLoader && currentThread.SyncWithStatic;
    }

    private static void CleanIrrelevantThreads(Program program, HashSet<Epoch> calls) {
      calls.RemoveWhere(epoch => !program.AllThreads.Contains(epoch.Thread));
    }

    private static HashSet<Epoch> GetUnsafeCalls(Program program, Call call) {
      if (call.Instance != null) {
        return call.Instance.UnsafeCalls;
      } else if (program.UnsafeCalls.ContainsKey(call.Type)) {
        return program.UnsafeCalls.GetValueSet(call.Type);
      } else {
        return null;
      }
    }

    private static bool IsUnsafe(ISymbol symbol) {
      var name = symbol.GetFullGenericName();
      if (ContainsPrefix(_exceptionallySafe, name)) {
        return false;
      }
      if (ContainsPrefix(_generallyUnsafe, name)) {
        return true;
      }
      return false;
    }

    private static bool ContainsPrefix(this string[] prefixes, string name) {
      foreach (var prefix in prefixes) {
        if (name.StartsWith(prefix)) {
          return true;
        }
      }
      return false;
    }
  }
}
