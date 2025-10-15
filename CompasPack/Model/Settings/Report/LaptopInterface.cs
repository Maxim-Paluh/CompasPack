using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Model.Settings
{
    public class LaptopInterface
    {
        public List<LaptopPort> LaptopPorts { get; set; }
        public string TestMicrophoneURL { get; set; }
        public string TestWebCamURL { get; set; }
    }
}
