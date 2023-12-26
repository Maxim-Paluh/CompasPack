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
