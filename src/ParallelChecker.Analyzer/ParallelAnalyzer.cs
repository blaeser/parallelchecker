using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using ParallelChecker.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace ParallelChecker.Analyzer {
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class ParallelAnalyzer : DiagnosticAnalyzer {
    private const string _DiagnosticId = "ParallelChecker";
    private const string _WarningTitle = "Concurrency Issue";
    private const string _WarningFormat = "Issue: #{0} {1}";
    private const string _Category = "Parallelization";
    private const string _HelpLink = "https://github.com/blaeser/parallelchecker";
    private const string _FaultSign = "*";
    private const string _NoneSign = "-";

    private static readonly DiagnosticDescriptor _diagnosticWarning =
      new DiagnosticDescriptor(_DiagnosticId, _WarningTitle, _WarningFormat,
        _Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, helpLinkUri: _HelpLink);

    private static readonly AnalysisOptions _options = new AnalysisOptions {
      DetectedIssues = { IssueCategory.DataRace, IssueCategory.Deadlock, IssueCategory.UnsafeCalls }
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics {
      get {
        return ImmutableArray.Create(_diagnosticWarning);
      }
    }

    public override void Initialize(AnalysisContext context) {
      context.EnableConcurrentExecution();
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
      context.RegisterSemanticModelAction(SemanticModelAction);
    }

    private static readonly Dictionary<string, HashSet<string>> _cache = new Dictionary<string, HashSet<string>>();

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
      var watch = Stopwatch.StartNew();
      try {
        var semanticModel = context.SemanticModel;
        var result = ParallelAnalysis.FindIssues(semanticModel.Compilation, context.CancellationToken, _options, out bool faulted);
        ReportIssues(context.ReportDiagnostic, result);
        ReportInfo(watch, result.Count().ToString(), faulted ? _FaultSign : string.Empty);
      } catch (Exception exception) {
        ReportInfo(watch, _NoneSign, exception.Message);
      }
    }

    private void ReportInfo(Stopwatch watch, string issues, string text) {
      Console.WriteLine($"Detection in {watch.ElapsedMilliseconds} ms ({issues} issues) {text}");
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
        var diagnostic = Diagnostic.Create(_diagnosticWarning, cause.Location, number, issue.Description);
        report(diagnostic);
      }
    }
  }
}
