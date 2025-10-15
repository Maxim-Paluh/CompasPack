using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public interface IViewModelReport
    {
        Task LoadAsync(XDocument _xDocument);
        bool HasChanges();
    }
}
