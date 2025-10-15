using System;

using CompasPack.ViewModel;

namespace CompasPack.Model.ViewAndViewModel
{
    public class DiskInfo : ViewModelBase
    {
        private bool _isSelect;
        public bool IsSelect
        {
            get { return _isSelect; }
            set
            {
                _isSelect = value;
                OnPropertyChanged();
            }
        }
        public int Order { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
    }
}
