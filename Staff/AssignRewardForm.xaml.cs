    using System.Windows;
    using System.Data.SqlClient;
    using System;

    namespace PersonnelDepartment.Staff
    {
        /// <summary>
        /// Логика взаимодействия для AssignRewardForm.xaml
        /// </summary>
        public partial class AssignRewardForm : Window
        {
            private int _employeeId; // ID сотрудника

            public AssignRewardForm(int employeeId, string employeeName)
            {
                InitializeComponent();
                _employeeId = employeeId;

                // Отображение имени сотрудника
                EmployeeNameTextBlock.Text = $"{employeeName}";

                // Загрузка типов наград в ComboBox
                LoadRewardTypes();
            }

            private void LoadRewardTypes()
            {
                using (var connection = new SqlConnection("data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False"))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT ID, Title FROM Salary_type WHERE ID IN (1, 2, 3)", connection); // Убедитесь, что выбираем 1, 2 и 3
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        RewardTypeComboBox.Items.Add(new { Id = reader.GetInt32(0), Title = reader.GetString(1) }); // Добавляем в ComboBox
                    }
                }
            }

            private void CancelBtn_Click(object sender, RoutedEventArgs e)
            {
                this.Close();
            }

            private void AddButton_Click(object sender, RoutedEventArgs e)
            {
                if (RewardTypeComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(AmountTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!float.TryParse(AmountTextBox.Text, out float amount))
                {
                    MessageBox.Show("Сумма должна быть числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedReward = (dynamic)RewardTypeComboBox.SelectedItem;
                int rewardTypeId = selectedReward.Id; // Получаем ID награды

                using (var connection = new SqlConnection("data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False"))
                {
                    connection.Open();

                    // Проверка, существует ли запись для выбранного сотрудника и типа награды
                    var checkCommand = new SqlCommand("SELECT Amount FROM Salary WHERE Id_personal_card = @employeeId AND Id_salary_type = @rewardTypeId", connection);
                    checkCommand.Parameters.AddWithValue("@employeeId", _employeeId);
                    checkCommand.Parameters.AddWithValue("@rewardTypeId", rewardTypeId);

                    var existingAmount = checkCommand.ExecuteScalar(); // Проверяем наличие записи

                    if (existingAmount != null) // Запись найдена
                    {
                        // Обновляем запись
                        var updateCommand = new SqlCommand("UPDATE Salary SET Amount = @amount, Date = @date WHERE Id_personal_card = @employeeId AND Id_salary_type = @rewardTypeId", connection);
                        updateCommand.Parameters.AddWithValue("@employeeId", _employeeId);
                        updateCommand.Parameters.AddWithValue("@amount", amount);
                        updateCommand.Parameters.AddWithValue("@date", DateTime.Now);
                        updateCommand.Parameters.AddWithValue("@rewardTypeId", rewardTypeId);

                        try
                        {
                            updateCommand.ExecuteNonQuery();
                            MessageBox.Show("Запись успешно обновлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else // Запись не найдена
                    {
                        // Добавляем новую запись
                        var insertCommand = new SqlCommand("INSERT INTO Salary (Id_personal_card, Amount, Date, Id_salary_type) VALUES (@employeeId, @amount, @date, @rewardTypeId)", connection);
                        insertCommand.Parameters.AddWithValue("@employeeId", _employeeId);
                        insertCommand.Parameters.AddWithValue("@amount", amount);
                        insertCommand.Parameters.AddWithValue("@date", DateTime.Now);
                        insertCommand.Parameters.AddWithValue("@rewardTypeId", rewardTypeId);

                        try
                        {
                            insertCommand.ExecuteNonQuery();
                            MessageBox.Show("Запись успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }

                this.Close();
            }

            private void RewardTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            {
                if (RewardTypeComboBox.SelectedItem == null)
                    return;

                var selectedReward = (dynamic)RewardTypeComboBox.SelectedItem;
                int rewardTypeId = selectedReward.Id;

                using (var connection = new SqlConnection("data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False"))
                {
                    connection.Open();

                    // Проверяем, есть ли запись для выбранного сотрудника и типа награды
                    var command = new SqlCommand("SELECT Amount FROM Salary WHERE Id_personal_card = @employeeId AND Id_salary_type = @rewardTypeId", connection);
                    command.Parameters.AddWithValue("@employeeId", _employeeId);
                    command.Parameters.AddWithValue("@rewardTypeId", rewardTypeId);

                    var existingAmount = command.ExecuteScalar();

                    if (existingAmount != null) // Запись найдена
                    {
                        AmountTextBox.Text = existingAmount.ToString(); // Отображаем сумму в TextBox
                    }
                    else
                    {
                        AmountTextBox.Clear(); // Если записи нет, очищаем TextBox
                    }
                }
            }
        }
    }
