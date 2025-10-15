using Prism.Events;

namespace CompasPack.Helper.Event
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
