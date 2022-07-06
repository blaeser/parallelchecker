using System.Threading.Tasks;
using System.Windows;

namespace TestAsyncUI
{
    public partial class MainWindow : Window
    {
        private bool isPrime;

        public MainWindow()
        {
            InitializeComponent();
            numberTextBox.Text = "20000000000000003";
        }

        private async void StartCalculationButtonClick(object sender, RoutedEventArgs e)
        {
            calculationResultLabel.Content = "(computing)";
            if (long.TryParse(numberTextBox.Text, out long number))
            {
                await Task.Run(() => isPrime = IsPrime(number));
                if (isPrime)
                {
                    calculationResultLabel.Content = "Prime";
                }
                else
                {
                    calculationResultLabel.Content = "No prime";
                }
            }
        }

        private static bool IsPrime(long number)
        {
            for (long i = 2; i * i <= number; i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
