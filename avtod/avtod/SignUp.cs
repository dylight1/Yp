using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace avtod
{
    public partial class SignUp : Form
    {
        private MaskedTextBox textBox4;
        public SignUp()
        {
            InitializeComponent();
        }

        public void buttonRegister_Click(object sender, EventArgs e)
        {
            string firstName = textBox1.Text.Trim();
            string lastName = textBox2.Text.Trim();
            string email = textBox3.Text.Trim();
            string phoneNumber = textBox4.Text.Trim();
            string password = textBox5.Text.Trim();
            string confirmPassword = textBox6.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phoneNumber) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Некорректный формат email.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("Пароль должен содержать минимум 8 символов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string passwordHash = HashPassword(password);

            try
            {
                // Проверка, существует ли пользователь с таким email
                if (IsEmailExists(email))
                {
                    MessageBox.Show("Пользователь с таким email уже зарегистрирован.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string query = @"
                    INSERT INTO Users (first_name, last_name, email, phone_number, password_hash, role_id)
                    VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @PasswordHash, @RoleId)";

                ExecuteNonQuery(query,
                    ("@FirstName", firstName),
                    ("@LastName", lastName),
                    ("@Email", email),
                    ("@PhoneNumber", phoneNumber),
                    ("@PasswordHash", passwordHash),
                    ("@RoleId", 2));

                MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public bool IsEmailExists(string email)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE email = @Email", connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        public void buttonCancel_Click(object sender, EventArgs e)
        {
            Auth authForm = new Auth();
            authForm.Show();
            this.Hide();
        }

        public void sign_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        public void ExecuteNonQuery(string query, params (string, object)[] parameters)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Item1, parameter.Item2);
                }

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при выполнении операции: " + ex.Message);
                }
            }
        }
    }
}