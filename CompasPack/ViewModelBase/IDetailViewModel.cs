using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack
{
    public interface IDetailViewModel
    {
        Task LoadAsync(int? Id);
        void Unsubscribe();
        bool HasChanges();
    }
}
