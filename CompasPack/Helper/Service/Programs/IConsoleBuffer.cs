using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public interface IConsoleBuffer
    {
        string Text { get; set; }
        void WriteLine(string message);
        void AddSplitter();
    }
}
