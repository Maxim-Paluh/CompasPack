using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class MonitorOtherViewModel : ReportHardWareViewModelBase, IReportViewModel, IDataErrorInfo
    {
        private bool _miniVGA;

        public bool MyProperty
        {
            get { return _miniVGA; }
            set { _miniVGA = value; }
        }

        public MonitorOtherViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
            Document = xDocument;
            SelectInterfaceCommand = new DelegateCommand(OnSelectInterface);
        }

        private void OnSelectInterface()
        {
            Result = string.Join(", ", SettingsReport.Monitor.MonitorInterfaces.Where(x => x.IsSelect).Select(c => c.Name));
        }

        public void Load()
        {
            
        }

        public ICommand SelectInterfaceCommand { get; set; }

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "Result":
                        if (string.IsNullOrWhiteSpace(Result))
                            error = "Введи значення";
                        break;
                }
                return error;
            }
        }
    }
}
