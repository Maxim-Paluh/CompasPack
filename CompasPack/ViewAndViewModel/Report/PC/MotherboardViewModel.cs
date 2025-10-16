using CompasPack.Data.Providers;
using CompasPack.Model.Settings;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Linq;
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
            Name = xDocument.XPathSelectElement(Settings.XPath)?.Value ?? "Not found";
            Result = Settings.Regex.Aggregate(Name, (current, pattern) => Regex.Replace(current, pattern, "")).Trim();
        }
    }
}
