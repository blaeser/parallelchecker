using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using ParallelChecker.Core.General;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace ParallelChecker.Core {
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class ParallelAnalyzer : DiagnosticAnalyzer {
    private const string _DiagnosticId = "ParallelChecker";
    private const string _DiagnosticTitle = "Concurrency Issue Detection";
    private const string _WarningFormat = "Issue: #{0} {1}";
    private const string _InfoFormat = "Detection in {0} ms ({1} issues) {2}";
    private const string _Category = "Parallelization";
    private const string _FaultSign = "*";
    private const string _NoneSign = "-";

    private static readonly Dictionary<IssueCategory, string> _helpLinks = new() {
      { IssueCategory.DataRace, "https://github.com/blaeser/parallelchecker/blob/main/doc/DataRace.md" },
      { IssueCategory.Deadlock, "https://github.com/blaeser/parallelchecker/blob/main/doc/Deadlock.md" },
      { IssueCategory.UnsafeCalls, "https://github.com/blaeser/parallelchecker/blob/main/doc/ThreadUnsafeUsage.md" }
    };

    private const string _GeneralHelpLink = "https://github.com/blaeser/parallelchecker";

    private static readonly DiagnosticDescriptor _diagnosticWarning =
      new(_DiagnosticId, _DiagnosticTitle, _WarningFormat,
        _Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, helpLinkUri: _GeneralHelpLink);
    private static readonly DiagnosticDescriptor _diagnosticInfo =
      new(_DiagnosticId, _DiagnosticTitle, _InfoFormat,
        _Category, DiagnosticSeverity.Info, isEnabledByDefault: true, helpLinkUri: _GeneralHelpLink);

    private static readonly AnalysisOptions _options = new() {
      DetectedIssues = { IssueCategory.DataRace, IssueCategory.Deadlock, IssueCategory.UnsafeCalls }
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics {
      get {
        return ImmutableArray.Create(_diagnosticWarning, _diagnosticInfo);
      }
    }

    public override void Initialize(AnalysisContext context) {
      context.EnableConcurrentExecution();
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
      context.RegisterSemanticModelAction(SemanticModelAction);
    }

    private class CacheEntry {
      public HashSet<string> Files { get; } = new();
      public Collection<Issue> Issues { get; } = new();
    }

    private static readonly Dictionary<string, CacheEntry> _cache = new();

    private void SemanticModelAction(SemanticModelAnalysisContext context) {
      var assembly = context.SemanticModel.Compilation.Assembly.Name;
      var file = context.SemanticModel.SyntaxTree.FilePath;
      var reset = false;
      lock (_cache) {
        if (!_cache.ContainsKey(assembly)) {
          _cache[assembly] = new();
          reset = true;
        }
        var set = _cache[assembly].Files;
        if (set.Contains(file)) {
          set.Clear();
          reset = true;
        }
        set.Add(file);
      }
      var tree = context.SemanticModel.SyntaxTree;
      var location = tree.GetRoot().GetLocation();
      var watch = Stopwatch.StartNew();
      if (reset) {
        try {
          var semanticModel = context.SemanticModel;
          var result = ParallelAnalysis.FindIssues(semanticModel.Compilation, context.CancellationToken, _options, out bool faulted);
          lock (_cache) {
            _cache[assembly].Issues.Clear();
            _cache[assembly].Issues.AddAll(result);
          }
          ReportIssues(context.ReportDiagnostic, tree, result);
#if DEBUG
          ReportInfo(context.ReportDiagnostic, location, watch, result.Count().ToString(), faulted ? _FaultSign : string.Empty);
#else
          if (faulted) {
            ReportInfo(context.ReportDiagnostic, location, watch, result.Count().ToString(), faulted ? _FaultSign : string.Empty);
          }
#endif
        } catch (OperationCanceledException) {
          lock (_cache) {
            _cache.Remove(assembly);
          }
#if DEBUG
          ReportInfo(context.ReportDiagnostic, location, watch, _NoneSign, $"Cancelled");
#endif
        } catch (Exception exception) {
          ReportInfo(context.ReportDiagnostic, location, watch, _NoneSign, exception.Message);
        }
      } else {
#if DEBUG
        lock (_cache) {
          ReportInfo(context.ReportDiagnostic, location, watch, _NoneSign, $"Cached {_cache[assembly].Issues}");
        }
#endif
        ReportIssues(context.ReportDiagnostic, tree, _cache[assembly].Issues);
      }
    }
    
    private void ReportInfo(Action<Diagnostic> report, Location location, Stopwatch watch, string issues, string text) {
      var diagnostic = Diagnostic.Create(_diagnosticInfo, location, watch.ElapsedMilliseconds, issues, text);
      report(diagnostic);
    }

    private void ReportIssues(Action<Diagnostic> report, SyntaxTree tree, IEnumerable<Issue> issueList) {
      int number = 0;
      foreach (var issue in issueList) {
        ReportIssue(report, tree, number, issue);
        number++;
      }
    }

    private static void ReportIssue(Action<Diagnostic> report, SyntaxTree tree, int number, Issue issue) {
      foreach (var cause in new HashSet<Cause>(issue.Causes)) {
        if (tree != null && tree.FilePath == cause.Location.SourceTree?.FilePath) {
          var location = Location.Create(tree, cause.Location.SourceSpan);
          var title = string.Format(_WarningFormat, number, issue.Message);
          var helpLink = _GeneralHelpLink;
          _helpLinks.TryGetValue(issue.Category, out helpLink);
          var diagnostic = Diagnostic.Create(_diagnosticWarning.Id, _diagnosticWarning.Category, title, _diagnosticWarning.DefaultSeverity, _diagnosticWarning.DefaultSeverity, _diagnosticWarning.IsEnabledByDefault, 3, title, issue.Description, helpLink, location, issue.Causes.Select(x => x.Location), null, null);
          report(diagnostic);
        }
      }
    }
  }
}
