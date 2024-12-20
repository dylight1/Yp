using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace avtod
{
    public partial class Auth : Form
    {
        
        public Auth Instance { get; private set; }

        public Auth()
        {
            InitializeComponent();
            Instance = this;
        }

        public void button1_Click(object sender, EventArgs e)
        {
            string email = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите email и пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                
                string hashedPassword = HashPassword(password);

                using (SqlConnection connection = DatabaseConnection.GetConnection())
                {
                    string query = "SELECT role_id FROM Users WHERE email = @Email AND password_hash = @PasswordHash";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@PasswordHash", hashedPassword); 

                    connection.Open();
                    object roleId = command.ExecuteScalar();
                    if (roleId != null)
                    {
                        int userRoleId = (int)roleId;
                        UserManager.SetCurrentUser(email, userRoleId); 

                        Main form2 = new Main();
                        form2.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Неверный email или пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (SqlException ex)
            {
                
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
               
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void button2_Click(object sender, EventArgs e)
        {
            SignUp signUpForm = new SignUp();

            signUpForm.Show();

            this.Hide();
        }

        public void Auth_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            Application.Exit();
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
    }
}