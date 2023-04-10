using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace CompasPack.ViewModel
{
    public class PCCaseViewModel : ReportViewModelBase, IDataErrorInfo
    {
        private string _name;
        public PCCaseViewModel()
        {
            Name = "";
        }

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "Name":
                        if(string.IsNullOrWhiteSpace(Name))
                            error = "Введи значення";
                        break;
                }
                return error;
            }
        }

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

        public string Error => throw new NotImplementedException();
    }
}
