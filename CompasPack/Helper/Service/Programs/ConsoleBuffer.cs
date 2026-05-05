using CompasPack.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.Helper.Service
{
    public class ConsoleBuffer : ViewModelBase, IConsoleBuffer
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }
        public void WriteLine(string message)
        {
            Text += message;
        }

        public void AddSplitter()
        {
            string[] strings = Text.Split('\n');
            if (string.IsNullOrWhiteSpace(strings.Last()))
            {
                if (!strings[strings.Length - 2].Contains("<----------------------------------------------------------------------------->"))
                    Text += "<----------------------------------------------------------------------------->\n";
            }
        }
    }
}
