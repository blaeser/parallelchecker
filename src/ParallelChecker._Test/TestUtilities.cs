using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParallelChecker.Core.General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ParallelChecker._Test {
  internal class TestUtilities {
    private const string _CSharpFileExtension = ".cs";
    private const string _TemporaryFileExtension = ".tmp";

    public static string ReplaceExtension(string filePath, string oldExtension, string newExtension) {
      if (!filePath.EndsWith(oldExtension)) {
        return filePath + newExtension;
      } else {
        return filePath.Remove(filePath.Length - oldExtension.Length) + newExtension;
      }
    }

    private static readonly string[] _libraries = {
        "System.Collections",
        "System.Collections.Concurrent",
        "System.Collections.NonGeneric",
        "System.Console",
        "System.Linq",
        "System.Linq.Parallel",
        "System.Runtime",
        "System.Threading",
        "System.Threading.Tasks",
        "System.Threading.Tasks.Parallel"
      };

    public static CompilationModel LoadCompilationModel(OutputKind outputKind, string[] codes, string[] additionalIncludes) {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var trees =
        (from code in codes
         select CSharpSyntaxTree.ParseText(code, parseOptions)).ToArray();
      var folder = Directory.GetParent(typeof(object).Assembly.Location).FullName;
      var includes = new List<MetadataReference> {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
      };
      includes.AddAll(from library in _libraries
                      select MetadataReference.CreateFromFile(Path.Combine(folder, library + ".dll")));
      includes.AddAll(from include in additionalIncludes
                      select MetadataReference.CreateFromFile(include));
      var compilationOptions = new CSharpCompilationOptions(outputKind).WithAllowUnsafe(true);
      var compilation = CSharpCompilation.Create("UnitTestCompilation", trees, includes, compilationOptions);
      var diagnostics = from diagnostic in compilation.GetDiagnostics() where diagnostic.Severity == DiagnosticSeverity.Error select diagnostic;
      Assert.IsFalse(diagnostics.Any());
      return new CompilationModel(compilation, CancellationToken.None);
    }

    public static string ReadCode(string filePath) {
      return File.ReadAllText(filePath).ReplaceLineEndings("\n");
    }

    public static void CompareAgainstReference(FileSystemInfo file, Action<string> exportToFile, string reportFileExtension) {
      var effectiveFile = ReplaceExtension(file.FullName, _CSharpFileExtension, _TemporaryFileExtension);
      exportToFile(effectiveFile);
      var expectedFile = ReplaceExtension(file.FullName, _CSharpFileExtension, reportFileExtension);
      CompareFiles(file.Name, effectiveFile, expectedFile);
      File.Delete(effectiveFile);
    }

    public static void CompareFiles(string description, string effectiveFile, string expectedFile) {
      // TODO: Use proper XML comparison
      var expected = File.ReadAllText(expectedFile);
      var effective = File.ReadAllText(effectiveFile);
      Assert.AreEqual(expected, effective, string.Format("{0} differs", description));
    }

    public static void ForceGarbageCollection() {
      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();
    }
  }
}
