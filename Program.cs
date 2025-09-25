using System;
using System.Windows.Forms;

namespace DataSplitPro
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Console.WriteLine("Program started");
                Application.EnableVisualStyles();
                Console.WriteLine("Visual styles enabled");
                Application.SetCompatibleTextRenderingDefault(false);
                Console.WriteLine("Text rendering set");
                Console.WriteLine("Creating MainForm...");
                Application.Run(new MainForm());
                Console.WriteLine("Application ended");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Program.Main: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
