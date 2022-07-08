using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ParallelChecker.Core.ControlFlow;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Collections.Generic;

namespace ParallelChecker._Test {
  [TestClass]
  public class ControlFlowTest {
    // TODO: Add test cases for syntactically or semantically incorrect program code, e.g. code occurring during program typing
    private const string _TestCasesFilePath = @".\ControlFlowTestCases";
    private const string _CSharpFileSearchPattern = "*.cs";
    private const string _DiagramFileExtension = ".dgml";

    [DynamicData(nameof(ControlFlowTestCases))]
    [DataTestMethod]
    public void TestControlFlowModel(FileInfo file) {
      TestControlFlowCase(file);
      TestUtilities.ForceGarbageCollection();
    }

    public static IEnumerable<object[]> ControlFlowTestCases {
      get {
        var dirInfo = new DirectoryInfo(_TestCasesFilePath);
        return
          from file in dirInfo.GetFiles(_CSharpFileSearchPattern)
          select new object[] { file };
      }
    }

    private static void TestControlFlowCase(FileInfo file) {
      Console.WriteLine("Checking {0}", file.Name);
      var code = TestUtilities.ReadCode(file.FullName);
      var compilationModel = TestUtilities.LoadCompilationModel(OutputKind.ConsoleApplication, new string[] { code }, Array.Empty<string>());
      var controlFlowModel = new ControlFlowModel(compilationModel);
      TestUtilities.CompareAgainstReference(file, fileName => ControlFlowDiagram.Export(controlFlowModel, fileName), _DiagramFileExtension);
    }
  }
}
