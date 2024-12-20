using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace avtod
{
    public partial class Authors : Form
    {
        public Authors()
        {
            InitializeComponent();
            LoadAuthors();
            dateTimePicker1.MinDate = DateTime.MinValue; // Установка минимальной даты (0001-01-01)
        }

        private void LoadAuthors()
        {

            int currentRoleId = UserManager.CurrentUser.RoleId;
            if (currentRoleId != 1 && currentRoleId != 3)
            {
                button1.Hide();
                button2.Hide();
                button3.Hide();
            }


            string query = @"
                SELECT 
                    author_id AS 'ID',
                    first_name AS 'Имя',
                    last_name AS 'Фамилия',
                    birthdate AS 'Дата рождения'
                FROM Authors";


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


            if (dataGridView1.Columns.Contains("ID"))
            {
                dataGridView1.Columns["ID"].Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string firstName = textBox1.Text.Trim();
            string lastName = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime birthdate = dateTimePicker1.Value;

            // Проверка даты
            if (birthdate < new DateTime(0001, 1, 1))
            {
                MessageBox.Show("Дата рождения не может быть раньше 0001 года.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = @"
        INSERT INTO Authors (first_name, last_name, birthdate)
        VALUES (@FirstName, @LastName, @Birthdate)";

            try
            {
                ExecuteNonQuery(query,
                    ("@FirstName", firstName),
                    ("@LastName", lastName),
                    ("@Birthdate", birthdate));

                LoadAuthors();
                ClearTextBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении автора: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите автора для изменения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string firstName = textBox1.Text.Trim();
            string lastName = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime birthdate = dateTimePicker1.Value;

            int authorId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

            string query = @"
        UPDATE Authors
        SET first_name = @FirstName,
            last_name = @LastName,
            birthdate = @Birthdate
        WHERE author_id = @AuthorId";

            ExecuteNonQuery(query,
                ("@FirstName", firstName),
                ("@LastName", lastName),
                ("@Birthdate", birthdate),
                ("@AuthorId", authorId));

            LoadAuthors();
            ClearTextBoxes();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int authorId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

                
                if (HasReferencesInShowpiece(authorId))
                {
                    MessageBox.Show("Невозможно удалить автора, так как на него есть ссылки в таблице экспонатов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                
                string query = "DELETE FROM Authors WHERE author_id = @AuthorId";
                ExecuteNonQuery(query, ("@AuthorId", authorId));

                LoadAuthors();
            }
            else
            {
                MessageBox.Show("Выберите автора для удаления.");
            }
        }

        private bool HasReferencesInShowpiece(int authorId)
        {
            string query = @"
        SELECT COUNT(*) 
        FROM Showpiece 
        WHERE author_id = @AuthorId";

            using (SqlConnection connection = DatabaseConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@AuthorId", authorId);

                try
                {
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при проверке ссылок: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadAuthors();
            ClearTextBoxes();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].Index != -1)
            {
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["Имя"].Value.ToString();
                textBox2.Text = dataGridView1.SelectedRows[0].Cells["Фамилия"].Value.ToString();

                // Заполнение DateTimePicker значением из DataGridView
                DateTime birthdate = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Дата рождения"].Value);
                dateTimePicker1.Value = birthdate;
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                dateTimePicker1.Value = DateTime.Now; // Установка текущей даты по умолчанию
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

        private void AuthorsForm_FormClosed(object sender, FormClosedEventArgs e)
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
            dateTimePicker1.Value = DateTime.Now; 
        }
    }
}