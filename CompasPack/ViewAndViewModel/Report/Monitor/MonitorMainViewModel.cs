using CompasPack.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using CompasPack.Service;

namespace CompasPack.ViewModel
{
    public class MonitorMainViewModel : ReportHardWareViewModelBase<Monitor>, IReportViewModel, IDataErrorInfo
    {
        private string _brand;
        private string _model;
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Brand
        {
            get { return _brand; }
            set
            {
                _brand = value;
                OnPropertyChanged();
                Result = $"{Brand} {Model}".Trim();
            }
        }
        public string Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged();
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
                }
                return error;
            }
        }
        public string Error => throw new NotImplementedException();
        public MonitorMainViewModel(Monitor monitorReportSettings, XDocument xDocument)
        {
            Settings = monitorReportSettings;
            Document = xDocument;
        }
        public void Load()
        {
            var tempName = Document.XPathSelectElement(Settings.MonitorName.XPath);
            if (tempName != null)
                Name = tempName.Value;
            else
                Name = string.Empty;
            
            var tempModel = Document.XPathSelectElement(Settings.MonitorModel.XPath);
            if (tempModel != null)
                Model = tempModel.Value;
            else
                Model = string.Empty;


            if (!string.IsNullOrWhiteSpace(Name))
                Brand = Settings.MonitorsBrand.FirstOrDefault(x => Name.Contains(x, StringComparison.InvariantCultureIgnoreCase));

            if(string.IsNullOrWhiteSpace(Brand))
                if(!string.IsNullOrWhiteSpace(Model))
                    Brand = Settings.MonitorsBrand.FirstOrDefault(x => Model.Contains(x, StringComparison.InvariantCultureIgnoreCase));

            if(!string.IsNullOrWhiteSpace(Brand))
            if (Model.Contains(Brand))
                Model = Model.Replace(Brand, "").Trim();

            Result = $"{Brand} {Model}".Trim();
        }
    }
}
