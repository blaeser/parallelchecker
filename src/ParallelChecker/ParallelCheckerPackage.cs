using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ParallelChecker {
  [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
  [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
  [ProvideOptionPage(typeof(OptionPageGrid), "Parallel Checker", "General", 0, 0, true)]
  [Guid(PackageGuidString)]
  public sealed class ParallelCheckerPackage : AsyncPackage {
    public const string PackageGuidString = "ab6a73ab-75ed-422c-ae28-100e1ad3649e";

    public ParallelCheckerPackage() { }

    protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {
      await base.InitializeAsync(cancellationToken, progress);
      await FullSolutionAnalysisCheck.InitializeAsync(this);
      await DisplayOptions.InitializeAsync(this);
    }
  }
}
