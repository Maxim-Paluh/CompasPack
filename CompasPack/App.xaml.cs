﻿using Autofac;
using CompasPack.Startup;
using CompasPack.View;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CompasPack
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindowView mainWindow = new Bootstrapper().Bootstrap().Resolve<MainWindowView>();
            mainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            List<Exception> exceptions = new List<Exception>();
            Exception exception = e.Exception;
            do
            {
                exceptions.Add(exception);
                exception = exception.InnerException;
            } while (exception != null);

            var Error = new ErrorsView(exceptions);
            Error.ShowDialog();
            e.Handled = true;
            Current.Shutdown();
        }
    }
}
