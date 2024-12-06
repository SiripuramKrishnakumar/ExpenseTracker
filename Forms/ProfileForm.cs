using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace ExpenseTracker.Forms
{
    public class ProfileForm : Form
    {
        private TableLayoutPanel mainPanel;
        private Label lblUsername;
        private TextBox txtEmail;
        private TextBox txtFullName;
        private Button btnSave;
        private Button btnChangePassword;
        private Button btnCancel;
        private readonly string currentUsername;
        private readonly Color primaryColor = Color.FromArgb(63, 81, 181);
        private readonly Color textColor = Color.FromArgb(33, 33, 33);
        private readonly Color backgroundColor = Color.White;

        public ProfileForm(string username)
        {
            currentUsername = username;
            InitializeComponent();
            ApplyModernStyling();
        }

        private void InitializeComponent()
        {
            this.Text = "Profile Settings";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = backgroundColor;

            // Main container
            var containerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                BackColor = backgroundColor
            };

            // Create main panel
            mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                BackColor = backgroundColor,
                ColumnStyles = 
                {
                    new ColumnStyle(SizeType.Percent, 30),
                    new ColumnStyle(SizeType.Percent, 70)
                }
            };

            // Add title
            var titleLabel = new Label
            {
                Text = "Profile Settings",
                Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold),
                ForeColor = primaryColor,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 50
            };

            // Username
            lblUsername = new Label
            {
                Text = $"@{currentUsername}",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 10, 0, 20)
            };
            mainPanel.Controls.Add(lblUsername, 0, 0);
            mainPanel.SetColumnSpan(lblUsername, 2);

            // Full Name
            var lblFullName = CreateFieldLabel("Full Name");
            mainPanel.Controls.Add(lblFullName, 0, 1);

            txtFullName = CreateTextBox();
            mainPanel.Controls.Add(txtFullName, 1, 1);

            // Email
            var lblEmail = CreateFieldLabel("Email Address");
            mainPanel.Controls.Add(lblEmail, 0, 2);

            txtEmail = CreateTextBox();
            mainPanel.Controls.Add(txtEmail, 1, 2);

            // Buttons panel
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(0, 10, 0, 0)
            };

            // Create button layout
            var buttonsTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                ColumnStyles = 
                {
                    new ColumnStyle(SizeType.Percent, 33.33F),
                    new ColumnStyle(SizeType.Percent, 33.33F),
                    new ColumnStyle(SizeType.Percent, 33.33F)
                }
            };

            btnChangePassword = CreateStyledButton("Change Password");
            btnSave = CreateStyledButton("Save Changes", true);
            btnCancel = CreateStyledButton("Cancel");

            buttonsTable.Controls.Add(btnChangePassword, 0, 0);
            buttonsTable.Controls.Add(btnSave, 1, 0);
            buttonsTable.Controls.Add(btnCancel, 2, 0);

            buttonPanel.Controls.Add(buttonsTable);

            // Wire up events
            btnChangePassword.Click += BtnChangePassword_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Hide();

            // Add all components
            containerPanel.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(titleLabel);
            this.Controls.Add(containerPanel);
        }

        private Label CreateFieldLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 11F),
                ForeColor = textColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 10, 10, 10)
            };
        }

        private TextBox CreateTextBox()
        {
            return new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 10, 0, 10),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };
        }

        private Button CreateStyledButton(string text, bool isPrimary = false)
        {
            var button = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = isPrimary ? primaryColor : Color.FromArgb(240, 240, 240),
                ForeColor = isPrimary ? Color.White : textColor,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand,
                Size = new Size(140, 35),
                Margin = new Padding(5),
                Anchor = AnchorStyles.None
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void ApplyModernStyling()
        {
            // Add hover effects for buttons
            foreach (Control control in this.Controls)
            {
                if (control is Panel panel)
                {
                    foreach (Control panelControl in panel.Controls)
                    {
                        if (panelControl is TableLayoutPanel tablePanel)
                        {
                            foreach (Control button in tablePanel.Controls)
                            {
                                if (button is Button btn)
                                {
                                    bool isPrimary = btn.BackColor == primaryColor;
                                    btn.MouseEnter += (s, e) => {
                                        btn.BackColor = isPrimary ? 
                                            Color.FromArgb(48, 63, 159) : 
                                            Color.FromArgb(224, 224, 224);
                                    };
                                    btn.MouseLeave += (s, e) => {
                                        btn.BackColor = isPrimary ? 
                                            primaryColor : 
                                            Color.FromArgb(240, 240, 240);
                                    };
                                }
                            }
                        }
                    }
                }
            }

            // Add focus effects for textboxes
            foreach (Control control in mainPanel.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Enter += (s, e) => {
                        textBox.BackColor = Color.FromArgb(245, 245, 245);
                    };
                    textBox.Leave += (s, e) => {
                        textBox.BackColor = Color.White;
                    };
                }
            }
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            using (var dialog = new Form())
            {
                dialog.Text = "Change Password";
                dialog.Size = new Size(400, 350);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = backgroundColor;

                var panel = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 8,
                    Padding = new Padding(20)
                };

                // Add password fields
                var fields = new (string Label, bool IsPassword)[]
                {
                    ("Current Password", true),
                    ("New Password", true),
                    ("Confirm Password", true)
                };

                int currentRow = 0;
                foreach (var field in fields)
                {
                    var label = new Label
                    {
                        Text = field.Label,
                        Font = new Font("Segoe UI", 11F),
                        Margin = new Padding(0, currentRow == 0 ? 0 : 15, 0, 5)
                    };
                    panel.Controls.Add(label, 0, currentRow++);

                    var textBox = new TextBox
                    {
                        PasswordChar = field.IsPassword ? '•' : '\0',
                        Font = new Font("Segoe UI", 11F),
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(0, 0, 0, 5)
                    };
                    panel.Controls.Add(textBox, 0, currentRow++);
                }

                // Add buttons
                var buttonPanel = new TableLayoutPanel
                {
                    ColumnCount = 2,
                    RowCount = 1,
                    Dock = DockStyle.Bottom,
                    Height = 60,
                    Padding = new Padding(0, 10, 0, 0)
                };

                var btnSavePassword = CreateStyledButton("Save", true);
                var btnCancelPassword = CreateStyledButton("Cancel");

                btnSavePassword.Click += (s, e) => {
                    var passwords = panel.Controls.OfType<TextBox>().ToArray();
                    if (passwords.Any(p => string.IsNullOrEmpty(p.Text)))
                    {
                        MessageBox.Show("Please fill in all fields.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (passwords[1].Text != passwords[2].Text)
                    {
                        MessageBox.Show("New passwords do not match.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // TODO: Implement actual password change logic
                    MessageBox.Show("Password changed successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dialog.Close();
                };

                btnCancelPassword.Click += (s, e) => dialog.Close();

                buttonPanel.Controls.Add(btnSavePassword, 0, 0);
                buttonPanel.Controls.Add(btnCancelPassword, 1, 0);

                dialog.Controls.Add(panel);
                dialog.Controls.Add(buttonPanel);
                dialog.ShowDialog(this);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFullName.Text) || string.IsNullOrEmpty(txtEmail.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // TODO: Implement profile update logic
            MessageBox.Show("Profile updated successfully!", "Success", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Hide();
        }
    }
}
