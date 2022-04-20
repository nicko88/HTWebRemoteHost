using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Diagnostics;

namespace HTWebRemoteHost.Util
{
    class ConfigHelper
    {
        public static string WorkingPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        public static string DeviceFile = Path.Combine(WorkingPath, "HTWebRemoteDevices.txt");
        public static string browsePaths = Path.Combine(WorkingPath, "HTWebRemoteBrowsePaths.txt");
        public static string jsonButtonFiles = Path.Combine(WorkingPath, "HTWebRemoteButtons");

        public static string GetEmbeddedResource(string filename)
        {
            string header = "";
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(filename));

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    header = reader.ReadToEnd();
                }
            }
            catch { }

            return header;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !ip.ToString().StartsWith("127"))
                {
                    return ip.ToString();
                }
            }
            return "IPerror";
        }

        public static string ConvertLegacyColor(string color)
        {
            if (color.StartsWith("#"))
            {
                return color;
            }
            else
            {
                string colorVal = "#808080";
                switch (color)
                {
                    case "Blue":
                        colorVal = "#007BFF";
                        break;
                    case "Green":
                        colorVal = "#28A745";
                        break;
                    case "Red":
                        colorVal = "#DC3545";
                        break;
                    case "Orange":
                        colorVal = "#FFC107";
                        break;
                    case "Teal":
                        colorVal = "#17A2C8";
                        break;
                    case "Gray":
                        colorVal = "#6C757D";
                        break;
                    case "White":
                        colorVal = "#F8F9FA";
                        break;
                    case "Black":
                        colorVal = "#343A40";
                        break;
                    default:
                        break;
                }

                return colorVal;
            }
        }
    }
}