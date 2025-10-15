using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Model.Settings
{
    public class ReportBase
    {
        public string XPath { get; set; }
        public List<string> Regex { get; set; }
    }
}
