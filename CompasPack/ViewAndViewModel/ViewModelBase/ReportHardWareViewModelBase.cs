using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class ReportHardwareViewModelBase<T> : ViewModelBase
    {
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
