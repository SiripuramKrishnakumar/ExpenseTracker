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
        private TableLayoutPanel headerLayout;
        private TableLayoutPanel summaryPanel;
        private TableLayoutPanel chartsPanel;
        private TableLayoutPanel filterPanel;
        private new readonly string currentUsername;

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
               // this.Size = new Size(BaseForm.FormWidth, BaseForm.FormHeight);
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
                    Padding = new Padding(0,5,0,0),
                    BackColor = Color.LightGray
                };


                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));                
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 43));  // Filter panel              
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));   // Charts panel
                
                InitializeFilterPanel();
                InitializeSummaryPanel();
                InitializeChartsPanel();

                headerLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 3,
                    RowCount = 1,
                    BackColor = Color.LightGray
                };
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 400));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 450));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 600));

                headerLayout.Controls.Add(summaryPanel, 0, 0);
                headerLayout.Controls.Add(filterPanel, 2, 0);
                // Add panels to main layout
                mainLayout.Controls.Add(headerLayout, 0, 1);
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
                Width = 620,
                Height = 35,
                ColumnCount = 7,
                RowCount = 1,
                BackColor = Color.WhiteSmoke,
            };

            var lblStartDate = new Label
            {
                Text = "Start Date:",
                TextAlign = ContentAlignment.BottomRight,
                Width = 120,
                Height = 25,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(5)
            };

            var lblEndDate = new Label
            {
                Text = "End Date:",
                TextAlign = ContentAlignment.BottomRight,
                Width = 120,
                Height = 25,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(5)
            };

            dtpStartDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddMonths(-1),
                DropDownAlign = LeftRightAlignment.Left,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Width = 120,
                Height = 25,
                Margin = new Padding(5),
                BackColor = Color.White
            };

            dtpEndDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                DropDownAlign = LeftRightAlignment.Left,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Width = 120,
                Height = 25,
                Margin = new Padding(5),
                BackColor = Color.White
            };

            btnApplyFilter = new Button
            {
                Text = "Apply Filter",
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 120,
                Height = 25,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(10, 0, 10, 0),
                Anchor = AnchorStyles.None,
                Cursor = Cursors.Hand
            };
            btnApplyFilter.FlatAppearance.BorderColor = Color.FromArgb(0, 100, 195);
            btnApplyFilter.Click += (s, e) => LoadDashboardData();

            filterPanel.Controls.Add(lblStartDate, 1, 0);
            filterPanel.Controls.Add(dtpStartDate, 2, 0);
            filterPanel.Controls.Add(lblEndDate, 3, 0);
            filterPanel.Controls.Add(dtpEndDate, 4, 0);
            filterPanel.Controls.Add(btnApplyFilter, 5, 0);
        }

        private void InitializeSummaryPanel()
        {
            summaryPanel = new TableLayoutPanel
            {
                ColumnCount = 6,
                RowCount = 1,
                BackColor = Color.WhiteSmoke,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Width = 800,
                Height = 35,
            };

            // Labels for titles
            var lblExpensesTitle = new Label
            {
                Text = "Total Expenses: ",
                Width = 150,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            var lblIncomeTitle = new Label
            {
                Text = "Total Income: ",
                Width = 150,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            var lblBalanceTitle = new Label
            {
                Text = "Net Balance: ",
                Width = 150,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            // Labels for values
            lblTotalExpenses = new Label
            {
                Text = "₹0.00",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Red
            };

            lblTotalIncome = new Label
            {
                Text = "₹0.00",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Green
            };

            lblNetBalance = new Label
            {
                Text = "₹0.00",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

           

            // Add controls to summary panel
            summaryPanel.Controls.Add(lblExpensesTitle, 0, 0);
            summaryPanel.Controls.Add(lblTotalExpenses, 1, 0);
            summaryPanel.Controls.Add(lblIncomeTitle, 2, 0);
            summaryPanel.Controls.Add(lblTotalIncome, 3, 0);
            summaryPanel.Controls.Add(lblBalanceTitle, 4, 0);
            summaryPanel.Controls.Add(lblNetBalance, 5, 0);
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
