using System;
using System.Windows.Forms;

namespace avtod
{
    public partial class Main : Form
    {
        public static Main Instance { get; private set; }

        public Main()
        {
            InitializeComponent();
            Instance = this;
        }

        private void Main_Load(object sender, EventArgs e)
        {
           
            if (UserManager.CurrentUser == null)
            {
                MessageBox.Show("Пользователь не авторизован!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            
            int currentRoleId = UserManager.CurrentUser.RoleId;
            string userEmail = UserManager.CurrentUser.Email;

            
            label1.Text = $"Добро пожаловать: {userEmail}";

           
            SetFormAccess(currentRoleId);
        }

        private void SetFormAccess(int roleId)
        {
           
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;

            
            switch (roleId)
            {
                case 1: 
                    button1.Enabled = true; 
                    button2.Enabled = true; 
                    button3.Enabled = true; 
                    button4.Enabled = true; 
                    button5.Enabled = true; 
                    button6.Enabled = true; 
                    button7.Enabled = true; 

                    break;

                case 2:
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    button4.Visible = false;
                    button5.Visible = false;
                    button6.Visible = false;
                    button7.Visible = false;
                    break;

                case 3:
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    button4.Enabled = true;
                    button5.Visible = false;
                    button6.Visible = false;
                    button7.Visible = false;
                    break;

                default:
                    MessageBox.Show("Неизвестная роль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    break;
            }
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        private void btnLogout_Click(object sender, EventArgs e)
        {
            
            UserManager.ClearCurrentUser();

            
            Auth AuthForm = new Auth();
            AuthForm.Show();

            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Showpiece ShowpieceForm = new Showpiece();
            ShowpieceForm.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Authors AuthorsForm = new Authors();
            AuthorsForm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Exhibitions ExhibitionsForm = new Exhibitions();
            ExhibitionsForm.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ExhibitionShowpiece ExhibitionShowpieceForm = new ExhibitionShowpiece();
            ExhibitionShowpieceForm.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
           
            Employees EmployeesForm = new Employees();
            EmployeesForm.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Roles RolesForm = new Roles();
            RolesForm.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Users UsersForm = new Users();
            UsersForm.Show();
            this.Hide();
        }
    }
}