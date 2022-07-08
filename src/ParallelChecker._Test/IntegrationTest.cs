using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParallelChecker.Core;
using ParallelChecker.Core.ControlFlow;
using System;
using System.Collections.Generic;
using System.IO;

namespace ParallelChecker._Test {
  [TestClass]
  public class IntegrationTest {
    private const string _TestCasesFilePath = @".\IntegrationTestCases";
    private const string _CSharpFileSearchPattern = "*.cs";
    private const string _ReportFileExtension = ".xml";

    [DynamicData(nameof(IntegrationTestCases))]
    [DataTestMethod]
    public void TestIntegration(OutputKind outputKind, DirectoryInfo projectFolder) {
      TestProject(projectFolder, outputKind);
      TestUtilities.ForceGarbageCollection();
    }

    public static IEnumerable<object[]> IntegrationTestCases {
      get {
        var dirInfo = new DirectoryInfo(_TestCasesFilePath);
        foreach (var kindFolder in dirInfo.GetDirectories()) {
          var outputKind = (OutputKind)Enum.Parse(typeof(OutputKind), kindFolder.Name);
          foreach (var projectFolder in kindFolder.GetDirectories()) {
            yield return new object[] { outputKind, projectFolder };
          }
        }
      }
    }

    private static void TestProject(DirectoryInfo folder, OutputKind outputKind) {
      Console.WriteLine("Checking {0}", folder.Name);
      var codes = ReadAllCodeFiles(folder);
      var additionalIncludes = outputKind switch
      {
        OutputKind.DynamicallyLinkedLibrary => new string[] { typeof(TestMethodAttribute).Assembly.Location },
        _ => Array.Empty<string>()
      };
      var compilationModel = TestUtilities.LoadCompilationModel(outputKind, codes, additionalIncludes);
      var controlFlowModel = new ControlFlowModel(compilationModel);
      var issues = ParallelAnalysis.FindIssues(compilationModel.Compilation, compilationModel.CancellationToken, AnalysisOptions.Default, out bool faulted);
      Assert.IsFalse(faulted, "Internal implementation exception occurred during analysis");
      TestUtilities.CompareAgainstReference(folder, fileName => IssueReport.Export(issues, fileName), _ReportFileExtension);
    }

    private static string[] ReadAllCodeFiles(DirectoryInfo folder) {
      var result = new List<string>();
      foreach (var file in folder.GetFiles(_CSharpFileSearchPattern)) {
        result.Add(TestUtilities.ReadCode(file.FullName));
      }
      return result.ToArray();
    }
  }
}
