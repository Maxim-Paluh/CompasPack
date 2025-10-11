using CompasPack.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CompasPack.View
{
    /// <summary>
    /// Логика взаимодействия для ErrorView.xaml
    /// </summary>
    public partial class ErrorsView : Window
    {
        private ErrorsViewModel _errorsViewModel;

        public ErrorsView(List<Exception> exception)
        {
            InitializeComponent();
            _errorsViewModel = new ErrorsViewModel(exception);
            DataContext = _errorsViewModel;
        }

    }
}
