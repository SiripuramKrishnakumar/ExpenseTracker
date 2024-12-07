using System;
using System.Windows.Forms;
using System.Drawing;

namespace ExpenseTracker.Forms
{
    public class BaseForm : Form
    {
        protected MenuStrip mainMenu;
        protected string currentUsername;
        protected const int FormWidth = 1920;
        protected const int FormHeight = 1080;
        private static Form mainDashboard; // Keep track of the main dashboard
        protected string Username { get; private set; }
        protected MenuStrip MainMenu { get; private set; }

        public BaseForm(string username)
        {
            currentUsername = username;
            Username = username;
            SetStandardFormProperties();
            InitializeMenu();

            // If this is the first dashboard instance, store it
            if (this is DashboardForm && mainDashboard == null)
            {
                mainDashboard = this;
                this.FormClosed += (s, args) => Application.Exit();
            }
        }

        private void SetStandardFormProperties()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Padding = new Padding(20);
            this.AutoSize = true;
        }

        private class CustomMenuRenderer : ToolStripProfessionalRenderer
        {
            private static readonly Color MenuBackColor = Color.FromArgb(30, 30, 30);
            private static readonly Color MenuForeColor = Color.White;
            private static readonly Color MenuHoverColor = Color.FromArgb(0, 122, 204);
            private static readonly Color MenuBorderColor = Color.FromArgb(45, 45, 45);

            public CustomMenuRenderer() : base(new CustomColorTable())
            {
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (!e.Item.Selected)
                {
                    base.OnRenderMenuItemBackground(e);
                    return;
                }

                var rc = new Rectangle(Point.Empty, e.Item.Size);
                using (var brush = new SolidBrush(MenuHoverColor))
                {
                    e.Graphics.FillRectangle(brush, rc);
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = e.Item.Selected ? Color.White : MenuForeColor;
                base.OnRenderItemText(e);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                var rc = new Rectangle(Point.Empty, e.Item.Size);
                using (var pen = new Pen(MenuBorderColor))
                {
                    e.Graphics.DrawLine(pen, rc.Left, rc.Height / 2, rc.Right, rc.Height / 2);
                }
            }
        }

        private class CustomColorTable : ProfessionalColorTable
        {
            private static readonly Color MenuBackColor = Color.FromArgb(30, 30, 30);
            private static readonly Color MenuBorderColor = Color.FromArgb(45, 45, 45);

            public override Color MenuItemSelected => Color.FromArgb(0, 122, 204);
            public override Color MenuItemBorder => MenuBorderColor;
            public override Color MenuBorder => MenuBorderColor;
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(0, 122, 204);
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(0, 122, 204);
            public override Color MenuItemPressedGradientBegin => Color.FromArgb(0, 100, 180);
            public override Color MenuItemPressedGradientEnd => Color.FromArgb(0, 100, 180);
            public override Color ToolStripDropDownBackground => MenuBackColor;
            public override Color ImageMarginGradientBegin => MenuBackColor;
            public override Color ImageMarginGradientMiddle => MenuBackColor;
            public override Color ImageMarginGradientEnd => MenuBackColor;
        }

        private void InitializeMenu()
        {
            mainMenu = new MenuStrip
            {
                Renderer = new CustomMenuRenderer(),
                BackColor = Color.FromArgb(131, 141, 201),
                Padding = new Padding(8, 4, 8, 4),
                Margin = new Padding(0),
                ImageScalingSize = new Size(20, 20)
            };

            // Create menu items with custom styling
            var dashboardMenuItem = CreateMenuItem("Dashboard", "\uE80F");
            dashboardMenuItem.Click += (s, e) => NavigateToForm(new DashboardForm(currentUsername));


            var addExpenseItem = CreateMenuItem("Add Expense", "\uE710");
            var viewExpensesItem = CreateMenuItem("View Expenses", "\uE8A1");
            addExpenseItem.Click += OpenAddExpense;
            viewExpensesItem.Click += OpenViewExpenses;

            var balanceSheetMenuItem = CreateMenuItem("Balance Sheet", "\uE9D2");
            balanceSheetMenuItem.Click += OpenBalanceSheet;

            var profileMenuItem = CreateMenuItem("Profile", "\uE77B");
            profileMenuItem.Click += OpenProfile;

            var logoutMenuItem = CreateMenuItem("Logout", "\uE77B");
            logoutMenuItem.Margin = new Padding(800, 0, 0, 0);
            logoutMenuItem.Click += BtnLogout_Click;


            // Add menu items to menu strip
            mainMenu.Items.AddRange(new ToolStripItem[] {
                dashboardMenuItem,
                addExpenseItem,
                viewExpensesItem,
                balanceSheetMenuItem,
                profileMenuItem,
                logoutMenuItem
            });

            // Add menu to form
            this.MainMenuStrip = mainMenu;
            this.Controls.Add(mainMenu);
        }


