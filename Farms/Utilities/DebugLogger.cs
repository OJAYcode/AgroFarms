using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Farms.Utilities
{
    public static class DebugLogger
    {
        private static readonly string LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug_log.txt");
        private static readonly object LockObject = new object();

        public static void Log(string message)
        {
            try
            {
                lock (LockObject)
                {
                    File.AppendAllText(LogFile, $"[{DateTime.Now}] {message}{Environment.NewLine}");
                }
                Console.WriteLine($"[DEBUG] {message}");
            }
            catch
            {
                // Fail silently
            }
        }

        public static void LogException(Exception ex, string context = "")
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"[{DateTime.Now}] EXCEPTION in {context}:");
                sb.AppendLine($"Message: {ex.Message}");
                sb.AppendLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    sb.AppendLine($"Inner Exception: {ex.InnerException.Message}");
                    sb.AppendLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
                }
                
                lock (LockObject)
                {
                    File.AppendAllText(LogFile, sb.ToString());
                }
                
                Console.WriteLine($"[EXCEPTION] {context}: {ex.Message}");
            }
            catch
            {
                // Fail silently
            }
        }
    }
}
