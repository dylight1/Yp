using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace avtod
{
    public partial class ExhibitionShowpiece : Form
    {
        public ExhibitionShowpiece()
        {
            InitializeComponent();
            LoadExhibitionShowpiece();
            LoadExhibitions();
            LoadShowpieces();
        }

        private void LoadExhibitionShowpiece()
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
                    es.exhibition_id AS 'ID выставки',
                    e.name AS 'Название выставки',
                    es.showpiece_id AS 'ID экспоната',
                    s.name AS 'Название экспоната'
                FROM ExhibitionShowpiece es
                JOIN Exhibitions e ON es.exhibition_id = e.exhibition_id
                JOIN Showpiece s ON es.showpiece_id = s.showpiece_id";

            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }

            dataGridView1.Columns["ID выставки"].HeaderText = "ID выставки";
            dataGridView1.Columns["Название выставки"].HeaderText = "Название выставки";
            dataGridView1.Columns["ID экспоната"].HeaderText = "ID экспоната";
            dataGridView1.Columns["Название экспоната"].HeaderText = "Название экспоната";

            if (dataGridView1.Columns.Contains("ID выставки"))
            {
                dataGridView1.Columns["ID выставки"].Visible = false;
            }
            if (dataGridView1.Columns.Contains("ID экспоната"))
            {
                dataGridView1.Columns["ID экспоната"].Visible = false;
            }
        }

        private void LoadExhibitions()
        {
            string query = "SELECT exhibition_id, name FROM Exhibitions";
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                comboBox1.DataSource = dataTable;
                comboBox1.DisplayMember = "name";
                comboBox1.ValueMember = "exhibition_id";
            }
        }

        private void LoadShowpieces()
        {
            string query = "SELECT showpiece_id, name FROM Showpiece";
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                comboBox2.DataSource = dataTable;
                comboBox2.DisplayMember = "name";
                comboBox2.ValueMember = "showpiece_id";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null || comboBox2.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, выберите выставку и экспонат.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int exhibitionId = Convert.ToInt32(comboBox1.SelectedValue);
            int showpieceId = Convert.ToInt32(comboBox2.SelectedValue);

            // Проверка на существование записи
            if (IsRecordExists(exhibitionId, showpieceId))
            {
                MessageBox.Show("Запись с такими данными уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = @"
        INSERT INTO ExhibitionShowpiece (exhibition_id, showpiece_id)
        VALUES (@ExhibitionId, @ShowpieceId)";

            ExecuteNonQuery(query,
                ("@ExhibitionId", exhibitionId),
                ("@ShowpieceId", showpieceId));

            LoadExhibitionShowpiece();
            ClearComboBoxes();
        }

        private bool IsRecordExists(int exhibitionId, int showpieceId)
        {
            string query = @"
        SELECT COUNT(*) 
        FROM ExhibitionShowpiece 
        WHERE exhibition_id = @ExhibitionId AND showpiece_id = @ShowpieceId";

            using (SqlConnection connection = DatabaseConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ExhibitionId", exhibitionId);
                command.Parameters.AddWithValue("@ShowpieceId", showpieceId);

                try
                {
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0; // Если count > 0, запись существует
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при проверке записи: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите запись для изменения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox1.SelectedValue == null || comboBox2.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, выберите выставку и экспонат.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int exhibitionId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID выставки"].Value);
            int showpieceId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID экспоната"].Value);

            int newExhibitionId = Convert.ToInt32(comboBox1.SelectedValue);
            int newShowpieceId = Convert.ToInt32(comboBox2.SelectedValue);

            string query = @"
                UPDATE ExhibitionShowpiece 
                SET exhibition_id = @NewExhibitionId, 
                    showpiece_id = @NewShowpieceId 
                WHERE exhibition_id = @ExhibitionId AND showpiece_id = @ShowpieceId";

            ExecuteNonQuery(query,
                ("@NewExhibitionId", newExhibitionId),
                ("@NewShowpieceId", newShowpieceId),
                ("@ExhibitionId", exhibitionId),
                ("@ShowpieceId", showpieceId));

            LoadExhibitionShowpiece();
            ClearComboBoxes();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int exhibitionId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID выставки"].Value);
                int showpieceId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID экспоната"].Value);

                string query = "DELETE FROM ExhibitionShowpiece WHERE exhibition_id = @ExhibitionId AND showpiece_id = @ShowpieceId";
                ExecuteNonQuery(query,
                    ("@ExhibitionId", exhibitionId),
                    ("@ShowpieceId", showpieceId));

                LoadExhibitionShowpiece();
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadExhibitionShowpiece();
            ClearComboBoxes();
        }

        private void ClearComboBoxes()
        {
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int exhibitionId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID выставки"].Value);
                int showpieceId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID экспоната"].Value);

                comboBox1.SelectedValue = exhibitionId;
                comboBox2.SelectedValue = showpieceId;
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

        private void ExhibitionShowpieceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Main.Instance.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Main mainForm = new Main();
            mainForm.Show();
            this.Close();
        }
    }
}