using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ExpenseTracker.Database;

namespace ExpenseTracker.Forms
{
    public partial class BalanceSheetForm : Form
    {
        private TableLayoutPanel mainPanel;
        private Button btnClose;
        private readonly string currentUsername;
        private readonly Color primaryColor = Color.FromArgb(63, 81, 181);
        private readonly Color textColor = Color.FromArgb(33, 33, 33);
        private readonly Color positiveColor = Color.FromArgb(76, 175, 80);
        private readonly Color negativeColor = Color.FromArgb(244, 67, 54);

        public BalanceSheetForm(string username)
        {
            currentUsername = username;
            InitializeComponent();
            PopulateBalanceSheet();
            ApplyModernStyling();
        }

        private void InitializeComponent()
        {
            this.Text = "Balance Sheet";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Main container
            var containerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            // Create main panel
            mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                BackColor = Color.White,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                ColumnStyles = 
                {
                    new ColumnStyle(SizeType.Percent, 50),
                    new ColumnStyle(SizeType.Percent, 50)
                }
            };

            // Add title
            var titleLabel = new Label
            {
                Text = "Balance Sheet",
                Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold),
                ForeColor = primaryColor,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 50
            };

            // Add balance sheet items with improved spacing
            string[] labels = { "Cash Balance", "Credit Card Balance", "Total Expenses", "Total Income", "Net Worth" };
            
            for (int i = 0; i < labels.Length; i++)
            {
                var itemPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(10),
                    Padding = new Padding(15),
                    BackColor = Color.FromArgb(250, 250, 250)
                };

                var labelName = new Label
                {
                    Text = labels[i],
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = textColor,
                    Dock = DockStyle.Left,
                    AutoSize = true
                };
                
                var labelValue = new Label
                {
                    Name = $"lbl{labels[i].Replace(" ", "")}Value",
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = primaryColor,
                    Dock = DockStyle.Right,
                    AutoSize = true
                };

                itemPanel.Controls.Add(labelValue);
                itemPanel.Controls.Add(labelName);
                mainPanel.Controls.Add(itemPanel, i % 2, i / 2 + 1);
            }

            // Add close button
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(0, 10, 0, 0)
            };

            btnClose = new Button
            {
                Text = "Close",
                Size = new Size(120, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = primaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand,
                Location = new Point((buttonPanel.Width - 120) / 2, 10)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Hide();

            buttonPanel.Controls.Add(btnClose);
            
            // Layout components
            containerPanel.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(titleLabel);
            this.Controls.Add(containerPanel);
        }

        private void ApplyModernStyling()
        {
            // Add hover effects
            btnClose.MouseEnter += (s, e) => {
                btnClose.BackColor = Color.FromArgb(48, 63, 159);
            };
            btnClose.MouseLeave += (s, e) => {
                btnClose.BackColor = primaryColor;
            };

            // Add shadow effect to panels
            foreach (Control control in mainPanel.Controls)
            {
                if (control is Panel panel)
                {
                    panel.Paint += (s, e) => {
                        var rect = new Rectangle(0, 0, panel.Width, panel.Height);
                        using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                        {
                            path.AddRectangle(rect);
                            using (var brush = new SolidBrush(Color.FromArgb(5, 0, 0, 0)))
                            {
                                e.Graphics.FillPath(brush, path);
                            }
                        }
                    };
                }
            }
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

            // Update labels with appropriate colors
            UpdateBalanceLabel("CashBalance", cashBalance);
            UpdateBalanceLabel("CreditCardBalance", creditCardBalance);
            UpdateBalanceLabel("TotalExpenses", -totalExpenses);
            UpdateBalanceLabel("TotalIncome", totalIncome);
            UpdateBalanceLabel("NetWorth", netWorth);
        }

        private void UpdateBalanceLabel(string labelName, decimal value)
        {
            var label = mainPanel.Controls.Find($"lbl{labelName}Value", true)[0] as Label;
            if (label != null)
            {
                label.Text = $"₹{Math.Abs(value):N2}";
                label.ForeColor = value >= 0 ? positiveColor : negativeColor;
            }
        }
    }
}
