using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Reflection;
using System.Threading;
using HTWebRemoteHost.RemoteFile;
using HTWebRemoteHost.Util;

namespace HTWebRemoteHost
{
    class HTWebRemoteHost
    {
        private string IP;
        private readonly Thread httpThread;

        public HTWebRemoteHost()
        {
            IP = ConfigHelper.GetLocalIPAddress();
            Console.WriteLine($"Listening on: http://{IP}:5000");

            httpThread = new Thread(StartListen);
            httpThread.Start();
        }

        public void StartListen()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://*:5000/");
            try
            {
                listener.Start();
            }
            catch
            {
                Console.WriteLine("Cannot open Port: 5000\n\nTry running as root.");
                Environment.Exit(0);
            }

            while (true)
            {
                IAsyncResult result = listener.BeginGetContext(new AsyncCallback(ProcessRequest), listener);
                result.AsyncWaitHandle.WaitOne();
            }
        }

        private void ProcessRequest(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string queryData = ProcessCommand(request);
            string htmlPage = ProcessResponse(request);

            if (!string.IsNullOrEmpty(queryData))
            {
                htmlPage = htmlPage.Replace("null;", "alert('" + queryData + "')");
            }

            byte[] buffer;
            if (htmlPage != null)
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(htmlPage);
            }
            else
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(" ");
            }

            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            try
            {
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
            catch { }
        }

        private string ProcessCommand(HttpListenerRequest request)
        {
            string queryData = null;

            string devtype = request.QueryString["devtype"];
            string devname = request.QueryString["devname"];
            string btnIndex = request.QueryString["btnIndex"];

            if (!string.IsNullOrEmpty(devtype))
            {
                string IP = request.QueryString["ip"];
                string cmd = request.QueryString["cmd"];
                string param = request.QueryString["param"];
                string specialData = request.QueryString["special"];

                Devices.DeviceSelector.CommandDevice(IP, devtype, cmd, param, specialData);
            }
            else if (!string.IsNullOrEmpty(devname))
            {
                string cmd = request.QueryString["cmd"];
                string param = request.QueryString["param"];

                queryData = Devices.DeviceSelector.FindDevice(devname, cmd, param);
            }
            else if (!string.IsNullOrEmpty(btnIndex))
            {
                string remoteID = request.QueryString["remoteID"];
                Remote remote = RemoteJSONLoader.LoadRemoteJSON(remoteID);

                remote.RemoteItems[Convert.ToInt32(btnIndex)].RunButtonCommands();
            }

            return queryData;
        }

        private string ProcessResponse(HttpListenerRequest request)
        {
            string htmlPage = null;
            string RemoteID = null;

            switch (request.RawUrl)
            {
                case "/":
                    RemoteID = "1";
                    break;
                case "/serial":
                    htmlPage = GetSerialPorts();
                    break;
                case "/deleteremotes":
                    DeleteRemotes();
                    break;
                case "/syncfile":
                    SaveFile(request);
                    break;
                case "/update":
                    UpdateApp();
                    break;
                case "/errorlog":
                    htmlPage = ErrorHandler.GetErrors();
                    break;
                case "/version":
                    htmlPage = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString().TrimEnd(new char[] { '.', '0' });
                    break;
                case "/rebootpc":
                    "reboot".Bash();
                    break;
                default:
                    break;
            }

            if (request.RawUrl.Length == 2)
            {
                RemoteID = request.RawUrl.Trim('/');
            }

            if (!string.IsNullOrEmpty(request.QueryString["rID"]))
            {
                RemoteID = request.QueryString["rID"];
            }

            if (RemoteID != null)
            {
                htmlPage = RemoteParser.GetRemoteHTML(RemoteID, true);
            }

            return htmlPage;
        }

        private void DeleteRemotes()
        {
            try
            {
                foreach (string file in Directory.GetFiles(ConfigHelper.WorkingPath, "HTWebRemoteButtons*"))
                {
                    File.Delete(file);
                }
            }
            catch { }
        }

        private void SaveFile(HttpListenerRequest request)
        {
            string fileName = "";
            string fileData;
            try
            {
                fileName = request.Headers["filename"];
                fileData = GetPostBody(request);

                if(fileName == "adbkey")
                {
                    File.WriteAllText("/root/.android/adbkey", fileData);
                    "adb kill-server".Bash();
                }
                else
                {
                    File.WriteAllText(Path.Combine(ConfigHelper.WorkingPath, fileName), fileData);
                }
            }
            catch
            {
                ErrorHandler.SendError($"Error saving incoming remote sync file: {fileName}");
            }
        }

        private void UpdateApp()
        {
            $"wget -O {Path.Combine(ConfigHelper.WorkingPath, "update.sh")} https://raw.githubusercontent.com/nicko88/HTWebRemoteHost/master/install/update.sh --no-check-certificate".Bash();
            $"bash {Path.Combine(ConfigHelper.WorkingPath, "update.sh")}".Bash();
        }

        private string GetSerialPorts()
        {
            string strPorts = "";

            try
            {
                string[] ports = SerialPort.GetPortNames();

                foreach (string s in ports)
                {
                    if (s.Contains("ttyS") || s.Contains("ttyUSB"))
                    {
                        strPorts += s.Substring(5) + ",";
                    }
                }
            }
            catch
            {
                strPorts = "error";
            }

            return strPorts.TrimEnd(',');
        }

        private string GetPostBody(HttpListenerRequest request)
        {
            string postData = null;
            if (request.HasEntityBody)
            {
                using Stream body = request.InputStream;
                using StreamReader reader = new StreamReader(body, request.ContentEncoding);
                postData = reader.ReadToEnd();
            }
            return postData;
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