using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    class LaptopMainViewModel :  ReportViewModelBase, IReportViewModel
    {
        private string _brand;
        private string _model;

        public LaptopMainViewModel(SettingsReportViewModel settingsReport)
        {
            SettingsReport = settingsReport;
        }

        public string Brand
        {
            get { return _brand; }
            set
            {
                _brand = value;
                OnPropertyChanged();
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
                Result = $"{_brand} {_model}";
            }
        }

        public void Load()
        {
            //throw new NotImplementedException();
        }
    }
}
