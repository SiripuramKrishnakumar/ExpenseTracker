using System;
using System.Windows.Forms;
using ExpenseTracker.Database;
using ExpenseTracker.Forms;
using ExpenseTracker.Auth;

namespace ExpenseTracker
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Initialize database and auth
            DatabaseHelper.InitializeDatabase();
            AuthHelper.InitializeAuthDatabase();

            // Enable visual styles and set compatibility
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start with Login Form
            Application.Run(new LoginForm());
        }
    }
}
