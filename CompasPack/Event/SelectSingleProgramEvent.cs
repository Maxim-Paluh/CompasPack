using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPac.Event
{
    public class SelectSingleProgramEvent : PubSubEvent<SelectSingleProgramEventArgs>
    {
    }

    public class SelectSingleProgramEventArgs
    {
        public int IdProgram { get; set; }
        public int IdGroup { get; set; }
    }
}
