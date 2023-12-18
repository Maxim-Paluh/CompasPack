using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompasPack.ViewModel
{
    public class LoadViewModel : ViewModelBase, IDetailViewModel
    {
        public bool HasChanges()
        {
            throw new NotImplementedException();
        }

        public async Task LoadAsync(int? Id)
        {
            
        }

        public void Unsubscribe()
        {
           
        }
    }
}
