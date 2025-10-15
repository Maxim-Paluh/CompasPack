using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CompasPack.ViewModel
{
    public class LoadViewModel : ViewModelBase, IViewModel, IViewModelReport
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

        public Task LoadAsync()
        {
            throw new NotImplementedException();
        }

        public Task LoadAsync(XDocument _xDocument)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
