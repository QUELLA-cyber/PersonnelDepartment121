using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelDepartment.ClassHelper
{
    public class Employee
    {
        public int ID { get; set; }
        public string FirstName { get; set; }       // Name из Personal_card
        public string LastName { get; set; }        // Surname из Personal_card
        public string Patronymic { get; set; }
        public string Position { get; set; }        // Title из Post
        public string Department { get; set; }      // Title из Department
        public string Phone { get; set; }           // Telephone из Personal_card
        public DateTime BirthDate { get; set; }     // Date_of_birth из Personal_card
        public byte[] PhotoData { get; set; }       // Путь к изображению
        

        // Свойство для отображения полного имени
        public string FullName => $"{FirstName} {LastName}";

        // Новые свойства для логина и пароля
        public string Login { get; set; }
        public string Password { get; set; }

    }
}
