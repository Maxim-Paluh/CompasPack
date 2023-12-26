using System;
using System.Reflection;
using System.Windows;

namespace CompasPack.View
{
    /// <summary>
    /// Логика взаимодействия для About.xaml
    /// </summary>
    public partial class AboutView : Window
    {

        public AboutView()
        {

            DataContext = new AboutContext()
            {
                Name = $"Comp@sPack (32 розрядна версія) {DateTime.Now:yyyy} рік",
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                Autor = $"©Greg_House_M_D {DateTime.Now:yyyy} рік",
                Info = "Дане програмне забезпечення написане для магазину: \"Комп@с\"\nта може вільно використовуватись та поширюватись!"
            };
            InitializeComponent();
        }
    }

    public class AboutContext : ViewModelBase
    {
        private string _name;
        private string _version;
        private string autor;
        private string _info;

        public string Info
        {
            get { return _info; }
            set
            {
                _info = value;
                OnPropertyChanged();
            }
        }
        public string Autor
        {
            get { return autor; }
            set
            {
                autor = value;
                OnPropertyChanged();
            }
        }
        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

}
