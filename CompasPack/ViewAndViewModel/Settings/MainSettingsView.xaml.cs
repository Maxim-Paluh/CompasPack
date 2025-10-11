using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CompasPack.Settings
{
    /// <summary>
    /// Логика взаимодействия для MainSettingsView.xaml
    /// </summary>
    public partial class MainSettingsView : Window
    {
        private MainSettingsViewModel _mainSettingsViewModel;
        public MainSettingsView(MainSettingsViewModel mainSettingsViewModel)
        {
            InitializeComponent();
            _mainSettingsViewModel = mainSettingsViewModel;
            DataContext = _mainSettingsViewModel;
            this.Loaded += MainWindow_Loaded;
        }
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _mainSettingsViewModel.LoadAsync(null);
        }
    }
}
