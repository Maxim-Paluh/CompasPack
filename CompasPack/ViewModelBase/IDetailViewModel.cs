using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.ViewModelBase
{
    public interface IDetailViewModel
    {
        Task LoadAsync(int? Id);
        void Unsubscribe();
        bool HasChanges();
    }
}
