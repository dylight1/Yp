using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace avtod
{
    public partial class Users : Form
    {
        public Users()
        {
            InitializeComponent();
            LoadUsers();
            LoadRoles(); // Загрузка ролей в ComboBox
        }

        private void LoadUsers()
        {
            int currentRoleId = UserManager.CurrentUser.RoleId;
            if (currentRoleId != 1)
            {
                button2.Hide();
                button3.Hide();
            }

            string query = @"
                SELECT 
                    u.user_id AS 'ID',
                    u.first_name AS 'Имя',
                    u.last_name AS 'Фамилия',
                    u.email AS 'Email',
                    u.phone_number AS 'Номер телефона',
                    r.role_name AS 'Роль'
                FROM Users u
                JOIN Roles r ON u.role_id = r.role_id";

            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }

            dataGridView1.Columns["ID"].HeaderText = "ID";
            dataGridView1.Columns["Имя"].HeaderText = "Имя";
            dataGridView1.Columns["Фамилия"].HeaderText = "Фамилия";
            dataGridView1.Columns["Email"].HeaderText = "Email";
            dataGridView1.Columns["Номер телефона"].HeaderText = "Номер телефона";
            dataGridView1.Columns["Роль"].HeaderText = "Роль";

            if (dataGridView1.Columns.Contains("ID"))
            {
                dataGridView1.Columns["ID"].Visible = false;
            }
        }

        private void LoadRoles()
        {
            string query = "SELECT role_name FROM Roles";
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                comboBox1.Items.Clear();
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["role_name"].ToString());
                }
            }
            comboBox1.SelectedIndex = 0; 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя для обновления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string firstName = textBox1.Text.Trim();
            string lastName = textBox2.Text.Trim();
            string email = textBox3.Text.Trim();
            string phoneNumber = textBox4.Text.Trim();
            string roleName = comboBox1.SelectedItem.ToString(); 

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Некорректный формат email.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int userId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

            int roleId = GetRoleIdByName(roleName);
            if (roleId == -1)
            {
                MessageBox.Show("Роль не найдена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = @"
                UPDATE Users 
                SET first_name = @FirstName, 
                    last_name = @LastName, 
                    email = @Email, 
                    phone_number = @PhoneNumber, 
                    role_id = @RoleId 
                WHERE user_id = @UserId";

            ExecuteNonQuery(query,
                ("@FirstName", firstName),
                ("@LastName", lastName),
                ("@Email", email),
                ("@PhoneNumber", phoneNumber),
                ("@RoleId", roleId),
                ("@UserId", userId));

            LoadUsers();
            ClearTextBoxes();
        }

        private bool IsValidEmail(string email)
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

        private int GetRoleIdByName(string roleName)
        {
            string query = "SELECT role_id FROM Roles WHERE role_name = @RoleName";
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@RoleName", roleName);
                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                return -1;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int userId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

                if (HasEmployeeLink(userId))
                {
                    MessageBox.Show("Невозможно удалить пользователя, так как с ним есть связь в таблице 'Работники'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string query = "DELETE FROM Users WHERE user_id = @UserId";
                ExecuteNonQuery(query, ("@UserId", userId));

                LoadUsers();
            }
            else
            {
                MessageBox.Show("Выберите пользователя для удаления.");
            }
        }

        private bool HasEmployeeLink(int userId)
        {
            string query = "SELECT COUNT(*) FROM Employees WHERE user_id = @UserId";
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadUsers();
            ClearTextBoxes();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].Index != -1)
            {
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["Имя"].Value.ToString();
                textBox2.Text = dataGridView1.SelectedRows[0].Cells["Фамилия"].Value.ToString();
                textBox3.Text = dataGridView1.SelectedRows[0].Cells["Email"].Value.ToString();
                textBox4.Text = dataGridView1.SelectedRows[0].Cells["Номер телефона"].Value.ToString();
                comboBox1.SelectedItem = dataGridView1.SelectedRows[0].Cells["Роль"].Value.ToString();
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                comboBox1.SelectedIndex = 0;
            }
        }

        private void ExecuteNonQuery(string query, params (string, object)[] parameters)
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
                    MessageBox.Show("Операция успешно выполнена.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при выполнении операции: " + ex.Message);
                }
            }
        }

        private void UsersForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Main.Instance.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Main mainForm = new Main();
            mainForm.Show();
            this.Close();
        }

        private void ClearTextBoxes()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.SelectedIndex = 0;
        }
    }
}