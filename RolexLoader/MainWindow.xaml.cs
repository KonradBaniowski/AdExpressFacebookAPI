using System.Windows;

namespace RolexLoader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new ViewModel.RolexLoaderViewModel();
            InitializeComponent();
        }
    }
}
