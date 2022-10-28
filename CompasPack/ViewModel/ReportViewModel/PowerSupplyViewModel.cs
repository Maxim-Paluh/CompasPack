using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.ViewModel
{
    public class PowerSupplyViewModel : ReportViewModelBase, IReportViewModel
    {
        private string _text;
        private string _power;

        public PowerSupplyViewModel(SettingsReportViewModel settingsReport)
        {
            SettingsReport = settingsReport;

        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
                Result = $"{_text} - {_power}W";
            }
        }

        public string Power
        {
            get { return _power; }
            set
            {
                _power = value;
                OnPropertyChanged();
                Result = $"{_text} - {_power}W";
            }
        }

        public void Load()
        {
            //throw new NotImplementedException();
        }
    }
}
