using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace avtod
{
    public partial class Exhibitions : Form
    {
       

        public Exhibitions()
        {
            InitializeComponent();
            LoadExhibitions();
            LoadCuratorsComboBox();

            
        }

        public void LoadCuratorsComboBox()
        {
            string query = @"
                SELECT 
                    employee_id,
                    first_name + ' ' + last_name AS 'FullName'
                FROM Employees";

            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                comboBox1.DisplayMember = "FullName";
                comboBox1.ValueMember = "employee_id";
                comboBox1.DataSource = dataTable;
            }
        }

        public void LoadExhibitions()
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
                    e.exhibition_id AS 'ID',
                    e.name AS 'Название',
                    e.start_date AS 'Дата начала',
                    e.end_date AS 'Дата окончания',
                    e.description AS 'Описание',
                    emp.first_name + ' ' + emp.last_name AS 'Куратор'
                FROM Exhibitions e
                JOIN Employees emp ON e.curator_id = emp.employee_id";

            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }

            dataGridView1.Columns["ID"].HeaderText = "ID";
            dataGridView1.Columns["Название"].HeaderText = "Название";
            dataGridView1.Columns["Дата начала"].HeaderText = "Дата начала";
            dataGridView1.Columns["Дата окончания"].HeaderText = "Дата окончания";
            dataGridView1.Columns["Описание"].HeaderText = "Описание";
            dataGridView1.Columns["Куратор"].HeaderText = "Куратор";

            if (dataGridView1.Columns.Contains("ID"))
            {
                dataGridView1.Columns["ID"].Visible = false;
            }
        }

        public void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim();
            string description = textBox4.Text.Trim();
            int curatorId = comboBox1.SelectedValue == null ? 0 : (int)comboBox1.SelectedValue;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) ||
                curatorId == 0)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;

            if (endDate < startDate)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = @"
                INSERT INTO Exhibitions (name, start_date, end_date, description, curator_id)
                VALUES (@Name, @StartDate, @EndDate, @Description, @CuratorId)";

            ExecuteNonQuery(query,
                ("@Name", name),
                ("@StartDate", startDate),
                ("@EndDate", endDate),
                ("@Description", description),
                ("@CuratorId", curatorId));

            LoadExhibitions();
            ClearTextBoxes();
        }

        public void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите выставку для изменения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string name = textBox1.Text.Trim();
            string description = textBox4.Text.Trim();
            int curatorId = (int)comboBox1.SelectedValue;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) ||
                curatorId == 0)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;

            if (endDate < startDate)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int exhibitionId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

            string query = @"
                UPDATE Exhibitions 
                SET name = @Name, 
                    start_date = @StartDate, 
                    end_date = @EndDate, 
                    description = @Description, 
                    curator_id = @CuratorId 
                WHERE exhibition_id = @ExhibitionId";

            ExecuteNonQuery(query,
                ("@Name", name),
                ("@StartDate", startDate),
                ("@EndDate", endDate),
                ("@Description", description),
                ("@CuratorId", curatorId),
                ("@ExhibitionId", exhibitionId));

            LoadExhibitions();
            ClearTextBoxes();
        }

        public void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int exhibitionId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

                DeleteExhibitionShowpieces(exhibitionId);

                string query = "DELETE FROM Exhibitions WHERE exhibition_id = @ExhibitionId";
                ExecuteNonQuery(query, ("@ExhibitionId", exhibitionId));

                LoadExhibitions();
            }
            else
            {
                MessageBox.Show("Выберите выставку для удаления.");
            }
        }

        public void DeleteExhibitionShowpieces(int exhibitionId)
        {
            string query = "DELETE FROM ExhibitionShowpiece WHERE exhibition_id = @ExhibitionId";
            ExecuteNonQuery(query, ("@ExhibitionId", exhibitionId));
        }

        public void button4_Click(object sender, EventArgs e)
        {
            LoadExhibitions();
            ClearTextBoxes();
        }

        public void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["Название"].Value.ToString();
                textBox4.Text = dataGridView1.SelectedRows[0].Cells["Описание"].Value.ToString();

                DateTime startDate = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Дата начала"].Value);
                DateTime endDate = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Дата окончания"].Value);
                dateTimePicker1.Value = startDate;
                dateTimePicker2.Value = endDate;

                string curatorFullName = dataGridView1.SelectedRows[0].Cells["Куратор"].Value.ToString();
                comboBox1.SelectedIndex = comboBox1.FindStringExact(curatorFullName);
            }
            else
            {
                textBox1.Text = "";
                textBox4.Text = "";
                dateTimePicker1.Value = DateTime.Now;
                dateTimePicker2.Value = DateTime.Now;
                comboBox1.SelectedIndex = -1;
            }
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
                    MessageBox.Show("Операция успешно выполнена.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при выполнении операции: " + ex.Message);
                }
            }
        }

        public void ExhibitionsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Main.Instance.Show();
        }

        public void button5_Click(object sender, EventArgs e)
        {
            Main mainForm = new Main();
            mainForm.Show();
            this.Close();
        }

        public void ClearTextBoxes()
        {
            textBox1.Text = "";
            textBox4.Text = "";
            comboBox1.SelectedIndex = -1;
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
        }
    }
}