using System;
using System.Windows.Forms;

namespace ExpenseTracker.Forms
{
    public class BaseForm : Form
    {
        protected MenuStrip mainMenu;
        protected string currentUsername;
        private const int FormWidth = 1920;
        private const int FormHeight = 1080;
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
            this.Size = new System.Drawing.Size(FormWidth, FormHeight);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Padding = new Padding(20);
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
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                Padding = new Padding(5, 2, 0, 2)
            };

            // Create menu items with custom styling
            var dashboardMenuItem = CreateMenuItem("Dashboard", "\uE80F");
            dashboardMenuItem.Click += (s, e) => NavigateToForm(new DashboardForm(currentUsername));

            // Create Expense menu
            var expenseMenu = CreateMenuItem("Expense", "\uE82E");
            var addExpenseItem = CreateMenuItem("Add Expense", "\uE710");
            var viewExpensesItem = CreateMenuItem("View Expenses", "\uE8A1");

            addExpenseItem.Click += OpenAddExpense;
            viewExpensesItem.Click += OpenViewExpenses;

            expenseMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                addExpenseItem,
                new ToolStripSeparator(),
                viewExpensesItem
            });

            var balanceSheetMenuItem = CreateMenuItem("Balance Sheet", "\uE9D2");
            balanceSheetMenuItem.Click += (s, e) => NavigateToForm(new BalanceSheetForm(currentUsername));

            var profileMenuItem = CreateMenuItem("Profile", "\uE77B");
            profileMenuItem.Click += (s, e) => NavigateToForm(new ProfileForm(currentUsername));

            // Add menu items to menu strip
            mainMenu.Items.AddRange(new ToolStripItem[] {
                dashboardMenuItem,
                expenseMenu,
                balanceSheetMenuItem,
                profileMenuItem
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
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                Padding = new Padding(8, 4, 8, 4),
                AutoSize = true,
                Height = 30
            };

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
