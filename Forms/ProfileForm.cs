using System;
using System.Windows.Forms;
using System.Drawing;

namespace ExpenseTracker.Forms
{
    public class ProfileForm : BaseForm
    {
        private Label lblUsername;
        private Button btnChangePassword;

        public ProfileForm(string username) : base(username)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "User Profile";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            lblUsername = new Label
            {
                Text = $"Username: {currentUsername}",
                Location = new Point(50, 50),
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 12)
            };

            btnChangePassword = new Button
            {
                Text = "Change Password",
                Location = new Point(50, 100),
                Size = new Size(150, 30)
            };
            btnChangePassword.Click += BtnChangePassword_Click;

            this.Controls.AddRange(new Control[] {
                lblUsername,
                btnChangePassword
            });
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            // TODO: Implement password change functionality
            MessageBox.Show("Password change functionality will be implemented soon.", 
                "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
