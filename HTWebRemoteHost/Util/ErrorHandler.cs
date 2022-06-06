using System;
using System.IO;

namespace HTWebRemoteHost.Util
{
    static class ErrorHandler
    {
        public static void SendError(string error)
        {
            File.AppendAllLines(Path.Combine(ConfigHelper.WorkingPath, "errorlog.txt"), new[] { $"[{DateTime.Now:G}] : {error}" + Environment.NewLine });
        }

        public static void SendMsg(string msg)
        {
            File.AppendAllLines(Path.Combine(ConfigHelper.WorkingPath, "errorlog.txt"), new[] { $"[{DateTime.Now:G}] : {msg}" + Environment.NewLine });
        }

        public static string GetErrors()
        {
            try
            {
                return File.ReadAllText(Path.Combine(ConfigHelper.WorkingPath, "errorlog.txt"));
            }
            catch
            {
                return "No errors reported since program start.";
            }
        }
    }
}