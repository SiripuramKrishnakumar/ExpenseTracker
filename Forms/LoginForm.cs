using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace ExpenseTracker.Forms
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private readonly Color primaryColor = Color.FromArgb(63, 81, 181);
        private readonly Color textColor = Color.FromArgb(33, 33, 33);
        private readonly Color backgroundColor = Color.White;
        private readonly string dbPath = "Database/ExpenseTracker.db";

        public LoginForm()
        {
            InitializeComponent();
            CheckDatabaseAndUser();
        }

        private void InitializeComponent()
        {
            this.Text = "Expense Tracker Login";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = backgroundColor;

            // Main container
            var containerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = backgroundColor
            };

            // Create main panel
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                BackColor = backgroundColor
            };

            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));

            // Add title
            var titleLabel = new Label
            {
                Text = "Expense Tracker",
                Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold),
                ForeColor = primaryColor,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 80
            };

            // Username
            var lblUsername = new Label
            {
                Text = "Username:",
                Font = new Font("Segoe UI", 11F),
                ForeColor = textColor,
                Height = 35,
            };
            mainPanel.Controls.Add(lblUsername, 0, 1);

            txtUsername = new TextBox
            {
                Font = new Font("Segoe UI", 12F),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 30),
                Padding = new Padding(8, 5, 8, 5),
                Height = 35,
                Width = 150,
            };
            mainPanel.Controls.Add(txtUsername, 1, 1);

            // Password
            var lblPassword = new Label
            {
                Text = "Password:",
                Font = new Font("Segoe UI", 11F),
                ForeColor = textColor,
                Height = 35,
            };
            mainPanel.Controls.Add(lblPassword, 0, 2);

            txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 12F),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '•',
                Margin = new Padding(0, 0, 0, 30),
                Padding = new Padding(8, 5, 8, 5),
                Height = 35,
                Width = 150,
            };
            mainPanel.Controls.Add(txtPassword, 1, 2);

            // Buttons panel
            var buttonPanel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Bottom,
                Height = 50,
                
            };
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            btnLogin = CreateStyledButton("Login", true);
            btnRegister = CreateStyledButton("Register", false);

            buttonPanel.Controls.Add(btnLogin, 0, 0);
            buttonPanel.Controls.Add(btnRegister, 1, 0);

            // Wire up events
            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;

            // Add focus effects for textboxes
            foreach (Control control in mainPanel.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Enter += (s, e) =>
                    {
                        textBox.BackColor = Color.FromArgb(245, 245, 245);
                    };
                    textBox.Leave += (s, e) =>
                    {
                        textBox.BackColor = Color.White;
                    };
                }
            }

            // Add all components
            containerPanel.Controls.Add(mainPanel);
            mainPanel.Controls.Add(buttonPanel, 1, 3);
            this.Controls.Add(titleLabel);
            this.Controls.Add(containerPanel);
        }

        private Button CreateStyledButton(string text, bool isPrimary)
        {
            var button = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = isPrimary ? primaryColor : Color.FromArgb(0, 120, 215),
                ForeColor = isPrimary ? Color.White : textColor,
                Font = new Font("Segoe UI", 11F),
                Cursor = Cursors.Hand,
                Size = new Size(120, 40),
                Margin = new Padding(5),
                Anchor = AnchorStyles.None,
                
            };
            button.FlatAppearance.BorderSize = 0;

            // Add hover effects
            button.MouseEnter += (s, e) =>
            {
                button.BackColor = isPrimary ?
                    Color.FromArgb(48, 63, 159) :
                    Color.FromArgb(224, 224, 224);
            };
            button.MouseLeave += (s, e) =>
            {
                button.BackColor = isPrimary ?
                    primaryColor :
                   Color.FromArgb(0, 120, 215);
            };

            return button;
        }

        private void CheckDatabaseAndUser()
        {
            try
            {
                if (!Directory.Exists("Database"))
                {
                    Directory.CreateDirectory("Database");
                }

                if (!File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                    using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        conn.Open();
                        string sql = @"
                            CREATE TABLE Users (
                                Username TEXT PRIMARY KEY,
                                Password TEXT NOT NULL,
                                FullName TEXT,
                                Email TEXT
                            );";
                        using (var cmd = new SQLiteCommand(sql, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Users", conn))
                    {
                        int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                        btnLogin.Visible = userCount > 0;
                        btnRegister.Visible = userCount == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Please enter both username and password.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                        cmd.Parameters.AddWithValue("@password", txtPassword.Text);
                        int userCount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (userCount > 0)
                        {
                            var dashboardForm = new DashboardForm(txtUsername.Text);
                            this.Hide();
                            dashboardForm.FormClosed += (s, args) => this.Close();
                            dashboardForm.Show();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Please enter both username and password.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO Users (Username, Password) VALUES (@username, @password)",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                        cmd.Parameters.AddWithValue("@password", txtPassword.Text);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Registration successful! You can now login.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        btnLogin.Visible = true;
                        btnRegister.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
