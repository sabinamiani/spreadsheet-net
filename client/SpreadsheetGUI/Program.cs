// Version 1.0 by Dylan Habersetzer and Tyler Wood

using SpreadsheetGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace SS
{
    /// <summary>
    /// Keeps track of how many top-level forms are running
    /// </summary>
    class SpreadApplicationContext : ApplicationContext
    {
        // Number of open forms
        private int formCount = 0;

        // Singleton ApplicationContext
        private static SpreadApplicationContext appContext;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private SpreadApplicationContext()
        {
        }

        /// <summary>
        /// Returns the one SpreadApplicationContext.
        /// </summary>
        public static SpreadApplicationContext getAppContext()
        {
            if (appContext == null)
            {
                appContext = new SpreadApplicationContext();
            }
            return appContext;
        }

        /// <summary>
        /// Runs the form
        /// </summary>
        public void RunForm(Form form)
        {
            // One more form is running
            formCount++;

            // When this form closes, we want to find out
            form.FormClosed += (o, e) => { if (--formCount <= 0) ExitThread(); };

            // Run the form
            form.Show();
        }

    }


    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start an application context and run one form inside it
            SpreadApplicationContext appContext = SpreadApplicationContext.getAppContext();
            appContext.RunForm(new Form1());
            Application.Run(appContext);
        }
    }
}
