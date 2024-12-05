using System;
using System.Windows.Forms;
using ExpenseTracker.Database;
using System.Drawing;
using System.Linq;

namespace ExpenseTracker.Forms
{
    public partial class ExpenseListForm : Form
    {
        private ComboBox cbYear;
        private ComboBox cbMonth;
        private ComboBox cbCategory;
        private ComboBox cbSource;
        private Button btnFilter;
        private Button btnClearFilter;
        private DataGridView dgvExpenses;
        private Label lblTotalDebit;
        private Label lblTotalCredit;
        private Label lblBalance;
        private Panel mainContainer;
        private Panel filterPanel;
        private Panel summaryPanel;
        private Panel gridPanel;
        private TableLayoutPanel filterLayout;
        private readonly string currentUsername;

        public ExpenseListForm(string username)
        {
            currentUsername = username;
            InitializeFormControls();
            LoadData();
            this.Resize += ExpenseListForm_Resize;
        }

        private void InitializeFormControls()
        {
            this.Text = "View All Expenses";
            this.MinimumSize = new Size(800, 600);
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // Main container panel
            mainContainer = new Panel
            {
                Dock = DockStyle.Fill
            };

            // Filter Panel with TableLayoutPanel
            filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(20, 15, 20, 15),
                BackColor = System.Drawing.Color.WhiteSmoke,
                BorderStyle = BorderStyle.Fixed3D,
                Margin = new Padding(5)
            };

            filterLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 2,
                Height = 70,
                Padding = new Padding(0),
                ColumnStyles = 
                {
                    new ColumnStyle(SizeType.Percent, 16.66f),
                    new ColumnStyle(SizeType.Percent, 16.66f),
                    new ColumnStyle(SizeType.Percent, 16.66f),
                    new ColumnStyle(SizeType.Percent, 16.66f),
                    new ColumnStyle(SizeType.Percent, 33.33f),
                    new ColumnStyle(SizeType.Absolute, 0)
                },
                RowStyles =
                {
                    new RowStyle(SizeType.Absolute, 25),
                    new RowStyle(SizeType.Absolute, 35)
                }
            };

            // Initialize filter controls
            InitializeFilterControls();

