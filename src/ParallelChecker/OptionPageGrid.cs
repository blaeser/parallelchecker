using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace ParallelChecker {
  public class OptionPageGrid : DialogPage, INotifyPropertyChanged {
    private bool _enableInformationMessages = true;

    [Category("Information")]
    [DisplayName("Diagnostic checker information")]
    [Description("Show information messages about analysis in the error list.")]
    public bool EnableInformationMessages {
      get {
        return _enableInformationMessages;
      }
      set {
        _enableInformationMessages = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableInformationMessages)));
      }
    }

    [Category("Assistance")]
    [DisplayName("Automatic full solution analysis")]
    [Description("Automatic activation of full solution analysis.")]
    public bool AutomaticFullSolutionAnalysis { get; set; } = true;

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
