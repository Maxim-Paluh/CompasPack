using CompasPack.Settings;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CompasPack.ViewModel
{
    public class MotherboardViewModel : ReportHardWareViewModelBase<Motherboard>, IReportViewModel
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
        public MotherboardViewModel(Motherboard motherboardReportSettings, XDocument xDocument)
        {
            Settings = motherboardReportSettings;
            Document = xDocument;
        }
        public void Load()
        {
            var tempName = Document.XPathSelectElement(Settings.XPath);
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