            // Summary Panel
            summaryPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10),
                BackColor = System.Drawing.Color.WhiteSmoke,
                BorderStyle = BorderStyle.Fixed3D
            };

            // Grid Panel
            gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            // Initialize DataGridView
            InitializeDataGridView();

            // Initialize Summary Labels
            InitializeSummaryLabels();

            // Add controls to main container
            mainContainer.Controls.Add(gridPanel);
            mainContainer.Controls.Add(summaryPanel);
            mainContainer.Controls.Add(filterPanel);

            this.Controls.Add(mainContainer);
        }

        private void InitializeFilterControls()
        {
            // Year Controls
            var lblYear = new Label { 
                Text = "Year:", 
                Dock = DockStyle.Fill, 
                TextAlign = ContentAlignment.BottomLeft,
                Margin = new Padding(0, 0, 0, 5)
            };
            cbYear = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 10, 0)
            };
            PopulateYearComboBox();

            // Month Controls
            var lblMonth = new Label { 
                Text = "Month:", 
                Dock = DockStyle.Fill, 
                TextAlign = ContentAlignment.BottomLeft,
                Margin = new Padding(0, 0, 0, 5)
            };
            cbMonth = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 10, 0)
            };
            PopulateMonthComboBox();

            // Category Controls
            var lblCategory = new Label { 
                Text = "Category:", 
                Dock = DockStyle.Fill, 
                TextAlign = ContentAlignment.BottomLeft,
                Margin = new Padding(0, 0, 0, 5)
            };
            cbCategory = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 10, 0)
            };
            PopulateCategoryComboBox();

            // Source Controls
            var lblSource = new Label { 
                Text = "Source:", 
                Dock = DockStyle.Fill, 
                TextAlign = ContentAlignment.BottomLeft,
                Margin = new Padding(0, 0, 0, 5)
            };
            cbSource = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 10, 0)
            };
            PopulateSourceComboBox();

            // Buttons
            btnFilter = new Button
            {
                Text = "Apply Filter",
                Dock = DockStyle.None,
                Height = 28,
                Width = 300,
                BackColor = System.Drawing.Color.FromArgb(0, 122, 204),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(this.Font.FontFamily, 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 10, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnFilter.Click += BtnFilter_Click;

            btnClearFilter = new Button
            {
                Text = "Clear Filter",
                Dock = DockStyle.None,
                Height = 28,
                Width = 300,
                BackColor = System.Drawing.Color.FromArgb(153, 153, 153),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(this.Font.FontFamily, 9, FontStyle.Regular),
                Margin = new Padding(0, 0, 10, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnClearFilter.Click += BtnClearFilter_Click;

            // Add controls to filter layout
            filterLayout.Controls.Add(lblYear, 0, 0);
            filterLayout.Controls.Add(cbYear, 0, 1);
            filterLayout.Controls.Add(lblMonth, 1, 0);
            filterLayout.Controls.Add(cbMonth, 1, 1);
            filterLayout.Controls.Add(lblCategory, 2, 0);
            filterLayout.Controls.Add(cbCategory, 2, 1);
            filterLayout.Controls.Add(lblSource, 3, 0);
            filterLayout.Controls.Add(cbSource, 3, 1);

            // Create a panel for buttons to center them
            var buttonPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                ColumnStyles =
                {
                    new ColumnStyle(SizeType.Percent, 50),
                    new ColumnStyle(SizeType.Percent, 50)
                },
                Margin = new Padding(0)
            };
            buttonPanel.Controls.Add(btnFilter, 0, 0);
            buttonPanel.Controls.Add(btnClearFilter, 1, 0);

            filterLayout.Controls.Add(buttonPanel, 4, 1);
            filterLayout.SetColumnSpan(buttonPanel, 2);

            filterPanel.Controls.Add(filterLayout);
        }

        private void InitializeDataGridView()
        {
            dgvExpenses = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None,
                RowsDefaultCellStyle = new DataGridViewCellStyle 
                { 
                    BackColor = System.Drawing.Color.White,
                    SelectionBackColor = System.Drawing.Color.FromArgb(230, 240, 250)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle 
                { 
                    BackColor = System.Drawing.Color.FromArgb(250, 250, 250),
                    SelectionBackColor = System.Drawing.Color.FromArgb(230, 240, 250)
                }
            };

            dgvExpenses.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { 
                    Name = "Id", 
                    HeaderText = "Id", 
                    DataPropertyName = "Id",
                    Visible = false
                },
                new DataGridViewTextBoxColumn { 
                    Name = "Date", 
                    HeaderText = "Date", 
                    DataPropertyName = "Date",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "dd-MM-yyyy" }
                },
                new DataGridViewTextBoxColumn { 
                    Name = "Category", 
                    HeaderText = "Category", 
                    DataPropertyName = "Category"
                },
                new DataGridViewTextBoxColumn { 
                    Name = "DebitAmount", 
                    HeaderText = "Debit (₹)", 
                    DataPropertyName = "DebitAmount",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
                },
                new DataGridViewTextBoxColumn { 
                    Name = "CreditAmount", 
                    HeaderText = "Credit (₹)", 
                    DataPropertyName = "CreditAmount",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
                },
                new DataGridViewTextBoxColumn { 
                    Name = "Source", 
                    HeaderText = "Source", 
                    DataPropertyName = "Source"
                },
                new DataGridViewTextBoxColumn { 
                    Name = "Notes", 
                    HeaderText = "Notes", 
                    DataPropertyName = "Notes"
                }
            });

            gridPanel.Controls.Add(dgvExpenses);
        }

        private void InitializeSummaryLabels()
        {
            var summaryLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };

            lblTotalDebit = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            lblTotalCredit = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            lblBalance = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight };

            summaryLayout.Controls.Add(lblTotalDebit, 0, 0);
            summaryLayout.Controls.Add(lblTotalCredit, 1, 0);
            summaryLayout.Controls.Add(lblBalance, 2, 0);

            summaryPanel.Controls.Add(summaryLayout);
        }

        private void ExpenseListForm_Resize(object sender, EventArgs e)
        {
            // Adjust filter panel height based on DPI and form width
            filterPanel.Height = (int)(80 * (this.Width / 1200.0));
            filterPanel.Height = Math.Max(80, Math.Min(filterPanel.Height, 120));

            // Adjust summary panel height
            summaryPanel.Height = (int)(50 * (this.Width / 1200.0));
            summaryPanel.Height = Math.Max(40, Math.Min(summaryPanel.Height, 80));

            // Force layout update
            this.PerformLayout();
        }

        private void PopulateYearComboBox()
        {
            cbYear.Items.Add("All Years");
            var currentYear = DateTime.Now.Year;
            for (int year = currentYear; year >= currentYear - 5; year--)
            {
                cbYear.Items.Add(year);
            }
            cbYear.SelectedIndex = 0;
        }

        private void PopulateMonthComboBox()
        {
            cbMonth.Items.Add("All Months");
            for (int month = 1; month <= 12; month++)
            {
                cbMonth.Items.Add(new System.Globalization.DateTimeFormatInfo().GetMonthName(month));
            }
            cbMonth.SelectedIndex = 0;
        }

        private void PopulateCategoryComboBox()
        {
            cbCategory.Items.Add("All Categories");
            cbCategory.Items.AddRange(new[] { "Groceries", "Mutton", "Dining Out", "Transportation", "Utilities", "Other" });
            cbCategory.SelectedIndex = 0;
        }

        private void PopulateSourceComboBox()
        {
            cbSource.Items.Add("All Sources");
            cbSource.Items.AddRange(new[] { "CASH", "CREDIT CARD" });
            cbSource.SelectedIndex = 0;
        }

        private void LoadData()
        {
            try
            {
                int? year = cbYear.SelectedIndex > 0 ? (int?)Convert.ToInt32(cbYear.SelectedItem) : null;
                int? month = cbMonth.SelectedIndex > 0 ? (int?)cbMonth.SelectedIndex : null;
                string category = cbCategory.SelectedIndex > 0 ? cbCategory.SelectedItem.ToString() : null;
                string source = cbSource.SelectedIndex > 0 ? cbSource.SelectedItem.ToString() : null;

                var expenses = DatabaseHelper.GetFilteredExpenses(year, month, category, source);
                
                // Remove any null entries and sort by date
                var filteredExpenses = expenses
                    .Where(e => e != null)
                    .OrderByDescending(e => e.Date)
                    .ToList();

                dgvExpenses.DataSource = filteredExpenses;

                // Update summary
                decimal totalDebit = filteredExpenses.Sum(e => e.DebitAmount);
                decimal totalCredit = filteredExpenses.Sum(e => e.CreditAmount);
                decimal balance = totalCredit - totalDebit;

                lblTotalDebit.Text = $"Total Debit: ₹{totalDebit:N2}";
                lblTotalCredit.Text = $"Total Credit: ₹{totalCredit:N2}";
                lblBalance.Text = $"Balance: ₹{balance:N2}";
                lblBalance.ForeColor = balance >= 0 ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            cbYear.SelectedIndex = 0;
            cbMonth.SelectedIndex = 0;
            cbCategory.SelectedIndex = 0;
            cbSource.SelectedIndex = 0;
            LoadData();
        }
    }
}
