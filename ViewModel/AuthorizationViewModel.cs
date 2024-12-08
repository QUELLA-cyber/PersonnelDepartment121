using PersonnelDepartment.Commands;
using PersonnelDepartment.ControlClasses;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PersonnelDepartment.ViewModel
{
    public class AuthorizationViewModel : BaseViewModel
    {
        private string _login;
        private string _password;
        private ICommand _loginCommand;

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged(nameof(Login));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand LoginCommand => _loginCommand ??= new RelayCommand(ExecuteLogin);

        private void ExecuteLogin()
        {
            var user = AppConnect.dbModel.Personal_card
                .FirstOrDefault(p => p.Login == Login && p.Password == Password);

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден!");
                return;
            }

            if (user.Post.ID == 1)
            {
                var mainWindow = new MainWindowAdmin(user);
                mainWindow.Show();
                Application.Current.MainWindow.Close();
            }
            else if (user.Post.ID == 2)
            {
                var mainWindow = new MainWindowInspector(user);
                mainWindow.Show();
                Application.Current.MainWindow.Close();
            }
        }
    }
}
