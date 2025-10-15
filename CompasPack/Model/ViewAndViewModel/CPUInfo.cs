using System;

using CompasPack.ViewModel;

namespace CompasPack.Model.ViewAndViewModel
{
    public class CPUInfo : ViewModelBase
    {
        private string _name;
        private string _clock;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string Clock
        {
            get { return _clock; }
            set
            {
                _clock = value;
                OnPropertyChanged();
            }
        }
    }
}
