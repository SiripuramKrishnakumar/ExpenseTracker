using System;
using System.Windows.Forms;
using ExpenseTracker.Database;
using ExpenseTracker.Models;
using System.Drawing;
using System.IO;
using ClosedXML.Excel;

namespace ExpenseTracker.Forms
{
    public partial class DataEntryForm : Form
    {
        private readonly string currentUsername;
        private DateTimePicker dtpExpenseDate;
        private ComboBox cbCategory;
        private TextBox txtDebitAmount;
        private TextBox txtCreditAmount;
        private ComboBox cbSource;
        private TextBox txtNotes;
        private Button btnAddExpense;
        private Button btnCancel;
        private Button btnBulkUpload;
        private Button btnDownloadTemplate;

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public DataEntryForm(string username)
        {
            currentUsername = username;
            InitializeFormControls();
        }

        private void InitializeFormControls()
        {
            this.Text = "Add New Expense";
            this.Size = new Size(600, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(20),
                ColumnStyles = 
                {
                    new ColumnStyle(SizeType.Percent, 30),
                    new ColumnStyle(SizeType.Percent, 70)
                }
            };

            // Add row styles for consistent spacing
            for (int i = 0; i < 6; i++)
            {
                mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            }
            // Last row for buttons - make it bigger
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));

