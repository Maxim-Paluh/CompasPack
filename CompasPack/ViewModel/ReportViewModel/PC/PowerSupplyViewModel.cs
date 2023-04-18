using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class PowerSupplyViewModel : ReportHardWareViewModelBase, IReportViewModel, IDataErrorInfo
    {
        private string _text;
        private string _power;

        public PowerSupplyViewModel(SettingsReportViewModel settingsReport)
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
                    case "Text":
                        if (string.IsNullOrWhiteSpace(Text))
                            error = "Введи значення";
                        break;
                    case "Power":
                        if (string.IsNullOrWhiteSpace(Power))
                            error = "Введи значення";
                        else if(!Power.All(char.IsDigit))
                            error = "Введи числове значення";
                        break;
                }
                return error;
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
                Result = $"{_text}-{_power}W";
            }
        }

        public string Power
        {
            get { return _power; }
            set
            {
                _power = value;
                OnPropertyChanged();
                Result = $"{_text}-{_power}W";
            }
        }

        public string Error => throw new NotImplementedException();

        public void Load()
        {
            //throw new NotImplementedException();
        }
    }
}
