using PersonnelDepartment.AdminDataSource;
using PersonnelDepartment.SraffAdmin;
using System.Windows;

namespace PersonnelDepartment
{
    public partial class MainWindowAdmin : Window
    {
        private Personal_card _currentUser;
        public MainWindowAdmin(Personal_card currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            DisplayUserInfo();
        }

        public MainWindowAdmin()
        {
            InitializeComponent();
        }

        private void DisplayUserInfo()
        {
            if (_currentUser != null)
            {
                Namelbl.Content = $"{_currentUser.Surname} {_currentUser.Name}";
                Rolelbl.Content = _currentUser.Post.Title;
            }
            else
            {
                MessageBox.Show("Сотрудник не найден");
            }
        }

        private void StaffBtn_Click(object sender, RoutedEventArgs e)
        {
            StaffAdminForm staffAdminForm = new StaffAdminForm(_currentUser);
            staffAdminForm.Show();
            Close();
        }

        private void DataSourceBtn_Click(object sender, RoutedEventArgs e)
        {
            DataSourceForm dataSourceForm = new DataSourceForm(_currentUser);
            dataSourceForm.Show();
            Close();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            // Показать окно подтверждения
            MessageBoxResult result = MessageBox.Show(
                "Вы действительно хотите выйти?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            // Проверка ответа пользователя
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown(); // Закрытие приложения
            }
            // Если пользователь нажал "Нет", ничего не делаем
        }
    }
}
