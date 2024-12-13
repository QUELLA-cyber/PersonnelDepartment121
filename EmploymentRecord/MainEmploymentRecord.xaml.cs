using PersonnelDepartment.ViewModel;
using System.Windows;

namespace PersonnelDepartment.EmploymentRecord
{
    public partial class MainEmploymentRecord : Window
    {
        private Personal_card _currentUser;

        public MainEmploymentRecord(Personal_card currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            var viewModel = new MainEmploymentRecordViewModel(_currentUser, this);
            DataContext = viewModel;
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
