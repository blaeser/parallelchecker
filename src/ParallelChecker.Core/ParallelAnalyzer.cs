using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

    private static readonly Dictionary<string, HashSet<string>> _cache = new();

    private void SemanticModelAction(SemanticModelAnalysisContext context) {
      var assembly = context.SemanticModel.Compilation.Assembly.Name;
      var file = context.SemanticModel.SyntaxTree.FilePath;
      var reset = false;
      lock (_cache) {
        if (!_cache.ContainsKey(assembly)) {
          _cache[assembly] = new HashSet<string>();
          reset = true;
        }
        var set = _cache[assembly];
        if (set.Contains(file)) {
          set.Clear();
          reset = true;
        }
        set.Add(file);
      }
      if (!reset) {
        return;
      }
      var location = context.SemanticModel.SyntaxTree.GetRoot().GetLocation();
      var watch = Stopwatch.StartNew();
      try {
        var semanticModel = context.SemanticModel;
        var result = ParallelAnalysis.FindIssues(semanticModel.Compilation, context.CancellationToken, _options, out bool faulted);
        ReportIssues(context.ReportDiagnostic, result);
#if DEBUG
        ReportInfo(context.ReportDiagnostic, location, watch, result.Count().ToString(), faulted ? _FaultSign : string.Empty);
#endif
      } catch (Exception exception) {
        ReportInfo(context.ReportDiagnostic, location, watch, _NoneSign, exception.Message);
      }
    }
    
    private void ReportInfo(Action<Diagnostic> report, Location location, Stopwatch watch, string issues, string text) {
      var diagnostic = Diagnostic.Create(_diagnosticInfo, location, watch.ElapsedMilliseconds, issues, text);
      report(diagnostic);
    }

    private void ReportIssues(Action<Diagnostic> report, IEnumerable<Issue> issueList) {
      int number = 0;
      foreach (var issue in issueList) {
        ReportIssue(report, number, issue);
        number++;
      }
    }

    private static void ReportIssue(Action<Diagnostic> report, int number, Issue issue) {
      foreach (var cause in new HashSet<Cause>(issue.Causes)) {
        var title = string.Format(_WarningFormat, number, issue.Message);
        var helpLink = _GeneralHelpLink;
        _helpLinks.TryGetValue(issue.Category, out helpLink);
        var diagnostic = Diagnostic.Create(_diagnosticWarning.Id, _diagnosticWarning.Category, title, _diagnosticWarning.DefaultSeverity, _diagnosticWarning.DefaultSeverity, _diagnosticWarning.IsEnabledByDefault, 3, title, issue.Description, helpLink, cause.Location, issue.Causes.Select(x => x.Location), null, null);
        report(diagnostic);
      }
    }
  }
}
