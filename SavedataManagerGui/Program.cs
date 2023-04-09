namespace SavedataManagerGui
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Global exception handling
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(GlobalHandler);

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        static void GlobalHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception? e = args.ExceptionObject as Exception;
            MessageBox.Show(e != null ? e.Message : "Unknown", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}