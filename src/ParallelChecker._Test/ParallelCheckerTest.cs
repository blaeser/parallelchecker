using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParallelChecker.Core;
using ParallelChecker.Core.ControlFlow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ParallelChecker._Test {
  [TestClass]
  public class ParallelCheckerTest {
    private const string _TestCasesFilePath = @".\ParallelCheckerCases";
    private const string _CSharpFileSearchPattern = "*.cs";
    private const string _ReportFileExtension = ".xml";

    [DynamicData(nameof(ParallelTestCases))]
    [DataTestMethod]
    public void TestParallelChecker(FileInfo file) {
      CheckParallelCase(file);
      TestUtilities.ForceGarbageCollection();
    }

    public static IEnumerable<object[]> ParallelTestCases {
      get {
        var dirInfo = new DirectoryInfo(_TestCasesFilePath);
        return 
          from file in dirInfo.GetFiles(_CSharpFileSearchPattern)
          select new object[] { file };
      }
    }

    private static void CheckParallelCase(FileInfo file) {
      Console.WriteLine("Checking {0}", file.Name);
      var code = TestUtilities.ReadCode(file.FullName);
      var compilationModel = TestUtilities.LoadCompilationModel(OutputKind.ConsoleApplication, new string[] { code }, Array.Empty<string>());
      var controlFlowModel = new ControlFlowModel(compilationModel);
      var issues = ParallelAnalysis.FindIssues(compilationModel.Compilation, compilationModel.CancellationToken, AnalysisOptions.Default, out bool faulted);
      Assert.IsFalse(faulted, $"Internal implementation exception occurred during analysis {file.Name}");
      TestUtilities.CompareAgainstReference(file, fileName => IssueReport.Export(issues, fileName), _ReportFileExtension);
    }
  }
}
