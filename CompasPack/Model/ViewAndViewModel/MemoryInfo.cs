using System;

using CompasPack.ViewModel;

namespace CompasPack.Model.ViewAndViewModel
{
    public class MemoryInfo : ViewModelBase
    {
        private string _type;
        private string _size;
        private string _frequency;
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }
        public string Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged();
            }
        }
        public string Frequency
        {
            get { return _frequency; }
            set
            {
                _frequency = value;
                OnPropertyChanged();
            }
        }
    }
}
