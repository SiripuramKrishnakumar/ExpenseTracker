using System;
using System.Windows.Forms;
using ExpenseTracker.Database;
using ExpenseTracker.Forms;
using ExpenseTracker.Auth;
using System.Globalization;

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

            //System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("hi-IN");
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;


            // Start with Login Form
            Application.Run(new LoginForm());
        }
    }
}
