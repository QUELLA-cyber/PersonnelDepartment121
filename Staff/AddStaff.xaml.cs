using PersonnelDepartment.ClassHelper;
using PersonnelDepartment.EmploymentRecord;
using PersonnelDepartment.OrdersForm;
using PersonnelDepartment.RepostsForm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PersonnelDepartment.Staff
{
    /// <summary>
    /// Логика взаимодействия для AddStaff.xaml
    /// </summary>
    public partial class AddStaff : Window
    {
        private Personal_card _currentUser; // Хранение данных текущего пользователя
        public AddStaff(Personal_card currentUser)
        {
            _currentUser = currentUser;
            InitializeComponent();
            LoadComboBoxData();
            
            DisplayUserInfo(); // Отображение информации о текущем пользователе
        }

        private void DisplayUserInfo()
        {
            if (_currentUser != null)
            {
                Namelbl.Content = $"{_currentUser.Surname} {_currentUser.Name}";
                Rolelbl.Content = _currentUser.Post.Title;
            }
        }

        private void LoadComboBoxData()
        {
            string connectionString = "data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Заполнение citizenshipComboBox
                string queryCitizenship = "SELECT ID, Country FROM Citizenship";
                using (SqlCommand command = new SqlCommand(queryCitizenship, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var citizenshipList = new List<KeyValuePair<int, string>>();
                    while (reader.Read())
                    {
                        citizenshipList.Add(new KeyValuePair<int, string>((int)reader["ID"], (string)reader["Country"]));
                    }
                    citizenshipComboBox.ItemsSource = citizenshipList;
                    citizenshipComboBox.DisplayMemberPath = "Value";
                    citizenshipComboBox.SelectedValuePath = "Key";
                }

                // Заполнение departmentComboBox
                string queryDepartment = "SELECT ID, Title FROM Department";
                using (SqlCommand command = new SqlCommand(queryDepartment, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var departmentList = new List<KeyValuePair<int, string>>();
                    while (reader.Read())
                    {
                        departmentList.Add(new KeyValuePair<int, string>((int)reader["ID"], (string)reader["Title"]));
                    }
                    departmentComboBox.ItemsSource = departmentList;
                    departmentComboBox.DisplayMemberPath = "Value";
                    departmentComboBox.SelectedValuePath = "Key";
                }

                // Загрузка всех должностей в postComboBox для дальнейшего фильтра
                string queryPost = "SELECT ID, Title, Id_department FROM Post";
                using (SqlCommand command = new SqlCommand(queryPost, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var postList = new List<dynamic>();
                    while (reader.Read())
                    {
                        postList.Add(new
                        {
                            Id = (int)reader["ID"],
                            Title = (string)reader["Title"],
                            IdDepartment = (int)reader["Id_department"]
                        });
                    }
                    postComboBox.ItemsSource = postList;
                    postComboBox.DisplayMemberPath = "Title";
                    postComboBox.SelectedValuePath = "Id";
                    postComboBox.Tag = postList; // Сохраняем полный список должностей в Tag для фильтрации
                }

                // Заполнение educationComboBox
                string queryEducation = "SELECT ID, Levell FROM Education";
                using (SqlCommand command = new SqlCommand(queryEducation, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var educationList = new List<KeyValuePair<int, string>>();
                    while (reader.Read())
                    {
                        educationList.Add(new KeyValuePair<int, string>((int)reader["ID"], (string)reader["Levell"]));
                    }
                    educationComboBox.ItemsSource = educationList;
                    educationComboBox.DisplayMemberPath = "Value";
                    educationComboBox.SelectedValuePath = "Key";
                }
            }
        }

        public int AddEmployeeAndSaveSalary(PersonalCard personalCard)
        {
            string connectionString = "data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Добавление сотрудника
                    string personalCardQuery = @"
                INSERT INTO Personal_card (Surname, Name, Patronymic, Date_of_birth, Series_and_number, 
                                           Issued_by_whom, Date_of_issue, Registration_address, Telephone, 
                                           Children, Military_service, Photo, Id_citizenship, Id_post, 
                                           Id_education, Birthplace, Email, EducationInstitution)
                OUTPUT INSERTED.ID
                VALUES (@Surname, @Name, @Patronymic, @DateOfBirth, @SeriesAndNumber, 
                        @IssuedByWhom, @DateOfIssue, @RegistrationAddress, @Telephone, 
                        @Children, @MilitaryService, @Photo, @IdCitizenship, @IdPost, 
                        @IdEducation, @Birthplace, @Email, @EducationInstitution)";

                    int newPersonalCardId;
                    using (SqlCommand personalCardCommand = new SqlCommand(personalCardQuery, connection, transaction))
                    {
                        personalCardCommand.Parameters.AddWithValue("@Surname", personalCard.Surname);
                        personalCardCommand.Parameters.AddWithValue("@Name", personalCard.Name);
                        personalCardCommand.Parameters.AddWithValue("@Patronymic", personalCard.Patronymic);
                        personalCardCommand.Parameters.AddWithValue("@DateOfBirth", personalCard.DateOfBirth);
                        personalCardCommand.Parameters.AddWithValue("@SeriesAndNumber", personalCard.SeriesAndNumber);
                        personalCardCommand.Parameters.AddWithValue("@IssuedByWhom", personalCard.IssuedByWhom);
                        personalCardCommand.Parameters.AddWithValue("@DateOfIssue", personalCard.DateOfIssue);
                        personalCardCommand.Parameters.AddWithValue("@RegistrationAddress", personalCard.RegistrationAddress);
                        personalCardCommand.Parameters.AddWithValue("@Telephone", personalCard.Telephone);
                        personalCardCommand.Parameters.AddWithValue("@Children", personalCard.Children);
                        personalCardCommand.Parameters.AddWithValue("@MilitaryService", personalCard.MilitaryService);
                        personalCardCommand.Parameters.AddWithValue("@Photo", personalCard.Photo);
                        personalCardCommand.Parameters.AddWithValue("@IdCitizenship", personalCard.IdCitizenship);
                        personalCardCommand.Parameters.AddWithValue("@IdPost", personalCard.IdPost);
                        personalCardCommand.Parameters.AddWithValue("@IdEducation", personalCard.IdEducation);
                        personalCardCommand.Parameters.AddWithValue("@Birthplace", personalCard.Birthplace);
                        personalCardCommand.Parameters.AddWithValue("@Email", personalCard.Email ?? (object)DBNull.Value);
                        personalCardCommand.Parameters.AddWithValue("@EducationInstitution", personalCard.EducationInstitution ?? (object)DBNull.Value);

                        newPersonalCardId = (int)personalCardCommand.ExecuteScalar();
                    }

                    // Получение базовой зарплаты для выбранной должности
                    string salaryQuery = @"
                SELECT Base_salary 
                FROM Post 
                WHERE ID = @IdPost";

                    double baseSalary;
                    using (SqlCommand salaryCommand = new SqlCommand(salaryQuery, connection, transaction))
                    {
                        salaryCommand.Parameters.AddWithValue("@IdPost", personalCard.IdPost);
                        object result = salaryCommand.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                        {
                            throw new Exception("Base_salary не найден для указанной должности.");
                        }
                        baseSalary = Convert.ToDouble(result);
                    }

                    // Добавление записи в таблицу Salary
                    string insertSalaryQuery = @"
            INSERT INTO Salary (Id_personal_card, Amount, Date, Id_salary_type)
            VALUES (@IdPersonalCard, @Amount, @Date, @IdSalaryType)";

                    using (SqlCommand insertSalaryCommand = new SqlCommand(insertSalaryQuery, connection, transaction))
                    {
                        insertSalaryCommand.Parameters.AddWithValue("@IdPersonalCard", newPersonalCardId);
                        insertSalaryCommand.Parameters.AddWithValue("@Amount", baseSalary);
                        insertSalaryCommand.Parameters.AddWithValue("@Date", DateTime.Now.Date);
                        insertSalaryCommand.Parameters.AddWithValue("@IdSalaryType", 3);

                        insertSalaryCommand.ExecuteNonQuery();
                    }

                    // Добавление записи в таблицу Entry_in_the_work
                    string entryQuery = @"
            INSERT INTO Entry_in_the_work_book (Id_personal_card, Date, Reason, Id_Mixing)
            VALUES (@IdPersonalCard, @Date, @Reason, @IdMixing)";

                    using (SqlCommand entryCommand = new SqlCommand(entryQuery, connection, transaction))
                    {
                        entryCommand.Parameters.AddWithValue("@IdPersonalCard", newPersonalCardId);
                        entryCommand.Parameters.AddWithValue("@Date", DateTime.Now.Date);
                        entryCommand.Parameters.AddWithValue("@Reason", DBNull.Value); // Пустое значение для причины
                        entryCommand.Parameters.AddWithValue("@IdMixing", 1); // Значение Id_Mixing всегда 1

                        entryCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return newPersonalCardId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Ошибка при добавлении сотрудника: {ex.Message}", ex);
                }

            }
        }

        private void departmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (departmentComboBox.SelectedValue == null) return;

            int selectedDepartmentId = (int)departmentComboBox.SelectedValue;

            // Фильтруем должности по выбранному отделу
            var allPosts = (List<dynamic>)postComboBox.Tag;
            var filteredPosts = allPosts.Where(p => p.IdDepartment == selectedDepartmentId).ToList();

            postComboBox.ItemsSource = filteredPosts;
            postComboBox.SelectedValue = null;
        }

        private void CreateOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage;

            // Валидация имени
            if (!Validator.IsValidName(nameTextBox.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация фамилии
            if (!Validator.IsValidSurName(surnameTextBox.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация отчества
            if (!Validator.IsValidPatronymic(patronymicTextBox.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // Валидация даты рождения
            if (dateOfBirthPicker.SelectedDate.HasValue)
            {
                string dateOfBirthString = dateOfBirthPicker.SelectedDate.Value.ToString("dd.MM.yyyy");  // Преобразуем в строку
                if (!Validator.IsValidBirthDate(dateOfBirthString, out errorMessage))
                {
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите дату рождения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Валидация места рождения
            if (!Validator.IsValidPlace(birthplaceTextBox.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация адреса регистрации
            if (!Validator.IsValidAddress(registrationAddressTextBox.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация телефона
            if (!Validator.IsValidPhone(telephoneTextBox.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация электронной почты
            if (!Validator.IsValidEmail(EmailTxb.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация отдела
            if (!Validator.IsValidField(departmentComboBox.Text, out errorMessage, "Отдел не может быть пустым"))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация должности
            if (!Validator.IsValidField(postComboBox.Text, out errorMessage, "Должность не может быть пустой"))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация серии паспорта
            if (!Validator.IsValidPassportSeries(seriesTextBox.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация номера паспорта
            if (!Validator.IsValidPassportNumber(numberTextBox.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация даты выдачи паспорта
            if (dateOfIssuePicker.SelectedDate.HasValue)
            {
                string dateOfIssueString = dateOfIssuePicker.SelectedDate.Value.ToString("dd.MM.yyyy");  // Преобразуем в строку
                if (!Validator.IsValidDateOfIssue(dateOfIssueString, out errorMessage))
                {
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите дату выдачи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Валидация "Кем выдан"
            if (!Validator.IsValidField(issuedByTextBox.Text, out errorMessage, "Поле «Кем выдан» не может быть пустым"))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация уровня образования
            if (!Validator.IsValidField(educationComboBox.Text, out errorMessage, "Уровень образования не может быть пустым"))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация учебного заведения
            if (!Validator.IsValidEducationInstitution(EducationInstitutionTextBox.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Валидация гражданства
            if (!Validator.IsValidField(citizenshipComboBox.Text, out errorMessage, "Гражданство не может быть пустым"))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        byte[] photoData = null;
            if (AvatarImg.Source != null)
            {
                var bitmapImage = AvatarImg.Source as BitmapImage;
                using (var memoryStream = new MemoryStream())
                {
                    var encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    encoder.Save(memoryStream);
                    photoData = memoryStream.ToArray();
                }
            }

            var personalCard = new PersonalCard
            {
                Surname = surnameTextBox.Text,
                Name = nameTextBox.Text,
                Patronymic = patronymicTextBox.Text,
                DateOfBirth = dateOfBirthPicker.SelectedDate ?? DateTime.MinValue,
                SeriesAndNumber = seriesTextBox.Text + numberTextBox.Text,
                IssuedByWhom = issuedByTextBox.Text,
                DateOfIssue = dateOfIssuePicker.SelectedDate ?? DateTime.MinValue,
                RegistrationAddress = registrationAddressTextBox.Text,
                Telephone = telephoneTextBox.Text,
                Children = childrenRadioButton.IsChecked == true,
                MilitaryService = militaryServiceRadioButton.IsChecked == true,
                IdCitizenship = (int?)citizenshipComboBox.SelectedValue ?? 0,
                IdPost = (int?)postComboBox.SelectedValue ?? 0,
                IdEducation = (int?)educationComboBox.SelectedValue ?? 0,
                Birthplace = birthplaceTextBox.Text,
                Email = EmailTxb.Text,
                EducationInstitution = EducationInstitutionTextBox.Text,
                Photo = photoData
            };

            try
            {
                int newEmployeeId = AddEmployeeAndSaveSalary(personalCard);
                MessageBox.Show("Сотрудник успешно добавлен.");

                // Открытие формы "Приказы"
                MainOrders mainOrdersWindow = new MainOrders(_currentUser);
                mainOrdersWindow.Show();

                // Закрываем текущее окно, если оно больше не нужно
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}");
            }
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                AvatarImg.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void MainWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindowAdmin mainWindowAdmin = new MainWindowAdmin(_currentUser);
            mainWindowAdmin.Show();
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
            MainOrders mainOrders = new MainOrders(_currentUser);
            mainOrders.Show();
            Close();
        }

        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainReports mainReports = new MainReports(_currentUser);
            mainReports.Show();
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            MainStaff mainStaff = new MainStaff(_currentUser);
            mainStaff.Show();
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
    }
}
