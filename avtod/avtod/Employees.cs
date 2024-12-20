using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace avtod
{
    public partial class Employees : Form
    {
        public Employees()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            int currentRoleId = UserManager.CurrentUser.RoleId;
            if (currentRoleId != 1)
            {
                button1.Hide();
                button2.Hide();
                button3.Hide();
            }

            string query = @"
                SELECT 
                    employee_id AS 'ID',
                    first_name AS 'Имя',
                    last_name AS 'Фамилия',
                    date_of_birth AS 'Дата рождения',
                    position AS 'Должность',
                    hiring_date AS 'Дата найма',
                    contact_info AS 'Контактная информация',
                    user_id AS 'ID пользователя'
                FROM Employees";

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
            dataGridView1.Columns["Дата рождения"].HeaderText = "Дата рождения";
            dataGridView1.Columns["Должность"].HeaderText = "Должность";
            dataGridView1.Columns["Дата найма"].HeaderText = "Дата найма";
            dataGridView1.Columns["Контактная информация"].HeaderText = "Контактная информация";
            dataGridView1.Columns["ID пользователя"].HeaderText = "ID пользователя";

            if (dataGridView1.Columns.Contains("ID"))
            {
                dataGridView1.Columns["ID"].Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string firstName = textBox1.Text.Trim();
            string lastName = textBox2.Text.Trim();
            string position = textBox4.Text.Trim();
            string contactInfo = textBox6.Text.Trim();
            string userIdText = textBox7.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(position) || string.IsNullOrEmpty(contactInfo) ||
                string.IsNullOrEmpty(userIdText))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime dateOfBirth = dateTimePicker1.Value;
            DateTime hiringDate = dateTimePicker2.Value;

            if (!int.TryParse(userIdText, out int userId))
            {
                MessageBox.Show("Некорректный формат ID пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = @"
        INSERT INTO Employees (first_name, last_name, date_of_birth, position, hiring_date, contact_info, user_id)
        VALUES (@FirstName, @LastName, @DateOfBirth, @Position, @HiringDate, @ContactInfo, @UserId)";

            ExecuteNonQuery(query,
                ("@FirstName", firstName),
                ("@LastName", lastName),
                ("@DateOfBirth", dateOfBirth),
                ("@Position", position),
                ("@HiringDate", hiringDate),
                ("@ContactInfo", contactInfo),
                ("@UserId", userId));

            LoadEmployees();
            ClearTextBoxes();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите сотрудника для изменения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string firstName = textBox1.Text.Trim();
            string lastName = textBox2.Text.Trim();
            string position = textBox4.Text.Trim();
            string contactInfo = textBox6.Text.Trim();
            string userIdText = textBox7.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(position) || string.IsNullOrEmpty(contactInfo) ||
                string.IsNullOrEmpty(userIdText))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime dateOfBirth = dateTimePicker1.Value;
            DateTime hiringDate = dateTimePicker2.Value;

            if (!int.TryParse(userIdText, out int userId))
            {
                MessageBox.Show("Некорректный формат ID пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int employeeId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

            string query = @"
        UPDATE Employees 
        SET first_name = @FirstName, 
            last_name = @LastName, 
            date_of_birth = @DateOfBirth, 
            position = @Position, 
            hiring_date = @HiringDate, 
            contact_info = @ContactInfo, 
            user_id = @UserId 
        WHERE employee_id = @EmployeeId";

            ExecuteNonQuery(query,
                ("@FirstName", firstName),
                ("@LastName", lastName),
                ("@DateOfBirth", dateOfBirth),
                ("@Position", position),
                ("@HiringDate", hiringDate),
                ("@ContactInfo", contactInfo),
                ("@UserId", userId),
                ("@EmployeeId", employeeId));

            LoadEmployees();
            ClearTextBoxes();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int employeeId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

                if (HasExhibitionLink(employeeId))
                {
                    MessageBox.Show("Назначьте нового куратора выставки перед удалением этого сотрудника.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = "DELETE FROM Employees WHERE employee_id = @EmployeeId";
                ExecuteNonQuery(query, ("@EmployeeId", employeeId));

                LoadEmployees();
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления.");
            }
        }

        private bool HasExhibitionLink(int employeeId)
        {
            string query = "SELECT COUNT(*) FROM Exhibitions WHERE curator_id = @EmployeeId";
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@EmployeeId", employeeId);
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadEmployees();
            ClearTextBoxes();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["Имя"].Value.ToString();
                textBox2.Text = dataGridView1.SelectedRows[0].Cells["Фамилия"].Value.ToString();
                textBox4.Text = dataGridView1.SelectedRows[0].Cells["Должность"].Value.ToString();
                textBox6.Text = dataGridView1.SelectedRows[0].Cells["Контактная информация"].Value.ToString();
                textBox7.Text = dataGridView1.SelectedRows[0].Cells["ID пользователя"].Value.ToString();

                DateTime dateOfBirth = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Дата рождения"].Value);
                DateTime hiringDate = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Дата найма"].Value);
                dateTimePicker1.Value = dateOfBirth;
                dateTimePicker2.Value = hiringDate;
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox4.Text = "";
                textBox6.Text = "";
                textBox7.Text = "";
                dateTimePicker1.Value = DateTime.Now;
                dateTimePicker2.Value = DateTime.Now;
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

        private void EmployeesForm_FormClosed(object sender, FormClosedEventArgs e)
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
            textBox4.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
        }
    }
}