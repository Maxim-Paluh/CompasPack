using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace CompasPack.ViewModel
{
    class LaptopMainViewModel : ReportViewModelBase, IReportViewModel, IDataErrorInfo
    {
        private string _brand;
        private string _model;
        private string _line;
        private KeyValuePair<string, List<string>> _lines;

        public LaptopMainViewModel(SettingsReportViewModel settingsReport)
        {
            SettingsReport = settingsReport;
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
                        if (string.IsNullOrWhiteSpace(Brand))
                            error = "Введи значення";
                        break;
                }
                return error;
            }
        }


        public KeyValuePair<string, List<string>> Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;
                OnPropertyChanged();
            }
        }
        public string Line
        {
            get { return _line; }
            set
            {
                _line = value;
                OnPropertyChanged();
                if(!string.IsNullOrWhiteSpace(_line))
                    Result = $"{_brand} {_line} {_model}";
                else
                    Result = $"{_brand} {_model}";
            }
        }
        public string Brand
        {
            get { return _brand; }
            set
            {
                _brand = value;
                OnPropertyChanged();
                if (!string.IsNullOrWhiteSpace(_line))
                    Result = $"{_brand} {_line} {_model}";
                else
                    Result = $"{_brand} {_model}";
            }
        }

        public string Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged();
                if (!string.IsNullOrWhiteSpace(_line))
                    Result = $"{_brand} {_line} {_model}";
                else
                    Result = $"{_brand} {_model}";
            }
        }

        public string Error => throw new NotImplementedException();

        public void Load()
        {
            //throw new NotImplementedException();
        }
    }
}
