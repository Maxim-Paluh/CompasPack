using CompasPack.Data.Providers;
using CompasPack.Helper.Extension;
using CompasPack.Model.Settings;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    public class MonitorMainViewModel : ReportHardwareViewModelBase<Monitor>, IDataErrorInfo
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
        public MonitorMainViewModel(ReportSettingsProvider reportSettingsProvider)
        {
            Settings = reportSettingsProvider.Settings.Monitor;
        }
        public void Load(XDocument xDocument)
        {
            Name = xDocument.XPathSelectElement(Settings.MonitorName.XPath)?.Value ?? string.Empty;
            Model = xDocument.XPathSelectElement(Settings.MonitorModel.XPath)?.Value ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(Name))
                Brand = Settings.MonitorsBrand.FirstOrDefault(x => Name.Contains(x, StringComparison.InvariantCultureIgnoreCase)); // шукаємо бренд в Name, якщо Name не пустий

            if (string.IsNullOrWhiteSpace(Brand)) // ідемо далі лише якщо не знайшли модель
                if (!string.IsNullOrWhiteSpace(Model)) // шукаємо бренд в Model, якщо Model не пустий
                    Brand = Settings.MonitorsBrand.FirstOrDefault(x => Model.Contains(x, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(Brand) && Model.Contains(Brand)) // якщо в Model міститься назва бренду то ми її видалимо, щоб не дублювати цю інформацію в Result
                Model = Model.Replace(Brand, "").Trim();

            Result = $"{Brand} {Model}".Trim();

        }
    }
}
