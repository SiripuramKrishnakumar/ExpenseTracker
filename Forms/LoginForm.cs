using System;
using System.Windows.Forms;
using ExpenseTracker.Auth;
using System.Drawing;

namespace ExpenseTracker.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Expense Tracker - Login";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Create controls
            var lblUsername = new Label
            {
                Text = "Username:",
                Location = new Point(50, 50),
                AutoSize = true
            };

            txtUsername = new TextBox
            {
                Location = new Point(150, 50),
                Size = new Size(200, 20)
            };

            var lblPassword = new Label
            {
                Text = "Password:",
                Location = new Point(50, 90),
                AutoSize = true
            };

            txtPassword = new TextBox
            {
                Location = new Point(150, 90),
                Size = new Size(200, 20),
                PasswordChar = '•'
            };

            btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(150, 130),
                Size = new Size(90, 30)
            };
            btnLogin.Click += BtnLogin_Click;

            btnRegister = new Button
            {
                Text = "Register",
                Location = new Point(260, 130),
                Size = new Size(90, 30)
            };
            btnRegister.Click += BtnRegister_Click;

            // Add controls to form
            this.Controls.AddRange(new Control[] 
            { 
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                btnLogin, btnRegister
            });
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both username and password.", "Login Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (AuthHelper.ValidateLogin(txtUsername.Text, txtPassword.Text))
            {
                var dashboardForm = new DashboardForm(txtUsername.Text);
                this.Hide();
                dashboardForm.FormClosed += (s, args) => this.Close();
                dashboardForm.Show();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both username and password.", "Registration Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Registration Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (AuthHelper.RegisterUser(txtUsername.Text, txtPassword.Text))
            {
                MessageBox.Show("Registration successful! You can now login.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Username already exists. Please choose a different username.", 
                    "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
    }
}