        private ToolStripMenuItem CreateMenuItem(string text, string icon)
        {
            var item = new ToolStripMenuItem
            {
                Text = text,
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.White,
                Padding = new Padding(8, 5, 8, 5),
                Margin = new Padding(0, 0, 50, 0),
                AutoSize = true,
                Height = 30,
                BackColor = Color.FromArgb(63, 81, 181),
            };

            if (!string.IsNullOrEmpty(icon))
            {
                var iconLabel = new Label
                {
                    Text = icon,
                    Font = new Font("Segoe MDL2 Assets", 12F),
                    AutoSize = true,
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                item.Text = " " + text; // Add padding for icon
                item.Image = new Bitmap(20, 20);
                using Graphics g = Graphics.FromImage(item.Image);
                g.Clear(Color.Transparent);
                using var iconFont = new Font("Segoe MDL2 Assets", 12F);
                using var brush = new SolidBrush(Color.White);
                g.DrawString(icon, iconFont, brush, 2, 2);
            }

            // Add hover event handlers for smooth animation effect
            item.MouseEnter += (s, e) =>
            {
                item.ForeColor = Color.FromArgb(0, 122, 204);
                Cursor = Cursors.Hand;
            };

            item.MouseLeave += (s, e) =>
            {
                item.ForeColor = Color.White;
                Cursor = Cursors.Default;
            };

            return item;
        }

        private void OpenDashboard(object sender, EventArgs e)
        {
            if (!(this is DashboardForm))
            {
                var dashboardForm = new DashboardForm(Username);
                dashboardForm.Show();
                this.Close();
            }
        }

        private void OpenAddExpense(object sender, EventArgs e)
        {
            var dataEntryForm = new DataEntryForm(Username);
            dataEntryForm.ShowDialog(this);
        }

        private void OpenViewExpenses(object sender, EventArgs e)
        {
            var expenseListForm = new ExpenseListForm(Username);
            expenseListForm.ShowDialog(this);
        }

        private void OpenBalanceSheet(object sender, EventArgs e)
        {
            var balanceSheetForm = new BalanceSheetForm(Username);
            balanceSheetForm.ShowDialog(this);
        }

        private void OpenProfile(object sender, EventArgs e)
        {
            var profileForm = new ProfileForm(Username);
            profileForm.ShowDialog(this);
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                var loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }


        protected void NavigateToForm(Form newForm)
        {
            if (this.GetType() != newForm.GetType())
            {
                newForm.Show();
                newForm.Location = this.Location;

                // If navigating to dashboard, show the main dashboard instance
                if (newForm is DashboardForm && mainDashboard != null && !mainDashboard.IsDisposed)
                {
                    newForm.Close();
                    mainDashboard.Show();
                    mainDashboard.Location = this.Location;
                }

                this.Hide();
            }
        }

        // Use this for back to dashboard button clicks
        protected void BackToDashboard()
        {
            if (mainDashboard != null && !mainDashboard.IsDisposed)
            {
                mainDashboard.Show();
                mainDashboard.Location = this.Location;
                this.Hide();
            }
            else
            {
                var newDashboard = new DashboardForm(currentUsername);
                mainDashboard = newDashboard;
                newDashboard.Show();
                newDashboard.Location = this.Location;
                newDashboard.FormClosed += (s, args) => Application.Exit();
                this.Hide();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (Application.OpenForms.Count <= 2) // Login form + current form
            {
                Application.Exit();
            }
        }
    }
}
