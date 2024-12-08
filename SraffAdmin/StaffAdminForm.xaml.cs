using PersonnelDepartment.AdminDataSource;
using PersonnelDepartment.ClassHelper;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PersonnelDepartment.SraffAdmin
{
    /// <summary>
    /// Логика взаимодействия для StaffAdminForm.xaml
    /// </summary>
    public partial class StaffAdminForm : Window, INotifyPropertyChanged
    {
        private Employee _selectedEmployee;

        // Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Personal_card _currentUser; // Хранение данных текущего пользователя
        public ObservableCollection<Employee> Employees { get; set; }
        public ObservableCollection<Employee> FilteredEmployees { get; set; }
        public ObservableCollection<string> Roles { get; set; }

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee)); // Обновляем привязки, когда меняется выбранный сотрудник
            }
        }

        public StaffAdminForm(Personal_card currentUser)
        {
            Roles = LoadRoles();
            Employees = LoadEmployeesFromDatabase();

            InitializeComponent();
            
            
            FilteredEmployees = new ObservableCollection<Employee>(Employees); // Копия для фильтрации
            EmployeeListView1.ItemsSource = FilteredEmployees;// Загружаем данные при инициализации
            _currentUser = currentUser;
            DisplayUserInfo(); // Отображение информации о текущем пользователе

            DataContext = this;
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

        private ObservableCollection<Employee> LoadEmployeesFromDatabase()
        {
            ObservableCollection<Employee> employees = new ObservableCollection<Employee>();

            // Здесь ваш код для подключения к базе данных и выполнения SQL-запроса

            using (var connection = new SqlConnection("data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework"))
            {
                connection.Open();
                var command = new SqlCommand("SELECT p.ID, p.Name AS FirstName, p.Surname AS LastName, p.Patronymic, po.Title AS Position, d.Title AS Department, p.Telephone AS Phone, p.Date_of_birth AS BirthDate, p.Photo, p.Login, p.Password FROM Personal_card p JOIN Post po ON p.Id_post = po.ID JOIN Department d ON po.Id_department = d.ID;", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            ID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Patronymic = reader.GetString(3),
                            Position = reader.GetString(4),
                            Department = reader.GetString(5),
                            Phone = reader.GetString(6),
                            BirthDate = reader.GetDateTime(7),
                            PhotoData = reader.IsDBNull(8) ? null : (byte[])reader[8],
                            Login = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                            Password = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                        });
                    }
                }
            }

            return employees;
        }

        //private ObservableCollection<string> LoadPositions()
        //{
        //    ObservableCollection<string> positions = new ObservableCollection<string>();
        //    using (var connection = new SqlConnection("data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework"))
        //    {
        //        connection.Open();
        //        var command = new SqlCommand("SELECT TOP 2 Title FROM Post ORDER BY ID ASC;", connection);
        //        using (var reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                positions.Add(reader.GetString(0));
        //            }
        //        }
        //    }
        //    return positions;
        //}

        private void EmployeeListView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeListView1.SelectedItem is Employee selectedEmployee)
            {
                SelectedEmployee = selectedEmployee; // Устанавливаем выбранного сотрудника для отображения его данных
            }
        }

        private void FilterEmployees()
        {
            string searchText = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                // Если поле поиска пустое, показываем всех сотрудников
                FilteredEmployees.Clear();
                foreach (var emp in Employees)
                {
                    FilteredEmployees.Add(emp);
                }
            }
            else
            {
                // Фильтруем список сотрудников по имени, фамилии или их комбинации
                var filteredList = Employees.Where(emp =>
                    (!string.IsNullOrEmpty(emp.FirstName) && emp.FirstName.ToLower().Contains(searchText)) ||
                    (!string.IsNullOrEmpty(emp.LastName) && emp.LastName.ToLower().Contains(searchText)) ||
                    ($"{emp.FirstName} {emp.LastName}".ToLower().Contains(searchText))
                ).ToList();

                // Обновляем коллекцию для отображения
                FilteredEmployees.Clear();
                foreach (var emp in filteredList)
                {
                    FilteredEmployees.Add(emp);
                }
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEmployee == null) return;

            using (var connection = new SqlConnection("data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework"))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE Personal_card SET Login = @Login, Password = @Password, Id_post = (SELECT ID FROM Post WHERE Title = @Position) WHERE ID = @ID", connection);
                command.Parameters.AddWithValue("@Login", SelectedEmployee.Login);
                command.Parameters.AddWithValue("@Password", SelectedEmployee.Password);
                command.Parameters.AddWithValue("@Position", SelectedEmployee.Position);
                command.Parameters.AddWithValue("@ID", SelectedEmployee.ID);
                command.ExecuteNonQuery();
            }
            MessageBox.Show("Изменения сохранены");
        }

        private ObservableCollection<string> LoadRoles()
        {
            ObservableCollection<string> roles = new ObservableCollection<string>();
            using (var connection = new SqlConnection("data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework"))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Title FROM Post ORDER BY ID ASC;", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(reader.GetString(0));
                    }
                }
            }
            return roles;
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            FilterEmployees();
        }

        private void EmploymentRecordBtn_Click(object sender, RoutedEventArgs e)
        {
            DataSourceForm dataSourceForm = new DataSourceForm(_currentUser);
            dataSourceForm.Show();
            Close();
        }

        private void MainWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindowAdmin mainWindowAdmin = new MainWindowAdmin(_currentUser);
            mainWindowAdmin.Show();
            Close();
        }
    }
}
