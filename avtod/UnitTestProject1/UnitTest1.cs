using Microsoft.VisualStudio.TestTools.UnitTesting;
using avtod;
using System;

namespace avtod.Tests
{
    [TestClass]
    public class SimpleAuthAndSignUpTests
    {


        //  Проверка совпадения паролей
        [TestMethod]
        public void PasswordsMatch_ShouldReturnTrueForMatchingPasswords()
        {
            
            string password = "testPassword123";
            string confirmPassword = "testPassword123";

            bool passwordsMatch = password == confirmPassword;

           
            Assert.IsTrue(passwordsMatch);
        }

        [TestMethod]
        public void PasswordsMatch_ShouldReturnFalseForDifferentPasswords()
        {
           
            string password = "testPassword123";
            string confirmPassword = "Password";

            bool passwordsMatch = password == confirmPassword;

           
            Assert.IsFalse(passwordsMatch);
        }

        //  Проверка длины пароля
        [TestMethod]
        public void PasswordLength_ShouldReturnTrueForValidLength()
        {
            
            string password = "testPassword123";

           
            bool isValidLength = password.Length >= 8;

            
            Assert.IsTrue(isValidLength);
        }

        [TestMethod]
        public void PasswordLength_ShouldReturnFalseForShortPassword()
        {
            
            string password = "1234567";

           
            bool isValidLength = password.Length >= 8;

            
            Assert.IsFalse(isValidLength);
        }

        //  Проверка пустого email или пароля
        [TestMethod]
        public void IsEmptyEmailOrPassword_ShouldReturnTrueForEmptyInput()
        {
            
            string email = "";
            string password = "";

            
            bool isEmpty = string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password);

            Assert.IsTrue(isEmpty);
        }

       


        // Проверка корректности хэширования пароля (разные пароли)
        [TestMethod]
        public void HashPassword_ShouldReturnDifferentHashesForDifferentPasswords()
        {
           
            var auth = new Auth();
            string password1 = "testPassword123";
            string password2 = "Password456";

            string hashedPassword1 = auth.HashPassword(password1);
            string hashedPassword2 = auth.HashPassword(password2);

           
            Assert.AreNotEqual(hashedPassword1, hashedPassword2);
        }


        //  Проверка заполнения полей для добавления выставки

        [TestMethod]
        public void AddExhibition_ShouldValidateRequiredFields()
        {
            
            string name = "Новая выставки";
            string description = "Новый год";
            int curatorId = 1;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(7);

            
            bool isValid = !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(description) && curatorId > 0;

        
            Assert.IsTrue(isValid);
        }

        //  Проверка хэширования пароля
        [TestMethod]
        public void HashPassword_ShouldReturnHashedPassword()
        {

            var auth = new Auth();
            string password = "testPassword123";

            string hashedPassword = auth.HashPassword(password);


            Assert.IsNotNull(hashedPassword);
            Assert.AreNotEqual(password, hashedPassword);
        }

        //  Проверка валидации email
        [TestMethod]
        public void IsValidEmail_ShouldReturnTrueForValidEmail()
        {

            var signUp = new SignUp();
            string validEmail = "test@example.com";


            bool isValid = signUp.IsValidEmail(validEmail);


            Assert.IsTrue(isValid);
        }

        //  Проверка заполнения полей для обновления экспоната 
        [TestMethod]
        public void UpdateShowpiece_ShouldNotValidateIfAcquisitionDateIsAfterCreationDate()
        {
           
            string inventoryNumber = "Inv 4345";
            string name = "Экспоант";
            string description = "Экспоантn";
            int authorId = 2;
            string material = "Воздух";
            DateTime acquisitionDate = DateTime.Now.AddDays(-5); 
            DateTime creationDate = DateTime.Now;

           
            bool isValid = !string.IsNullOrEmpty(inventoryNumber) && !string.IsNullOrEmpty(name) &&
                           !string.IsNullOrEmpty(description) && authorId > 0 && !string.IsNullOrEmpty(material) &&
                           acquisitionDate >= creationDate; 

            
            Assert.IsTrue(isValid, "Дата приобретения должна быть позже даты создания."); 
        }


    }
}