using CompasPack.Data.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CompasPack.ViewModel
{
    public class PowerSupplyViewModel : ReportHardwareViewModelBase<List<string>>, IDataErrorInfo
    {
        private string _name;
        private string _power;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
                if (!string.IsNullOrWhiteSpace(_power))
                    Result = $"{_name}-{_power}W";
                else
                    Result = $"{_name}";
            }
        }
        public string Power
        {
            get { return _power; }
            set
            {
                _power = value;
                OnPropertyChanged();
                if(!string.IsNullOrWhiteSpace(_power))
                    Result = $"{_name}-{_power}W";
                else
                    Result = $"{_name}";
            }
        }
        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case "Name":
                        if (string.IsNullOrWhiteSpace(Name))
                            error = "Введи значення";
                        break;
                    case "Power":
                        if (string.IsNullOrWhiteSpace(Power))
                            error = "Введи значення";
                        else if (!Power.All(char.IsDigit))
                            error = "Введи числове значення";
                        break;
                }
                return error;
            }
        }
        public string Error => throw new NotImplementedException();
        public PowerSupplyViewModel(ReportSettingsProvider reportSettingsProvider)
        {
            Settings = reportSettingsProvider.Settings.PCPowerSupply;
        }
        public void Load()
        {

        }
    }
}
