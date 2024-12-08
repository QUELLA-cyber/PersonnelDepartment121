using System.Data.SqlClient;

namespace PersonnelDepartment.ClassHelper
{
    public class UserService
    {
        private string _connectionString = "data source=DESKTOP-F4MJF3P;initial catalog=TestDB;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework";

        public UserService(string connectionString)
        {
            this._connectionString = connectionString;
        }

        // Метод для авторизации пользователя по логину и паролю
        public UserInfo Authenticate(string login, string password)
        {
            string query = @"
            SELECT p.FullName, ps.Position
            FROM personal_card p
            JOIN post ps ON p.PostId = ps.Id
            WHERE p.Login = @Login AND p.Password = @Password";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        return new UserInfo
                        {
                            FullName = reader["FullName"].ToString(),
                            Position = reader["Position"].ToString()
                        };
                    }
                    else
                    {
                        return null; // Если пользователь не найден
                    }
                }
            }
        }
    }

    // Класс для хранения информации о пользователе
    public class UserInfo
    {
        public string FullName { get; set; }
        public string Position { get; set; }
    }
}



