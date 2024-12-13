using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersonnelDepartment;
using PersonnelDepartment.ClassHelper;
using System;
using System.IO;
using System.Linq;

namespace HumanDepartmaentTests
{
    [TestClass]
    public class ValidatorTests
    {
        private HumanResourcesDepartmentEntities dbModel;

        [TestInitialize]
        public void Setup()
        {

            dbModel = new HumanResourcesDepartmentEntities();
        }

        // Тест 1 - Ввод с заглавной буквы значения в поле «Имя»
        [TestMethod]
        public void Test_ValidData()
        {
            string errorMessage;

            // Проверяем ввод правильных данных для каждого поля
            bool isValid = Validator.IsValidName("Иван", out errorMessage);
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            isValid = Validator.IsValidName("Иванов", out errorMessage);  // Фамилия
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            isValid = Validator.IsValidName("Иванович", out errorMessage);  // Отчество
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            isValid = Validator.IsValidPhone("78965431234", out errorMessage);  // Телефон
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            isValid = Validator.IsValidEmail("abcd@mail.ru", out errorMessage);  // Электронная почта
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            string validDate = "15.03.2020";
            isValid = Validator.IsValidBirthDate(validDate, out errorMessage);
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            isValid = Validator.IsValidPlace("г. Тула", out errorMessage);  // Место рождения
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            isValid = Validator.IsValidAddress("г. Тула, ул. Ленина, д. 15, кв. 15", out errorMessage);  // Адрес регистрации
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            isValid = Validator.IsValidPassportSeries("1234", out errorMessage);  // Серия паспорта
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            isValid = Validator.IsValidPassportNumber("123456", out errorMessage);  // Номер паспорта
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            validDate = "14.10.2022";
            isValid = Validator.IsValidDateOfIssue(validDate, out errorMessage);
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            isValid = Validator.IsValidIssuedBy("МВД", out errorMessage);  // Кем выдан
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            isValid = Validator.IsValidEducationInstitution("МГУ", out errorMessage);  // Учебное заведение
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            // Списки
            isValid = Validator.IsValidDepartment("Технический", out errorMessage);  // Отдел
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            isValid = Validator.IsValidEducationLevel("Высшее образование (бакалавриат)", out errorMessage);  // Уровень
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            isValid = Validator.IsValidCitizenship("Россия", out errorMessage);  // Гражданство
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);

            byte[] photoBytes = File.ReadAllBytes(@"C:\PDepartment\Media\face.jpg");

            // Сохраняем данные в БД
            var personalCard = new Personal_card
            {
                Name = "Иван",
                Surname = "Иванов",
                Patronymic = "Иванович",
                Date_of_birth = DateTime.Parse("15.03.2020"),
                Email = "abcd@mail.ru",
                Telephone = "78965431234",
                Birthplace = "г. Тула",
                Registration_address = "г. Тула, ул. Ленина, д. 15, кв. 15",
                Series_and_number = "123456",
                Date_of_issue = DateTime.Parse("14.10.2022"),
                Issued_by_whom = "МВД",
                EducationInstitution = "МГУ",
                Id_post = 4,
                Id_education = 1,
                Id_citizenship = 1,
                Children = true,
                Military_service = true,
                Photo = photoBytes
            };

            dbModel.Personal_card.Add(personalCard);
            dbModel.SaveChanges();

            // Проверка, что запись была добавлена
            var addedRecord = dbModel.Personal_card.FirstOrDefault(x => x.Name == "Иван" && x.Surname == "Иванов");
            Assert.IsNotNull(addedRecord);
            Assert.AreEqual("Иван", addedRecord.Name);
            Assert.AreEqual("Иванов", addedRecord.Surname);
        }

