using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CompasPack.ViewModel
{
    class LaptopMainViewModel : ReportHardwareViewModelBase<Dictionary<string, List<string>>>, IDataErrorInfo
    {
        private string _brand;
        private string _line;
        private string _model;

        private List<string> _lines;
        public List<string> Lines
        {
            get => _lines;
            set
            {
                _lines = value;
                OnPropertyChanged();
            }
        }
        public string Brand
        {
            get { return _brand; }
            set
            {
                _brand = value;
                OnPropertyChanged();
                UpdateLines();
                SetResult();
            }
        }
        public string Line
        {
            get { return _line; }
            set
            {
                _line = value;
                OnPropertyChanged();
                SetResult();
            }
        }
        public string Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged();
                SetResult();
            }
        }
        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "Brand":
                        if (string.IsNullOrWhiteSpace(Brand))
                            error = "Введи значення";
                        break;
                    case "Model":
                        if (string.IsNullOrWhiteSpace(Model))
                            error = "Введи значення";
                        break;
                }
                return error;
            }
        }
        public string Error => throw new NotImplementedException();
        
        public void Load(Dictionary<string, List<string>> laptopsBrandAndModel)
        {
            Settings = laptopsBrandAndModel;
        }
        private void SetResult() 
        {
            Result = string.Join(" ", new[] { _brand, _line, _model }.Where(s => !string.IsNullOrWhiteSpace(s)));
        }
        private void UpdateLines()
        {
            Settings.TryGetValue(_brand, out var list);
            Lines = list ?? new List<string>();
        }
    }
}
