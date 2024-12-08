using PersonnelDepartment.ClassHelper;
using PersonnelDepartment.ControlClasses;
using PersonnelDepartment.EmploymentRecord;
using PersonnelDepartment.OrdersForm;
using PersonnelDepartment.RepostsForm;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PersonnelDepartment.Staff
{
    public partial class EditStaff : Window
    {
        private HumanResourcesDepartmentEntities dbModel;
        private Employee _employee;
        private Personal_card _currentUser;
        public event EventHandler EmployeeUpdated;
        public event EventHandler EmployeeDeleted;

        public EditStaff(Employee employee, Personal_card currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            
            _employee = employee;
            LoadDepartmentTitles();
            LoadPostTitles();
            DisplayUserInfo();
            LoadEducationLevels(); // Загружаем уровни образования

            if (_employee != null)
            {
                LoadEmployeeData(_employee.ID); // Загружаем данные сотрудника
            }

            LoadDepartmentTitles();
            LoadPostTitles();         
        }

        private void DisplayUserInfo()
        {
            if (_currentUser != null)
            {
                Namelbl.Content = $"{_currentUser.Surname} {_currentUser.Name}";
                Rolelbl.Content = _currentUser.Post.Title;
            }
        }

        private void LoadDepartmentTitles()
        {
            dbModel = new HumanResourcesDepartmentEntities();
            var departmentTitles = dbModel.Department.Select(d => d.Title).ToList();
            DepartmantCmb.ItemsSource = departmentTitles;
        }

        private void LoadPostTitles()
        {
            dbModel = new HumanResourcesDepartmentEntities();
            var postTitles = dbModel.Post
                .Select(p => new { p.ID, p.Title })
                .ToList();

            PostCmd.ItemsSource = postTitles;
            PostCmd.DisplayMemberPath = "Title";       // Устанавливаем отображаемое значение
            PostCmd.SelectedValuePath = "ID";          // Устанавливаем идентификатор для выбора
        }



        private void LoadEmployeeData(int employeeId)
        {
            var employeeData = AppConnect.dbModel.Personal_card
        .Include(e => e.Education)
        .Include("Post.Department")
        .FirstOrDefault(e => e.ID == employeeId);

            if (employeeData != null)
            {
                // Личные данные
                SurnameTextBox.Text = employeeData.Surname;
                FirstNameTextBox.Text = employeeData.Name;
                PatronymicTextBox.Text = employeeData.Patronymic;
                DateOfBirthPicker.SelectedDate = employeeData.Date_of_birth;
                BirthplaceTextBox.Text = employeeData.Birthplace;
                RegistrationAddressTextBox.Text = employeeData.Registration_address;

                // Привязка Email
                EmailTextBox.Text = employeeData.Email;

                // Контактные данные
                TelephoneTextBox.Text = employeeData.Telephone;

                // Устанавливаем выбранный отдел и загружаем должности для него
                if (employeeData.Post?.Department != null)
                {
                    DepartmantCmb.SelectedItem = employeeData.Post.Department.Title;
                    LoadPostsForDepartment(employeeData.Post.Department.Title);
                }

                // Устанавливаем выбранную должность
                if (employeeData.Post != null)
                {
                    PostCmd.SelectedValue = employeeData.Post.ID;
                }

                // Устанавливаем уровень образования
                if (employeeData.Education != null)
                {
                    EducationLevelComboBox.SelectedValue = employeeData.Id_education;
                }

                // Привязка учебного заведения
                EducationInstitutionTextBox.Text = employeeData.EducationInstitution;

                // Паспортные данные
                if (!string.IsNullOrEmpty(employeeData.Series_and_number))
                {
                    if (employeeData.Series_and_number.Length >= 4)
                    {
                        PassportSeriesTextBox.Text = employeeData.Series_and_number.Substring(0, 4);
                        PassportNumberTextBox.Text = employeeData.Series_and_number.Substring(4);
                    }
                    else
                    {
                        PassportSeriesTextBox.Text = employeeData.Series_and_number;
                        PassportNumberTextBox.Text = string.Empty;
                    }
                }
                else
                {
                    PassportSeriesTextBox.Text = string.Empty;
                    PassportNumberTextBox.Text = string.Empty;
                }

                PassportIssuedByTextBox.Text = employeeData.Issued_by_whom;
                DateOfIssuePicker.SelectedDate = employeeData.Date_of_issue;

                // Прочие данные
                ChildrenRadioButton.IsChecked = employeeData.Children;
                MilitaryServiceRadioButton.IsChecked = employeeData.Military_service;

                // Фото сотрудника
                if (employeeData.Photo != null && employeeData.Photo.Length > 0)
                {
                    BitmapImage bitmap = new BitmapImage();
                    using (MemoryStream stream = new MemoryStream(employeeData.Photo))
                    {
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                    }
                    AvatarImage.Source = bitmap;
                }
                else
                {
                    AvatarImage.Source = null;
                }
            }
            else
            {
                MessageBox.Show("Сотрудник с таким ID не найден.");
            }
        }

        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainReports mainReportswindow = new MainReports(_currentUser);
            mainReportswindow.Show();
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            MainStaff mainStaff = new MainStaff(_currentUser);
            mainStaff.Show();
            Close();
        }



        private void SaveEditBtn_Click(object sender, RoutedEventArgs e)
        {
            

            var employeeData = AppConnect.dbModel.Personal_card.FirstOrDefault(x => x.ID == _employee.ID);

            if (employeeData != null)
            {
                // Личные данные
                employeeData.Surname = SurnameTextBox.Text;
                employeeData.Name = FirstNameTextBox.Text;
                employeeData.Patronymic = PatronymicTextBox.Text;
                employeeData.Date_of_birth = DateOfBirthPicker.SelectedDate ?? DateTime.MinValue;
                employeeData.Birthplace = BirthplaceTextBox.Text;
                employeeData.Registration_address = RegistrationAddressTextBox.Text;

                // Контактные данные
                employeeData.Telephone = TelephoneTextBox.Text;
                employeeData.Email = EmailTextBox.Text;

                // Обновление изображения, если оно было изменено
                if (_updatedPhoto != null)
                {
                    employeeData.Photo = _updatedPhoto;
                }

                // Сохранение изменений
                AppConnect.dbModel.SaveChanges();

                // Трудовые данные: проверка типа SelectedValue и приведение к int
                if (PostCmd.SelectedValue != null)
                {
                    if (PostCmd.SelectedValue is int selectedPostId)
                    {
                        employeeData.Id_post = selectedPostId;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка: SelectedValue не является корректным ID.");
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка: SelectedValue является null.");
                }

                // Образование
                if (EducationLevelComboBox.SelectedValue is int selectedEducationId)
                {
                    employeeData.Id_education = selectedEducationId;
                }
                employeeData.EducationInstitution = EducationInstitutionTextBox.Text;

                // Паспортные данные
                employeeData.Series_and_number = PassportSeriesTextBox.Text + PassportNumberTextBox.Text;
                employeeData.Issued_by_whom = PassportIssuedByTextBox.Text;
                employeeData.Date_of_issue = DateOfIssuePicker.SelectedDate ?? DateTime.MinValue;

                // Прочие данные
                employeeData.Children = ChildrenRadioButton.IsChecked ?? false;
                employeeData.Military_service = MilitaryServiceRadioButton.IsChecked ?? false;

                // Сохраняем изменения
                AppConnect.dbModel.SaveChanges();
                // Вывод сообщения об успешном сохранении
                MessageBox.Show("Данные успешно сохранены.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Сообщение, если данные сотрудника не найдены
                MessageBox.Show("Сотрудник с таким ID не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void LoadEducationLevels()
        {
            // Загружаем все уровни образования из таблицы Education
            var educationLevels = AppConnect.dbModel.Education
                .Select(e => new { e.ID, e.Levell }) // Получаем Id и Levell
                .ToList();

            EducationLevelComboBox.ItemsSource = educationLevels;
            EducationLevelComboBox.DisplayMemberPath = "Levell"; // Указываем, что будет отображаться
            EducationLevelComboBox.SelectedValuePath = "ID";     // Указываем идентификатор для выбора
        }

        private void DepartmantCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DepartmantCmb.SelectedItem != null)
            {
                string selectedDepartment = DepartmantCmb.SelectedItem.ToString();
                LoadPostsForDepartment(selectedDepartment);
            }
        }
        private void LoadPostsForDepartment(string departmentTitle)
        {
            var department = dbModel.Department.FirstOrDefault(d => d.Title == departmentTitle);
            if (department != null)
            {
                var postTitles = dbModel.Post
                    .Where(p => p.Id_department == department.ID) // Фильтруем должности по выбранному отделу
                    .Select(p => new { p.ID, p.Title })
                    .ToList();

                PostCmd.ItemsSource = postTitles;
                PostCmd.DisplayMemberPath = "Title";
                PostCmd.SelectedValuePath = "ID";
            }
        }

        private void MainWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindowInspector mainWindowInspector = new MainWindowInspector(_currentUser);
            mainWindowInspector.Show();
            Close();
        }

        private void EmploymentRecordBtn_Click(object sender, RoutedEventArgs e)
        {
            MainEmploymentRecord mainEmploymentRecord = new MainEmploymentRecord(_currentUser);
            mainEmploymentRecord.Show();
            Close();
        }

        private void OrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            MainOrders mainOrderswindow = new MainOrders(_currentUser);
            mainOrderswindow.Show();
            Close();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уверены, что хотите выйти из приложения?",
                                                          "Подтверждение о закрытии",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning);
            Close();
        }

        private void DeleteEmployee(int employeeId)
        {
            using (var transaction = AppConnect.dbModel.Database.BeginTransaction())
            {
                try
                {
                    // Удаляем записи из таблицы Salary, связанные с сотрудником
                    var salariesToDelete = AppConnect.dbModel.Salary
                                            .Where(s => s.Id_personal_card == employeeId).ToList();
                    AppConnect.dbModel.Salary.RemoveRange(salariesToDelete);

                    // Удаляем записи из таблицы Entry_in_the_work_book, связанные с сотрудником
                    var entriesToDelete = AppConnect.dbModel.Entry_in_the_work_book
                                            .Where(e => e.Id_personal_card == employeeId).ToList();
                    AppConnect.dbModel.Entry_in_the_work_book.RemoveRange(entriesToDelete);


                    // Удаляем запись о сотруднике из Personal_card
                    var employeeToDelete = AppConnect.dbModel.Personal_card
                                            .FirstOrDefault(e => e.ID == employeeId);

                    if (employeeToDelete != null)
                    {
                        AppConnect.dbModel.Personal_card.Remove(employeeToDelete);
                    }
                    else
                    {
                        throw new Exception("Сотрудник с таким ID не найден.");
                    }

                    // Сохраняем изменения в базе данных
                    AppConnect.dbModel.SaveChanges();
                    transaction.Commit();

                    // Уведомляем об успешном удалении
                    MessageBox.Show("Данные сотрудника успешно удалены.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                    EmployeeDeleted?.Invoke(this, EventArgs.Empty);
                    MainStaff mainStaff = new MainStaff(_currentUser);
                    Close();
                    mainStaff.Show();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Ошибка при удалении данных сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_employee != null)
            {
                DeleteEmployee(_employee.ID);
            }
            else
            {
                MessageBox.Show("Сотрудник не выбран.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Поле для хранения измененного изображения
        private byte[] _updatedPhoto;

        private void ChangeImage_Click(object sender, RoutedEventArgs e)
        {
            // Открытие диалогового окна для выбора изображения
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Загрузка изображения в элемент AvatarImage
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                AvatarImage.Source = bitmap;

                // Преобразование изображения в массив байтов для сохранения
                using (MemoryStream stream = new MemoryStream())
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(stream);

                    // Сохраняем байты в поле _updatedPhoto
                    _updatedPhoto = stream.ToArray();
                }
            }
        }
    }
}
