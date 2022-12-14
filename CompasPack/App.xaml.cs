using Autofac;
using CompasPack.Startup;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CompasPack
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindowView mainWindow = new Bootstrapper().Bootstrap().Resolve<MainWindowView>();
            mainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var textError = "Виникло непередбачувана помилка.\n" + e.Exception.Message + "\n" + e.Exception.StackTrace;
            MessageBox.Show(textError);
            e.Handled = true;
        }
    }
}
