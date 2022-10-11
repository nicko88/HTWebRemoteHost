using System.Diagnostics;

namespace HTWebRemoteHost.Devices.Controllers
{
    class PCControl
    {
        public static void RunCmd(string cmd)
        {
            cmd.Bash();
        }
    }

    public static class ShellHelper
    {
        public static string Bash(this string cmd)
        {
            string escapedArgs = cmd.Replace("\"", "\\\"");

            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }
}