        // Тест 2 - Ввод строчной буквы в поле «Имя»
        [TestMethod]
        public void Test_InvalidName_Lowercase()
        {
            string errorMessage;
            bool isValid = Validator.IsValidName("иван", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Некорректный ввод в поле «Имя»", errorMessage);
        }

        // Тест 3 - Ввод в поле «Фамилия» чисел
        [TestMethod]
        public void Test_InvalidSurname_WithNumbers()
        {
            string errorMessage;
            bool isValid = Validator.IsValidSurName("Иванов123", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Некорректный ввод в поле «Фамилия»", errorMessage);
        }

        // Тест 4 - Ввод специального символа в поле «Отчество»
        [TestMethod]
        public void Test_InvalidPatronymic_WithSpecialChar()
        {
            string errorMessage;
            bool isValid = Validator.IsValidPatronymic("Иванович@", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Некорректный ввод в поле «Отчество»", errorMessage);
        }

        // Тест 5 - Ввод номера с буквой в поле «Телефон»
        [TestMethod]
        public void Test_InvalidPhone_WithLetter()
        {
            string errorMessage;
            bool isValid = Validator.IsValidPhone("78965431234a", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Некорректный ввод в поле «Телефонный номер»", errorMessage);
        }

        // Тест 6 - Ввод значения поля «Электронная почта» без знака @
        [TestMethod]
        public void Test_InvalidEmail_WithoutAtSign()
        {
            string errorMessage;
            bool isValid = Validator.IsValidEmail("abcdmail.ru", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Некорректный ввод в поле «Электронная почта»", errorMessage);
        }

        // Тест 7 - Выбор даты в поле «Дата рождения» больше системной
        [TestMethod]
        public void Test_InvalidBirthDate_FutureDate()
        {
            string errorMessage;
            DateTime invalidDate = DateTime.Now.AddYears(1);
            string invalidDateString = invalidDate.ToString("dd.MM.yyyy");  // Преобразуем в строку
            bool isValid = Validator.IsValidBirthDate(invalidDateString, out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Некорректный ввод в поле «Дата рождения»", errorMessage);
        }

        // Тест 8 - Ввод значения в поле «Место рождения» с прописной буквы
        [TestMethod]
        public void Test_InvalidPlace_WithLowercase()
        {
            string errorMessage;
            bool isValid = Validator.IsValidPlace("тула", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Некорректный ввод в поле «Место рождения»", errorMessage);
        }

        // Тест 9 - Ввод чисел в поле «Адрес регистрации»
        [TestMethod]
        public void Test_InvalidAddress_WithNumbers()
        {
            string errorMessage;
            bool isValid = Validator.IsValidAddress("12345", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Некорректный ввод в поле «Адрес регистрации»", errorMessage);
        }

        // Тест 10 - Ввод буквы в поле «Серия»
        [TestMethod]
        public void Test_InvalidPassportSeries_WithLetter()
        {
            string errorMessage;
            bool isValid = Validator.IsValidPassportSeries("123a", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Некорректный ввод в поле «Серия паспорта»", errorMessage);
        }

        // Тест 11 - Ввод специального символа в поле «Номер»
        [TestMethod]
        public void Test_InvalidPassportNumber_WithSpecialChar()
        {
            string errorMessage;
            bool isValid = Validator.IsValidPassportNumber("12345@", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Некорректный ввод в поле «Номер паспорта»", errorMessage);
        }

        // Тест 12 - Ввод даты с специальным символом в поле «Дата выдачи»
        [TestMethod]
        public void Test_InvalidDateOfIssue_WithSpecialChar()
        {
            string errorMessage;
            bool isValid = Validator.IsValidDateOfIssue("14,10.2024", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Некорректный ввод в поле «Дата выдачи»", errorMessage);
        }

        // Тест 13 - Ввод чисел в поле «Кем выдан»
        [TestMethod]
        public void Test_InvalidIssuedBy_WithNumbers()
        {
            string errorMessage;
            bool isValid = Validator.IsValidIssuedBy("12345", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Некорректный ввод в поле «Кем выдан»", errorMessage);
        }

        // Тест 14 - Ввод значения со специальным символом в поле «Учебное заведение»
        [TestMethod]
        public void Test_InvalidEducationInstitution_WithSpecialChar()
        {
            string errorMessage;
            bool isValid = Validator.IsValidEducationInstitution("МГУ@", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Некорректный ввод в поле «Учебное заведение»", errorMessage);
        }

        // Тест 15 - Поле со списком «Отдел» пустое
        [TestMethod]
        public void Test_EmptyDepartment()
        {
            string errorMessage;
            bool isValid = Validator.IsValidDepartment("", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Поле со списком «Отдел» пустое", errorMessage);
        }

        // Тест 16 - Поле со списком «Должность» пустое
        [TestMethod]
        public void Test_EmptyPost()
        {
            string errorMessage;
            bool isValid = Validator.IsValidPost("", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Поле со списком «Должность» пустое", errorMessage);
        }

        // Тест 17 - Поле со списком «Уровень» пустое
        [TestMethod]
        public void Test_EmptyEducationLevel()
        {
            string errorMessage;
            bool isValid = Validator.IsValidEducationLevel("", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Поле со списком «Уровень» пустое", errorMessage);
        }

        // Тест 18 - Поле со списком «Гражданство» пустое
        [TestMethod]
        public void Test_EmptyCitizenship()
        {
            string errorMessage;
            bool isValid = Validator.IsValidCitizenship("", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Поле со списком «Гражданство» пустое", errorMessage);
        }

        // Тест 19 - Поле "Аватар" пустое
        [TestMethod]
        public void Test_InvalidPhoto_FileNotFound()
        {
            string errorMessage;

            bool isValid = Validator.IsValidPhoto("", out errorMessage);
            Assert.IsFalse(isValid);
            Assert.AreEqual("Выберите значение в поле «Аватар»", errorMessage);
        }
    }
}
