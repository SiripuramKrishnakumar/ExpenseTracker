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

        private void InitializeMenu()
        {
            mainMenu = new MenuStrip();

            //// Create menu items
            var dashboardMenuItem = new ToolStripMenuItem("Dashboard");
            dashboardMenuItem.Click += (s, e) => NavigateToForm(new DashboardForm(currentUsername));

            // Create Expense menu
            var expenseMenu = new ToolStripMenuItem("Expense");
            expenseMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Add Expense", null, OpenAddExpense),
                new ToolStripMenuItem("View Expenses", null, OpenViewExpenses)
            });

            var balanceSheetMenuItem = new ToolStripMenuItem("Balance Sheet");
            balanceSheetMenuItem.Click += (s, e) => NavigateToForm(new BalanceSheetForm(currentUsername));

            var profileMenuItem = new ToolStripMenuItem("Profile");
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
