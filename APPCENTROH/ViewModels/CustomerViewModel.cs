using APPCENTROH.Models;
using APPCENTROH.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace APPCENTROM.ViewModels
{
    public class CustomerViewModel : INotifyPropertyChanged
    {
        private readonly IUserRepository _userRepository;
        private ObservableCollection<UserModel> _users;

        public ObservableCollection<UserModel> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                OnPropertyChanged("Users");
            }
        }

        public ICommand UpdateCommand { get; private set; }

        public CustomerViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            Users = new ObservableCollection<UserModel>();
            UpdateCommand = new RelayCommand(UpdateData);
        }

        private void UpdateData(object parameter)
        {
            var usersFromRepo = _userRepository.GetAllUsers();
            Users.Clear();
            foreach (var user in usersFromRepo)
            {
                Users.Add(user);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
