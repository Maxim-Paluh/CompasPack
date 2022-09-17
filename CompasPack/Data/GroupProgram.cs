using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CompasPack.Data
{
    public class GroupProgram 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool SingleChoice { get; set; }
        public List<UserProgram> UserPrograms { get; set; }
    }
}
