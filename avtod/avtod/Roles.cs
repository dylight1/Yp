using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace avtod
{
    public partial class Roles : Form
    {
        public Roles()
        {
            InitializeComponent();
            LoadRoles();
        }

        private void LoadRoles()
        {
            int currentRoleId = UserManager.CurrentUser.RoleId;
            if (currentRoleId != 1)
            {
                button1.Hide();
                button2.Hide();
               
            }

            string query = @"
                SELECT 
                    role_id AS 'ID',
                    role_name AS 'Название роли'
                FROM Roles";

            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }

            dataGridView1.Columns["ID"].HeaderText = "ID";
            dataGridView1.Columns["Название роли"].HeaderText = "Название роли";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string roleName = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(roleName))
            {
                MessageBox.Show("Пожалуйста, заполните поле 'Название роли'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = @"
                INSERT INTO Roles (role_name)
                VALUES (@RoleName)";

            ExecuteNonQuery(query, ("@RoleName", roleName));

            LoadRoles();
            ClearTextBoxes();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите роль для обновления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string roleName = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(roleName))
            {
                MessageBox.Show("Пожалуйста, заполните поле 'Название роли'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int roleId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

            string query = @"
                UPDATE Roles 
                SET role_name = @RoleName 
                WHERE role_id = @RoleId";

            ExecuteNonQuery(query,
                ("@RoleName", roleName),
                ("@RoleId", roleId));

            LoadRoles();
            ClearTextBoxes();
        }

       

        private void button4_Click(object sender, EventArgs e)
        {
            LoadRoles();
            ClearTextBoxes();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["Название роли"].Value.ToString();
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

        private void RolesForm_FormClosed(object sender, FormClosedEventArgs e)
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
        }
    }
}