            // Date
            var lblDate = new Label { Text = "Date:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            dtpExpenseDate = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            // Category
            var lblCategory = new Label { Text = "Category:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            cbCategory = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbCategory.Items.AddRange(new[] { "Groceries", "Transportation", "Utilities", "Other" });

            // Debit Amount
            var lblDebitAmount = new Label { Text = "Debit Amount:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            txtDebitAmount = new TextBox
            {
                Dock = DockStyle.Fill,
                PlaceholderText = "Enter debit amount"
            };

            // Credit Amount
            var lblCreditAmount = new Label { Text = "Credit Amount:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            txtCreditAmount = new TextBox
            {
                Dock = DockStyle.Fill,
                PlaceholderText = "Enter credit amount"
            };

            // Source
            var lblSource = new Label { Text = "Source:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            cbSource = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbSource.Items.AddRange(new[] { "CASH", "CREDIT CARD" });

            // Notes
            var lblNotes = new Label { Text = "Notes:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            txtNotes = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                Height = 60,
                PlaceholderText = "Enter notes (optional)"
            };

            // Buttons Panel
            var buttonsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                ColumnStyles = 
                {
                    new ColumnStyle(SizeType.Percent, 50),
                    new ColumnStyle(SizeType.Percent, 50)
                },
                Margin = new Padding(0, 5, 0, 0)
            };

            btnAddExpense = new Button
            {
                Text = "Add Expense",
                Dock = DockStyle.None,
                BackColor = Color.FromArgb(63, 81, 181),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 10, 0),
                Height = 28,
                Width = 300,
                Font = new Font(this.Font.FontFamily, 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnAddExpense.FlatAppearance.BorderSize = 0;
            btnAddExpense.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, btnAddExpense.Width, btnAddExpense.Height, 2, 2));

            btnAddExpense.Resize += (s, e) =>
            {
                btnAddExpense.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, btnAddExpense.Width, btnAddExpense.Height, 2, 2));
            };

            btnAddExpense.MouseEnter += (s, e) =>
            {
                btnAddExpense.BackColor = Color.FromArgb(63, 81, 181);
                btnAddExpense.ForeColor = Color.FromArgb(240, 240, 240);
            };
            btnAddExpense.MouseLeave += (s, e) =>
            {
                btnAddExpense.BackColor = Color.FromArgb(63, 81, 181);
                btnAddExpense.ForeColor = Color.White;
            };

            btnAddExpense.Click += BtnAddExpense_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Dock = DockStyle.None,
                BackColor = Color.FromArgb(63, 81, 181),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 10, 0),
                Height = 28,
                Width = 300,
                Font = new Font(this.Font.FontFamily, 9, FontStyle.Regular),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, btnCancel.Width, btnCancel.Height, 2, 2));

            btnCancel.Resize += (s, e) =>
            {
                btnCancel.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, btnCancel.Width, btnCancel.Height, 2, 2));
            };

            btnCancel.MouseEnter += (s, e) =>
            {
                btnCancel.BackColor = Color.FromArgb(63, 81, 181);
                btnCancel.ForeColor = Color.FromArgb(240, 240, 240);
            };
            btnCancel.MouseLeave += (s, e) =>
            {
                btnCancel.BackColor = Color.FromArgb(63, 81, 181);
                btnCancel.ForeColor = Color.White;
            };

            btnCancel.Click += BtnCancel_Click;

            // Bulk Upload Panel
            var bulkUploadPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                ColumnStyles = 
                {
                    new ColumnStyle(SizeType.Percent, 50),
                    new ColumnStyle(SizeType.Percent, 50)
                },
                Margin = new Padding(0, 5, 0, 0)
            };

            btnBulkUpload = new Button
            {
                Text = "Bulk Upload",
                Dock = DockStyle.None,
                BackColor = Color.FromArgb(63, 81, 181),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 10, 0),
                Height = 28,
                Width = 300,
                Font = new Font(this.Font.FontFamily, 9, FontStyle.Regular),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnBulkUpload.FlatAppearance.BorderSize = 0;
            btnBulkUpload.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, btnBulkUpload.Width, btnBulkUpload.Height, 2, 2));

            btnBulkUpload.Resize += (s, e) =>
            {
                btnBulkUpload.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, btnBulkUpload.Width, btnBulkUpload.Height, 2, 2));
            };

            btnBulkUpload.MouseEnter += (s, e) =>
            {
                btnBulkUpload.BackColor = Color.FromArgb(63, 81, 181);
                btnBulkUpload.ForeColor = Color.FromArgb(240, 240, 240);
            };
            btnBulkUpload.MouseLeave += (s, e) =>
            {
                btnBulkUpload.BackColor = Color.FromArgb(63, 81, 181);
                btnBulkUpload.ForeColor = Color.White;
            };

            btnBulkUpload.Click += BtnBulkUpload_Click;

            btnDownloadTemplate = new Button
            {
                Text = "Get Template",
                Dock = DockStyle.None,
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 10, 0),
                Height = 28,
                Width = 300,
                Font = new Font(this.Font.FontFamily, 9, FontStyle.Regular),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnDownloadTemplate.Click += BtnDownloadTemplate_Click;

            buttonsPanel.Controls.AddRange(new Control[] { btnAddExpense, btnCancel });
            bulkUploadPanel.Controls.AddRange(new Control[] { btnBulkUpload, btnDownloadTemplate });

            // Add controls to main panel
            mainPanel.Controls.Add(lblDate, 0, 0);
            mainPanel.Controls.Add(dtpExpenseDate, 1, 0);
            mainPanel.Controls.Add(lblCategory, 0, 1);
            mainPanel.Controls.Add(cbCategory, 1, 1);
            mainPanel.Controls.Add(lblDebitAmount, 0, 2);
            mainPanel.Controls.Add(txtDebitAmount, 1, 2);
            mainPanel.Controls.Add(lblCreditAmount, 0, 3);
            mainPanel.Controls.Add(txtCreditAmount, 1, 3);
            mainPanel.Controls.Add(lblSource, 0, 4);
            mainPanel.Controls.Add(cbSource, 1, 4);
            mainPanel.Controls.Add(lblNotes, 0, 5);
            mainPanel.Controls.Add(txtNotes, 1, 5);
            mainPanel.Controls.Add(bulkUploadPanel, 0, 6);
            mainPanel.Controls.Add(buttonsPanel, 1, 6);

            this.Controls.Add(mainPanel);
        }

        private void BtnAddExpense_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var expense = new Expense
            {
                Date = dtpExpenseDate.Value,
                Category = cbCategory.SelectedItem?.ToString(),
                DebitAmount = decimal.Parse(txtDebitAmount.Text),
                CreditAmount = decimal.Parse(txtCreditAmount.Text),
                Source = cbSource.SelectedItem?.ToString(),
                Notes = txtNotes.Text
            };

            DatabaseHelper.AddExpense(expense);
            MessageBox.Show("Expense added successfully!", "Success", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnBulkUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Select Excel File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook(openFileDialog.FileName))
                        {
                            var worksheet = workbook.Worksheet(1);
                            var rows = worksheet.RowsUsed().Skip(1); // Skip header row

                            int successCount = 0;
                            int errorCount = 0;
                            string errorMessage = "";

                            foreach (var row in rows)
                            {
                                try
                                {
                                    var expense = new Expense
                                    {
                                        Date = row.Cell(1).GetDateTime(),
                                        Category = row.Cell(2).GetString(),
                                        DebitAmount = decimal.Parse(row.Cell(3).GetString()),
                                        CreditAmount = decimal.Parse(row.Cell(4).GetString()),
                                        Source = row.Cell(5).GetString(),
                                        Notes = row.Cell(6).GetString()
                                    };

                                    if (expense.Category != "" && (expense.DebitAmount > 0 || expense.CreditAmount > 0))
                                    {
                                        DatabaseHelper.AddExpense(expense);
                                        successCount++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorCount++;
                                    errorMessage += $"Error in row {row.RowNumber()}: {ex.Message}\n";
                                }
                            }

                            string message = $"Successfully imported {successCount} expenses.\n";
                            if (errorCount > 0)
                            {
                                message += $"\n{errorCount} errors occurred:\n{errorMessage}";
                            }

                            MessageBox.Show(message, "Import Complete", 
                                MessageBoxButtons.OK, 
                                errorCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading Excel file: {ex.Message}", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnDownloadTemplate_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Save Excel Template";
                saveFileDialog.FileName = "ExpenseTemplate.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        CreateExcelTemplate(saveFileDialog.FileName);
                        MessageBox.Show("Template downloaded successfully!", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creating template: {ex.Message}", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private bool ValidateInputs()
        {
            return dtpExpenseDate != null &&
                   cbCategory.SelectedItem != null &&
                   decimal.TryParse(txtDebitAmount.Text, out _) &&
                   decimal.TryParse(txtCreditAmount.Text, out _) &&
                   cbSource.SelectedItem != null;
        }

        public static void CreateExcelTemplate(string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Expenses");

                // Add headers
                worksheet.Cell(1, 1).Value = "Date (MM/DD/YYYY)";
                worksheet.Cell(1, 2).Value = "Category";
                worksheet.Cell(1, 3).Value = "Debit Amount";
                worksheet.Cell(1, 4).Value = "Credit Amount";
                worksheet.Cell(1, 5).Value = "Source";
                worksheet.Cell(1, 6).Value = "Notes";

                // Add sample data
                worksheet.Cell(2, 1).Value = DateTime.Now.ToString("MM/dd/yyyy");
                worksheet.Cell(2, 2).Value = "Groceries";
                worksheet.Cell(2, 3).Value = "50.00";
                worksheet.Cell(2, 4).Value = "0.00";
                worksheet.Cell(2, 5).Value = "CASH";
                worksheet.Cell(2, 6).Value = "Monthly groceries";

                worksheet.Cell(3, 1).Value = DateTime.Now.ToString("MM/dd/yyyy");
                worksheet.Cell(3, 2).Value = "Transportation";
                worksheet.Cell(3, 3).Value = "30.00";
                worksheet.Cell(3, 4).Value = "0.00";
                worksheet.Cell(3, 5).Value = "CREDIT CARD";
                worksheet.Cell(3, 6).Value = "Bus fare";

                // Format headers
                var headerRange = worksheet.Range("A1:F1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save the workbook
                workbook.SaveAs(filePath);
            }
        }
    }
}
