using System;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Source { get; set; }
        public string Notes { get; set; }
    }
}
