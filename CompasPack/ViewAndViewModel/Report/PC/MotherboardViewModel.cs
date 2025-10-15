using CompasPack.Data.Providers;
using CompasPack.Model.Settings;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    public class MotherboardViewModel : ReportHardwareViewModelBase<Motherboard>
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public MotherboardViewModel(ReportSettingsProvider reportSettingsProvider)
        {
            Settings = reportSettingsProvider.Settings.Motherboard;
        }
        public void Load(XDocument xDocument)
        {
            var tempName = xDocument.XPathSelectElement(Settings.XPath);
            if (tempName != null)
                Name = tempName.Value;
            else
                Name = "Not found";

            var tempResault = Name;
            foreach (var item in Settings.Regex)
                tempResault = Regex.Replace(tempResault, item, "");

            Result = tempResault.Trim();
        }
    }
}
