using PersonnelDepartment.Commands;
using PersonnelDepartment.EmploymentRecord;
using PersonnelDepartment.OrdersForm;
using PersonnelDepartment.RepostsForm;
using PersonnelDepartment.Staff;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PersonnelDepartment.ViewModel
{
    public class MainWindowInspectorViewModel : BaseViewModel
    {
        private Personal_card _currentUser;
        private string _userName;
        private string _userRole;
        private BitmapImage _userImage;
        private Window _currentWindow;

        public MainWindowInspectorViewModel(Personal_card currentUser, Window currentWindow)
        {
            _currentUser = currentUser;
            _currentWindow = currentWindow;
            UserName = $"{currentUser.Surname} {currentUser.Name}";
            UserRole = currentUser.Post.Title;

            if (currentUser.Photo != null)
            {
                UserImage = ByteArrayToBitmapImage(currentUser.Photo);
            }

            StaffCommand = new RelayCommand(ExecuteStaff);
            EmploymentRecordCommand = new RelayCommand(ExecuteEmploymentRecord);
            OrdersCommand = new RelayCommand(ExecuteOrders);
            ReportCommand = new RelayCommand(ExecuteReports);
            ExitCommand = new RelayCommand(ExecuteExit);
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        public string UserRole
        {
            get => _userRole;
            set
            {
                _userRole = value;
                OnPropertyChanged(nameof(UserRole));
            }
        }

        public BitmapImage UserImage
        {
            get => _userImage;
            set
            {
                _userImage = value;
                OnPropertyChanged(nameof(UserImage));
            }
        }

        public ICommand StaffCommand { get; }
        public ICommand EmploymentRecordCommand { get; }
        public ICommand OrdersCommand { get; }
        public ICommand ReportCommand { get; }
        public ICommand ExitCommand { get; }

        private BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            if (byteArray == null) return null;

            using var stream = new System.IO.MemoryStream(byteArray);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            return image;
        }

        private void ExecuteStaff()
        {
            MainStaff mainStaffWindow = new MainStaff(_currentUser);
            mainStaffWindow.Show();
            _currentWindow.Close();
        }

        private void ExecuteEmploymentRecord()
        {
            MainEmploymentRecord mainEmploymentRecordWindow = new MainEmploymentRecord(_currentUser);
            mainEmploymentRecordWindow.Show();
            _currentWindow.Close();
        }

        private void ExecuteOrders()
        {
            MainOrders mainOrdersWindow = new MainOrders(_currentUser);
            mainOrdersWindow.Show();
            _currentWindow.Close();
        }

        private void ExecuteReports()
        {
            MainReports mainReportsWindow = new MainReports(_currentUser);
            mainReportsWindow.Show();
            _currentWindow.Close();
        }

        private void ExecuteExit()
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из приложения?",
                                         "Подтверждение о закрытии",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
