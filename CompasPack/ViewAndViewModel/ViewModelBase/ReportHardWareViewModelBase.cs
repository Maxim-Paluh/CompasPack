using System.Xml.Linq;

namespace CompasPack
{
    public interface IReportViewModel
    {
        void Load();
    }
    public class ReportHardWareViewModelBase<T> : ViewModelBase
    {
        private XDocument _document;
        private T _settings;
        private string _result;
        public T Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }
        public XDocument Document
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
