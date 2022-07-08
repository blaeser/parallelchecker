using Microsoft.VisualStudio.Shell;
using System;

namespace ParallelChecker {
  public class DisplayOptions {
    private const string EnvironmentVariableName = "ParallelChecker_EnableInfo";

    public static bool EnableInformationMessages {
      get {
        return Environment.GetEnvironmentVariable(EnvironmentVariableName, EnvironmentVariableTarget.User) != null;
      }
      private set {
        Environment.SetEnvironmentVariable(EnvironmentVariableName, value ? "true" : null, EnvironmentVariableTarget.User);
      }
    }

    public static async System.Threading.Tasks.Task InitializeAsync(Package package) {
      var options = (OptionPageGrid)package.GetDialogPage(typeof(OptionPageGrid));
      await options.FinishPendingWorkAsync();
      EnableInformationMessages = options.EnableInformationMessages;
      options.PropertyChanged += (sender, e) => EnableInformationMessages = options.EnableInformationMessages;
    }
  }
}
