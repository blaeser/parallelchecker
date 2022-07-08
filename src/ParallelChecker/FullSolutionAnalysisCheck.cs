using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis.Options;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParallelChecker {
  public class FullSolutionAnalysisCheck {
    private const string FullSolutionMessageTitle = "Parallel Checker";
    private const string FullSolutionManualMessage = "To enable Parallel Checker, set the following option:\n\nTools -> Options -> Text Editor -> C# -> Advanced:\n\"Background analysis scope\": Select \"Entire solution\"\n\nShow this message again?";

    private readonly AsyncPackage _package;

    public static FullSolutionAnalysisCheck Instance { get; private set; }

    private FullSolutionAnalysisCheck(AsyncPackage package) {
      _package = package ?? throw new ArgumentNullException(nameof(package));
    }

    public static async Task InitializeAsync(AsyncPackage package) {
      Instance = new FullSolutionAnalysisCheck(package);
      await Instance.LoadAsync();
    }

    private async Task LoadAsync() {
      var dte = await GetDteAsync();
      var solution = dte.Solution;
      if (solution != null) {
        await CheckActivationAsync();
      }
    }

    private async Task<DTE2> GetDteAsync() {
      return (DTE2)await _package.GetServiceAsync(typeof(DTE));
    }

    private async Task CheckActivationAsync() {
      var options = await GetOptionsAsync();
      if (options.AutomaticFullSolutionAnalysis) {
        await EnableNewSettingAsync();
      }
    }

    private async Task EnableNewSettingAsync() {
      try {
        var crawlerAssembly = GetCrawlerAssembly();
        var crawlerType =
          (from type in crawlerAssembly.DefinedTypes
           where type.Name == "SolutionCrawlerOptionsStorage" select type).Single();
        var analysisAssembly = typeof(Microsoft.CodeAnalysis.Solution).Assembly;
        var scopeType =
          (from type in analysisAssembly.DefinedTypes
           where type.Name == "BackgroundAnalysisScope" select type).Single();
        var field = crawlerType.GetField("BackgroundAnalysisScopeOption", BindingFlags.Public | BindingFlags.Static);
        var value = (IOption)field.GetValue(null);
        var enumValue = scopeType.GetField("FullSolution").GetValue(null);
        if (!Equals(value.DefaultValue, enumValue)) {
          var constructor = value.GetType().GetConstructor(new Type[] { typeof(string), typeof(string), scopeType });
          var newValue = constructor.Invoke(new object[] { value.Feature, value.Name, enumValue });
          field.SetValue(null, newValue);
        }
      } catch (Exception e) {
        Debug.WriteLine($"New setting of full solution analysis failed: {e}");
        await PromptManualSettingAsync();
      }
    }

    private Assembly GetCrawlerAssembly() {
      var analysisAssembly = typeof(Microsoft.CodeAnalysis.Solution).Assembly;
      var version = analysisAssembly.GetName().Version;
      if (version.Major == 4 && version.Minor >= 2 || version.Major > 4) {
        var folder = Path.GetDirectoryName(analysisAssembly.Location);
        var languageServerAssemblyFile = Path.Combine(folder, "Microsoft.CodeAnalysis.LanguageServer.Protocol.dll");
        return Assembly.LoadFrom(languageServerAssemblyFile);
      } else {
        return analysisAssembly;
      }
    }

    private async Task<OptionPageGrid> GetOptionsAsync() {
      var options = (OptionPageGrid)_package.GetDialogPage(typeof(OptionPageGrid));
      await options.FinishPendingWorkAsync();
      return options;
    }

    private async Task PromptManualSettingAsync() {
      var answer = MessageBox.Show(FullSolutionManualMessage, FullSolutionMessageTitle,
        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
      if (answer == DialogResult.No) {
        var options = await GetOptionsAsync();
        options.AutomaticFullSolutionAnalysis = false;
        options.SaveSettingsToStorage();
      }
    }
  }
}
