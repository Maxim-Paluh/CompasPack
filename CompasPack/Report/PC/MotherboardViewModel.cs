using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    public class MotherboardViewModel : ReportHardWareViewModelBase, IReportViewModel
    {
        private string _name;
        public MotherboardViewModel(SettingsReportViewModel settingsReport, XDocument xDocument)
        {
            SettingsReport = settingsReport;
            Document = xDocument;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public void Load()
        {
            var tempName = Document.XPathSelectElement(SettingsReport.Motherboard.XPath);
            if (tempName != null)
                Name = tempName.Value;
            else
                Name = "Not found";

            var tempResault = Name;
            foreach (var item in SettingsReport.Motherboard.Regex)
                tempResault = Regex.Replace(tempResault, item, "");

            Result = tempResault.Trim();
        }
    }
}
