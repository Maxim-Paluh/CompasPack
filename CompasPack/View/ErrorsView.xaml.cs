﻿using CompasPack.ViewModel;
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

namespace CompasPack.View
{
    /// <summary>
    /// Логика взаимодействия для ErrorView.xaml
    /// </summary>
    public partial class ErrorsView : Window
    {
        private ErrorsViewModel? _errorsViewModel;

        public ErrorsView(List<Exception> exception)
        {
            InitializeComponent();
            _errorsViewModel = new ErrorsViewModel(exception);
            DataContext = _errorsViewModel;
        }

    }
}
