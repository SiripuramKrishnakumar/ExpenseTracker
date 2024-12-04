using System;
using System.Windows.Forms;
using ExpenseTracker.Database;

namespace ExpenseTracker.Forms
{
    public partial class BalanceSheetForm : BaseForm
    {
        public BalanceSheetForm(string username) : base(username)
        {
            InitializeComponent();
            PopulateBalanceSheet();
        }

        private void InitializeComponent()
        {
            // Create TableLayoutPanel for better alignment
            tableLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.None,
                Location = new System.Drawing.Point(100, 100),
                Size = new System.Drawing.Size(600, 400),
                ColumnCount = 2,
                RowCount = 5,
                Anchor = AnchorStyles.None,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };

            // Add column styles for better alignment
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Add row styles for equal spacing
            for (int i = 0; i < 5; i++)
            {
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            }

            // Add labels for balance sheet items
            string[] labels = { "Cash Balance", "Credit Card Balance", "Total Expenses", "Total Income", "Net Worth" };
            
            for (int i = 0; i < labels.Length; i++)
            {
                var labelName = new Label
                {
                    Text = labels[i],
                    AutoSize = true,
                    Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 12),
                    Anchor = AnchorStyles.None
                };
                
                var labelValue = new Label
                {
                    Name = $"lbl{labels[i].Replace(" ", "")}Value",
                    AutoSize = true,
                    Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 12, System.Drawing.FontStyle.Bold),
                    Anchor = AnchorStyles.None
                };

                tableLayoutPanel.Controls.Add(labelName, 0, i);
                tableLayoutPanel.Controls.Add(labelValue, 1, i);
            }

            // Back to Dashboard Button
            btnBackToDashboard = new Button
            {
                Text = "Back to Dashboard",
                Location = new System.Drawing.Point(350, 520),
                Size = new System.Drawing.Size(150, 30),
                Anchor = AnchorStyles.None
            };
            btnBackToDashboard.Click += BtnBackToDashboard_Click;

            // Add controls to form
            this.Controls.Add(tableLayoutPanel);
            this.Controls.Add(btnBackToDashboard);

            this.Text = "Balance Sheet";
            this.Size = new System.Drawing.Size(800, 600);
        }

        private void PopulateBalanceSheet()
        {
            var expenses = DatabaseHelper.GetAllExpenses();

            // Calculate balances
            decimal cashBalance = expenses
                .Where(e => e.Source == "Cash")
                .Sum(e => e.CreditAmount - e.DebitAmount);

            decimal creditCardBalance = expenses
                .Where(e => e.Source == "Credit Card")
                .Sum(e => e.CreditAmount - e.DebitAmount);

            decimal totalExpenses = expenses.Sum(e => e.DebitAmount);
            decimal totalIncome = expenses.Sum(e => e.CreditAmount);
            decimal netWorth = totalIncome - totalExpenses;

            // Update labels
            tableLayoutPanel.Controls.Find("lblCashBalanceValue", true)[0].Text = $"₹{cashBalance:N2}";
            tableLayoutPanel.Controls.Find("lblCreditCardBalanceValue", true)[0].Text = $"₹{creditCardBalance:N2}";
            tableLayoutPanel.Controls.Find("lblTotalExpensesValue", true)[0].Text = $"₹{totalExpenses:N2}";
            tableLayoutPanel.Controls.Find("lblTotalIncomeValue", true)[0].Text = $"₹{totalIncome:N2}";
            tableLayoutPanel.Controls.Find("lblNetWorthValue", true)[0].Text = $"₹{netWorth:N2}";
        }

        private void BtnBackToDashboard_Click(object sender, EventArgs e)
        {
            BackToDashboard();
        }

        // Form controls
        private TableLayoutPanel tableLayoutPanel;
        private Button btnBackToDashboard;
    }
}
