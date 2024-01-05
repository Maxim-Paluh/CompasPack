using CompasPack.ViewModel;
using System.Windows;

namespace CompasPack
{
    public partial class MainWindowView : Window
    {
        private MainWindowViewModel _mainWindowViewModel;

        public MainWindowView(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            _mainWindowViewModel = mainWindowViewModel;
            DataContext = _mainWindowViewModel;
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _mainWindowViewModel.LoadAsync();
        }
    }
}
