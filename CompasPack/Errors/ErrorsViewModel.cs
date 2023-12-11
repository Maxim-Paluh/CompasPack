using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CompasPack.ViewModel
{
    class ErrorsViewModel : ViewModelBase
    {
        public List<Exception> Exceptions {get; set;}
        private Exception _selectedException;

        public Exception SelectedException
        {
            get { return _selectedException; }
            set 
            { 
                _selectedException = value;
                OnPropertyChanged();
            }
        }
        
        public ErrorsViewModel(List<Exception> exceptions)
        {
            Exceptions = exceptions;
            SelectedException = Exceptions.FirstOrDefault();
            GetReportCommand = new DelegateCommand(OnGetReport);
        }

        private void OnGetReport()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<----------------------------------------------------------------------------------------->");
            foreach (Exception ex in Exceptions)
            {
                sb.AppendLine($"Message: {ex.Message}");
                sb.AppendLine($"StackTrace:");
                sb.AppendLine(ex.StackTrace);
                sb.AppendLine("<----------------------------------------------------------------------------------------->");
                Clipboard.SetText(sb.ToString());
            }
        }

        public ICommand GetReportCommand { get; }
    }
}
