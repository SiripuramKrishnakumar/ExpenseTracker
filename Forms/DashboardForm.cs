using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ExpenseTracker.Database;
using System.Linq;
using System.Drawing;
using System.ComponentModel;

namespace ExpenseTracker.Forms
{
    public partial class DashboardForm : BaseForm
    {
        private IContainer components = null;
        private Chart chartExpensesBySource;
        private Chart chartExpensesByCategory;
        private Chart chartMonthlyTrend;
        private Label lblTotalExpenses;
        private Label lblTotalIncome;
        private Label lblNetBalance;
        private Label lblExpenseChange;
        private Label lblIncomeChange;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnApplyFilter;
        private TableLayoutPanel mainLayout;
        private TableLayoutPanel summaryPanel;
        private TableLayoutPanel chartsPanel;
        private TableLayoutPanel filterPanel;
        private string currentUsername;

        public DashboardForm(string username) : base(username)
        {
            this.currentUsername = username;
            InitializeComponent();
            InitializeFormControls();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            this.SuspendLayout();
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Name = "DashboardForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Expense Dashboard";
            
            this.ResumeLayout(false);
        }

        private void InitializeFormControls()
        {
            try
            {
                this.Text = "Expense Dashboard";
                this.Size = new Size(1200, 800);
                this.WindowState = FormWindowState.Maximized;

                // Initialize date pickers with default values
                dtpStartDate = new DateTimePicker
                {
                    Value = DateTime.Today.AddMonths(-1),
                    Format = DateTimePickerFormat.Short
                };

                dtpEndDate = new DateTimePicker
                {
                    Value = DateTime.Today,
                    Format = DateTimePickerFormat.Short
                };

                // Main layout
                mainLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 3,
                    Padding = new Padding(20),
                    BackColor = Color.White
                };

                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));  // Filter panel
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Summary panel
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Charts panel

                // Initialize all panels
                InitializeFilterPanel();
                InitializeSummaryPanel();
                InitializeChartsPanel();

                // Add panels to main layout
                mainLayout.Controls.Add(filterPanel, 0, 0);
                mainLayout.Controls.Add(summaryPanel, 0, 1);
                mainLayout.Controls.Add(chartsPanel, 0, 2);

                this.Controls.Add(mainLayout);

                // Load initial data
                LoadDashboardData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeFilterPanel()
        {
            filterPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1,
                BackColor = Color.WhiteSmoke,
                Margin = new Padding(0, 0, 0, 10)
            };

            var lblStartDate = new Label
            {
                Text = "Start Date:",
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };

            var lblEndDate = new Label
            {
                Text = "End Date:",
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };

            dtpStartDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddMonths(-1),
                Dock = DockStyle.Fill
            };

            dtpEndDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Dock = DockStyle.Fill
            };

            btnApplyFilter = new Button
            {
                Text = "Apply Filter",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnApplyFilter.Click += (s, e) => LoadDashboardData();

            filterPanel.Controls.Add(lblStartDate, 0, 0);
            filterPanel.Controls.Add(dtpStartDate, 1, 0);
            filterPanel.Controls.Add(lblEndDate, 2, 0);
            filterPanel.Controls.Add(dtpEndDate, 3, 0);
            filterPanel.Controls.Add(btnApplyFilter, 4, 0);
        }

        private void InitializeSummaryPanel()
        {
            summaryPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3,
                BackColor = Color.WhiteSmoke,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Margin = new Padding(0, 0, 0, 20)
            };

            // Labels for titles
            var lblExpensesTitle = new Label
            {
                Text = "Total Expenses",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            var lblIncomeTitle = new Label
            {
                Text = "Total Income",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            var lblBalanceTitle = new Label
            {
                Text = "Net Balance",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            // Labels for values
            lblTotalExpenses = new Label
            {
                Text = "₹0.00",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Red
            };

            lblTotalIncome = new Label
            {
                Text = "₹0.00",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Green
            };

            lblNetBalance = new Label
            {
                Text = "₹0.00",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };

            // Labels for percentage change
            lblExpenseChange = new Label
            {
                Text = "",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Segoe UI", 9)
            };

            lblIncomeChange = new Label
            {
                Text = "",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Segoe UI", 9)
            };

            // Add controls to summary panel
            summaryPanel.Controls.Add(lblExpensesTitle, 0, 0);
            summaryPanel.Controls.Add(lblIncomeTitle, 1, 0);
            summaryPanel.Controls.Add(lblBalanceTitle, 2, 0);
            summaryPanel.Controls.Add(lblTotalExpenses, 0, 1);
            summaryPanel.Controls.Add(lblTotalIncome, 1, 1);
            summaryPanel.Controls.Add(lblNetBalance, 2, 1);
            summaryPanel.Controls.Add(lblExpenseChange, 0, 2);
            summaryPanel.Controls.Add(lblIncomeChange, 1, 2);
        }

        private void InitializeChartsPanel()
        {
            chartsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.White,
                ColumnStyles = 
                {
                    new ColumnStyle(SizeType.Percent, 50),
                    new ColumnStyle(SizeType.Percent, 50)
                },
                RowStyles = 
                {
                    new RowStyle(SizeType.Percent, 50),
                    new RowStyle(SizeType.Percent, 50)
                }
            };

            // Initialize charts
            chartExpensesBySource = CreateChart("Expenses by Source", SeriesChartType.Pie);
            chartExpensesByCategory = CreateChart("Expenses by Category", SeriesChartType.Doughnut);
            chartMonthlyTrend = CreateChart("Monthly Trend", SeriesChartType.Column);

            // Add charts to panel with proper spacing
            chartsPanel.Controls.Add(chartExpensesBySource, 0, 0);
            chartsPanel.Controls.Add(chartExpensesByCategory, 1, 0);
            chartsPanel.Controls.Add(chartMonthlyTrend, 0, 1);
            chartsPanel.SetColumnSpan(chartMonthlyTrend, 2);
        }

        private Chart CreateChart(string title, SeriesChartType chartType)
        {
            var chart = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(10)
            };

            // Add Chart Area
            var chartArea = new ChartArea("MainArea");
            chartArea.BackColor = Color.White;
            chart.ChartAreas.Add(chartArea);

            // Add Legend
            var legend = new Legend("MainLegend")
            {
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center
            };
            chart.Legends.Add(legend);

            // Add Title
            var titleArea = new Title
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            chart.Titles.Add(titleArea);

            // Add Series
            var series = new Series("MainSeries")
            {
                ChartType = chartType,
                ChartArea = "MainArea",
                Legend = "MainLegend"
            };

            if (chartType == SeriesChartType.Pie || chartType == SeriesChartType.Doughnut)
            {
                series["PieLabelStyle"] = "Outside";
                series.IsValueShownAsLabel = true;
                series.Label = "#PERCENT{P0}";
            }
            else
            {
                series.IsValueShownAsLabel = true;
                series.LabelFormat = "₹{0:#,##0}";
            }

            chart.Series.Add(series);
            return chart;
        }

        private void LoadDashboardData()
        {
            try
            {
                var allExpenses = DatabaseHelper.GetAllExpenses();
                
                if (allExpenses == null || !allExpenses.Any())
                {
                    MessageBox.Show("No expense data found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
               
                // Apply date filter
                var expenses = allExpenses.Where(e => 
                    e.Date >= dtpStartDate?.Value.Date && 
                    e.Date <= dtpEndDate?.Value.Date).ToList();

                // Update summary values
                decimal totalExpenses = expenses.Sum(e => e.DebitAmount);
                decimal totalIncome = expenses.Sum(e => e.CreditAmount);
                decimal netBalance = totalIncome - totalExpenses;

                // Update labels
                if (lblTotalExpenses != null) lblTotalExpenses.Text = $"₹{totalExpenses:#,##0}";
                if (lblTotalIncome != null) lblTotalIncome.Text = $"₹{totalIncome:#,##0}";
                if (lblNetBalance != null)
                {
                    lblNetBalance.Text = $"₹{netBalance:#,##0}";
                    lblNetBalance.ForeColor = netBalance >= 0 ? Color.Green : Color.Red;
                }

                // Clear and reload charts
                if (chartExpensesBySource?.Series.Count > 0)
                    chartExpensesBySource.Series[0].Points.Clear();
                if (chartExpensesByCategory?.Series.Count > 0)
                    chartExpensesByCategory.Series[0].Points.Clear();
                if (chartMonthlyTrend != null)
                    chartMonthlyTrend.Series.Clear();

                // Load Source Chart
                var sourceData = expenses
                    .GroupBy(e => string.IsNullOrEmpty(e.Source) ? "Other" : e.Source)
                    .Select(g => new { Name = g.Key, Value = g.Sum(e => e.DebitAmount) })
                    .Where(x => x.Value > 0)
                    .OrderByDescending(x => x.Value)
                    .ToList();

                if (sourceData.Any() && chartExpensesBySource?.Series.Count > 0)
                {
                    foreach (var item in sourceData)
                    {
                        chartExpensesBySource.Series[0].Points.AddXY(item.Name, item.Value);
                    }
                }

                // Load Category Chart
                var categoryData = expenses
                    .GroupBy(e => string.IsNullOrEmpty(e.Category) ? "Uncategorized" : e.Category)
                    .Select(g => new { Name = g.Key, Value = g.Sum(e => e.DebitAmount) })
                    .Where(x => x.Value > 0)
                    .OrderByDescending(x => x.Value)
                    .ToList();

                if (categoryData.Any() && chartExpensesByCategory?.Series.Count > 0)
                {
                    foreach (var item in categoryData)
                    {
                        chartExpensesByCategory.Series[0].Points.AddXY(item.Name, item.Value);
                    }
                }

                // Load Monthly Trend
                if (chartMonthlyTrend != null)
                {
                    var expenseSeries = new Series("Expenses")
                    {
                        ChartType = SeriesChartType.Column,
                        IsValueShownAsLabel = true,
                        LabelFormat = "₹{0:#,##0}",
                        Color = Color.FromArgb(220, 80, 80)
                    };

                    var incomeSeries = new Series("Income")
                    {
                        ChartType = SeriesChartType.Column,
                        IsValueShownAsLabel = true,
                        LabelFormat = "₹{0:#,##0}",
                        Color = Color.FromArgb(80, 180, 80)
                    };

                    var monthlyData = expenses
                        .GroupBy(e => new { e.Date.Year, e.Date.Month })
                        .Select(g => new
                        {
                            Date = $"{g.Key.Year}-{g.Key.Month:00}",
                            Expenses = g.Sum(e => e.DebitAmount),
                            Income = g.Sum(e => e.CreditAmount)
                        })
                        .OrderBy(x => x.Date)
                        .ToList();

                    if (monthlyData.Any())
                    {
                        foreach (var month in monthlyData)
                        {
                            expenseSeries.Points.AddXY(month.Date, month.Expenses);
                            incomeSeries.Points.AddXY(month.Date, month.Income);
                        }

                        chartMonthlyTrend.Series.Add(expenseSeries);
                        chartMonthlyTrend.Series.Add(incomeSeries);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LoadDashboardData(); // Refresh charts on resize
        }
    }
}
