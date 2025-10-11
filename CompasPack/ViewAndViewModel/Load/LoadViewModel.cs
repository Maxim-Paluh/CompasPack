using System;
using System.Threading.Tasks;

namespace CompasPack.ViewModel
{
    public class LoadViewModel : ViewModelBase, IDetailViewModel
    {
        private string _message;

        public string Message
        {
            get { return _message; }
            set 
            {
                _message = value;
                OnPropertyChanged();
            }
        }
        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public LoadViewModel()
        {
            IsActive = true;
        }
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
