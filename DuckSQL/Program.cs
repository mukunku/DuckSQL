using DuckSQL.Helpers;

namespace DuckSQL
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            RouteUnhandledExceptions();

            Application.Run(new MainForm());
        }

        /// <summary>
        /// When called, all unhandled exceptions within the runtime and winforms UI thread
        /// will be routed to <see cref="ExceptionHandler"/>. 
        /// </summary>
        /// <remarks>Side effect: The application will never quit when an unhandled exception happens.</remarks>
        private static void RouteUnhandledExceptions()
        {
            //If we're not debugging, route all unhandled exceptions to our top level exception handler
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // Add the event handler for handling non-UI thread exceptions to the event. 
                AppDomain.CurrentDomain.UnhandledException += new((sender, e) => ExceptionHandler((Exception)e.ExceptionObject));

                // Add the event handler for handling UI thread exceptions to the event.
                // Warning: Winforms only surfaces the inner-most exception to this handler.
                Application.ThreadException += new((sender, e) => ExceptionHandler(e.Exception));
            }
        }

        private static void ExceptionHandler(Exception ex)
        {
            MessageBox.Show($"{Resources.Errors.GenericErrorMessage} {Resources.Errors.CopyErrorMessageText}:{Env.DoubleNewLine}{ex}",
                ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}