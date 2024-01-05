using CompasPack.Settings;
using System;
using System.ComponentModel;

namespace CompasPack.ViewModel
{
    public class PCCaseViewModel : ReportHardWareViewModelBase<ReportSettings>, IDataErrorInfo
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
                Result = $"Корпус \"{_name}\"";
            }
        }
        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "Name":
                        if (string.IsNullOrWhiteSpace(Name))
                            error = "Введи значення";
                        break;
                }
                return error;
            }
        }
        public string Error => throw new NotImplementedException();
        public PCCaseViewModel()
        {
            Name = "";
        }
    }
}
