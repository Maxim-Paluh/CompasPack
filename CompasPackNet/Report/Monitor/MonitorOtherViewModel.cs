using CompasPack.Settings;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class MonitorOtherViewModel : ReportHardWareViewModelBase<MonitorReportSettings>, IReportViewModel, IDataErrorInfo
    {
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
        public string Error => throw new NotImplementedException();
        public MonitorOtherViewModel(MonitorReportSettings monitorReportSettings, XDocument xDocument)
        {
            Settings = monitorReportSettings;
            Document = xDocument;
            SelectInterfaceCommand = new DelegateCommand(OnSelectInterface);
        }
        public void Load()
        {

        }
        private void OnSelectInterface()
        {
            Result = string.Join(", ", Settings.MonitorInterfaces.Where(x => x.IsSelect).Select(c => c.Name));
        }
        public ICommand SelectInterfaceCommand { get; set; }
    }
}
