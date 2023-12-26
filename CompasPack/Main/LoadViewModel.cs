using System;
using System.Threading.Tasks;

namespace CompasPack.ViewModel
{
    public class LoadViewModel : ViewModelBase, IDetailViewModel
    {
        public bool HasChanges()
        {
            throw new NotImplementedException();
        }

        public Task LoadAsync(int? Id)
        {
            return Task.CompletedTask;
        }

        public void Unsubscribe()
        {
           
        }
    }
}
