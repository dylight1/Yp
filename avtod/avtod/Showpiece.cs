using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace avtod
{
    public partial class Showpiece : Form
    {
        public Showpiece()
        {
            InitializeComponent();
            LoadShowpieces();
            LoadAuthorsComboBox();
        }

        private void LoadAuthorsComboBox()
        {
            string query = @"
                SELECT 
                    author_id,
                    first_name + ' ' + last_name AS 'FullName'
                FROM Authors";

            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                comboBox1.DisplayMember = "FullName";
                comboBox1.ValueMember = "author_id";
                comboBox1.DataSource = dataTable;
            }
        }

        private void LoadShowpieces()
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
                    s.showpiece_id AS 'ID',
                    s.inventory_number AS 'Инвентарный номер',
                    s.name AS 'Название',
                    s.description AS 'Описание',
                    s.acquisition_date AS 'Дата приобретения',
                    a.first_name + ' ' + a.last_name AS 'Автор',
                    s.creation_date AS 'Дата создания',
                    s.material AS 'Материал'
                FROM Showpiece s
                JOIN Authors a ON s.author_id = a.author_id";

            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }

            dataGridView1.Columns["ID"].HeaderText = "ID";
            dataGridView1.Columns["Инвентарный номер"].HeaderText = "Инвентарный номер";
            dataGridView1.Columns["Название"].HeaderText = "Название";
            dataGridView1.Columns["Описание"].HeaderText = "Описание";
            dataGridView1.Columns["Дата приобретения"].HeaderText = "Дата приобретения";
            dataGridView1.Columns["Автор"].HeaderText = "Автор";
            dataGridView1.Columns["Дата создания"].HeaderText = "Дата создания";
            dataGridView1.Columns["Материал"].HeaderText = "Материал";

            if (dataGridView1.Columns.Contains("ID"))
            {
                dataGridView1.Columns["ID"].Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string inventoryNumber = textBox1.Text.Trim();
            string name = textBox2.Text.Trim();
            string description = textBox3.Text.Trim();
            int authorId = comboBox1.SelectedValue == null ? 0 : (int)comboBox1.SelectedValue;
            string material = textBox7.Text.Trim();

            
            if (string.IsNullOrEmpty(inventoryNumber) || string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(description) || authorId == 0 ||
                string.IsNullOrEmpty(material))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime acquisitionDate = dateTimePicker1.Value;
            DateTime creationDate = dateTimePicker2.Value;

          
            string query = @"
        INSERT INTO Showpiece (inventory_number, name, description, acquisition_date, author_id, creation_date, material)
        VALUES (@InventoryNumber, @Name, @Description, @AcquisitionDate, @AuthorId, @CreationDate, @Material)";

            ExecuteNonQuery(query,
                ("@InventoryNumber", inventoryNumber),
                ("@Name", name),
                ("@Description", description),
                ("@AcquisitionDate", acquisitionDate),
                ("@AuthorId", authorId),
                ("@CreationDate", creationDate),
                ("@Material", material));

            LoadShowpieces();
            ClearTextBoxes();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите экспонат для обновления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string inventoryNumber = textBox1.Text.Trim();
            string name = textBox2.Text.Trim();
            string description = textBox3.Text.Trim();
            int authorId = (int)comboBox1.SelectedValue;
            string material = textBox7.Text.Trim();

            if (string.IsNullOrEmpty(inventoryNumber) || string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(description) || authorId == 0 ||
                string.IsNullOrEmpty(material))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime acquisitionDate = dateTimePicker1.Value;
            DateTime creationDate = dateTimePicker2.Value;

            int showpieceId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

            string query = @"
        UPDATE Showpiece 
        SET inventory_number = @InventoryNumber, 
            name = @Name, 
            description = @Description, 
            acquisition_date = @AcquisitionDate, 
            author_id = @AuthorId, 
            creation_date = @CreationDate, 
            material = @Material 
        WHERE showpiece_id = @ShowpieceId";

            ExecuteNonQuery(query,
                ("@InventoryNumber", inventoryNumber),
                ("@Name", name),
                ("@Description", description),
                ("@AcquisitionDate", acquisitionDate),
                ("@AuthorId", authorId),
                ("@CreationDate", creationDate),
                ("@Material", material),
                ("@ShowpieceId", showpieceId));

            LoadShowpieces();
            ClearTextBoxes();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int showpieceId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

                string query = "DELETE FROM Showpiece WHERE showpiece_id = @ShowpieceId";
                ExecuteNonQuery(query, ("@ShowpieceId", showpieceId));

                LoadShowpieces();
            }
            else
            {
                MessageBox.Show("Выберите экспонат для удаления.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadShowpieces();
            ClearTextBoxes();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["Инвентарный номер"].Value.ToString();
                textBox2.Text = dataGridView1.SelectedRows[0].Cells["Название"].Value.ToString();
                textBox3.Text = dataGridView1.SelectedRows[0].Cells["Описание"].Value.ToString();
                textBox7.Text = dataGridView1.SelectedRows[0].Cells["Материал"].Value.ToString();

                
                DateTime acquisitionDate = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Дата приобретения"].Value);
                DateTime creationDate = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["Дата создания"].Value);
                dateTimePicker1.Value = acquisitionDate;
                dateTimePicker2.Value = creationDate;

                string authorFullName = dataGridView1.SelectedRows[0].Cells["Автор"].Value.ToString();
                comboBox1.SelectedIndex = comboBox1.FindStringExact(authorFullName);
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox7.Text = "";
                dateTimePicker1.Value = DateTime.Now; 
                dateTimePicker2.Value = DateTime.Now; 
                comboBox1.SelectedIndex = -1;
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

        private void Showpiece_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Main mainForm = new Main();
            mainForm.Show();
            this.Hide();
        }

        private void ClearTextBoxes()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox7.Text = "";
            dateTimePicker1.Value = DateTime.Now; 
            dateTimePicker2.Value = DateTime.Now;
            comboBox1.SelectedIndex = -1;
        }
    }
}