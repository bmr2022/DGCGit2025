using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Helpers
{
    public static class LogHelper
    {
        private static readonly object _lock = new object();

        public static void Write(string message)
        {
            try
            {
                // Log location:
                // wwwroot/Logs/Tally
                string logFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "Logs",
                    "Tally"
                );

                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                string fileName =
                    $"PurchaseBill_{DateTime.Now:yyyy-MM-dd}.txt";

                string filePath = Path.Combine(
                    logFolder,
                    fileName
                );

                string logMessage =
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";

                lock (_lock)
                {
                    File.AppendAllText(
                        filePath,
                        logMessage + Environment.NewLine
                    );
                }
            }
            catch
            {
                // Logging failure should not break application
            }
        }

        public static void Error(string message, Exception ex)
        {
            Write(
                $"ERROR: {message}" +
                Environment.NewLine +
                $"Exception: {ex.Message}" +
                Environment.NewLine +
                $"StackTrace: {ex.StackTrace}"
            );
        }
    }
}