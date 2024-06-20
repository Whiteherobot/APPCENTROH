using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using APPVETERINARIA.Models;
using APPVETERINARIA.Repositories;

namespace APPVETERINARIA.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        //Fields
        private UserAccountModel _currentUserAccount;
        private IUserRepository userRepository;

        public UserAccountModel CurrentUserAccount
        {
            get
            {
                return _currentUserAccount;
            }

            set
            {
                _currentUserAccount = value;
                OnPropertyChanged(nameof(CurrentUserAccount));
            }
        }

        public MainViewModel()
        {
            userRepository = new UserRepository();
            CurrentUserAccount = new UserAccountModel();
            LoadCurrentUserData();
        }

        private void LoadCurrentUserData()
        {
            var user = userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            if (user != null)
            {
                CurrentUserAccount.Username = user.Username;

                // Verificar si la propiedad "Name" contiene un valor y asignar el nombre en consecuencia
                if (!string.IsNullOrEmpty(user.Name))
                {
                    CurrentUserAccount.DisplayName = $"Bienvenido {user.Name} {user.LastName}";
                }
                else
                {
                    CurrentUserAccount.DisplayName = "Bienvenido";
                }

                CurrentUserAccount.ProfilePicture = null;
            }
            else
            {
                CurrentUserAccount.DisplayName = "Invalid user, not logged in";
                // Ocultar vistas secundarias.
            }
        }
    }
}
