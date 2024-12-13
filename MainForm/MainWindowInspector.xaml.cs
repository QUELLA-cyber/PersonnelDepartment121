using PersonnelDepartment.ViewModel;
using System.Windows;

namespace PersonnelDepartment
{

    public partial class MainWindowInspector : Window
    {
        private Personal_card _currentUser;

        public MainWindowInspector(Personal_card currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            var viewModel = new MainWindowInspectorViewModel(_currentUser, this);
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
