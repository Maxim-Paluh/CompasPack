using CompasPack.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CompasPack
{
    public interface IReportViewModel
    {
        void Load();
    }
    public class ReportHardWareViewModelBase : ViewModelBase
    {
        private XDocument? _document;
        private SettingsReportViewModel _settingsReport;
        private string _result;
        public SettingsReportViewModel SettingsReport
        {
            get { return _settingsReport; }
            set
            {
                _settingsReport = value;
                OnPropertyChanged();
            }
        }
        public XDocument? Document
        {
            get { return _document; }
            set
            {
                _document = value;
                OnPropertyChanged();
            }
        }
        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }
    }
}
