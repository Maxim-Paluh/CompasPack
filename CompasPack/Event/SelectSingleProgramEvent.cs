using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Event
{
    public class SelectSingleProgramEvent : PubSubEvent<SelectSingleProgramEventArgs>
    {
    }

    public class SelectSingleProgramEventArgs
    {
        public string NameProgram { get; set; }
        public string NameGroup { get; set; }
    }
}
