using System.Threading.Tasks;

namespace CompasPack.ViewModel
{
    public interface IViewModel
    {
        Task LoadAsync();
        void Unsubscribe();
        bool HasChanges();
    }
}
