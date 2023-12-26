using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CompasPack.ViewModel
{
    public class PowerSupplyViewModel : ReportHardWareViewModelBase<List<string>>, IReportViewModel, IDataErrorInfo
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
                Result = $"{_name}-{_power}W";
            }
        }
        public string Power
        {
            get { return _power; }
            set
            {
                _power = value;
                OnPropertyChanged();
                Result = $"{_name}-{_power}W";
            }
        }
        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "Text":
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
        public PowerSupplyViewModel(List<string> PCPowerSupply)
        {
            Settings = PCPowerSupply;

        }
        public void Load()
        {
        }
    }
}
