using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace HTWebRemoteHost.Devices.Controllers
{
    class MiniDSPrsControl
    {
        private static Dictionary<string, string> _commands;
        private static Dictionary<string, string> Commands
        {
            get
            {
                _commands = new Dictionary<string, string>
                {
                    { "config1", @"{""master_status"":{""preset"":0}}" },
                    { "config2", @"{""master_status"":{""preset"":1}}" },
                    { "config3", @"{""master_status"":{""preset"":2}}" },
                    { "config4", @"{""master_status"":{""preset"":3}}" },
                    { "analog", @"{""master_status"":{""source"": ""Analog""}}" },
                    { "toslink", @"{""master_status"":{""source"": ""Toslink""}}" },
                    { "usb", @"{""master_status"":{""source"": ""Usb""}}" },
                    { "mute", @"{""master_status"":{""mute"": true}}" },
                    { "unmute", @"{""master_status"":{""mute"": false}}" },
                    { "volume", @"{""master_status"":{""volume"": xx}}" }
                };

                return _commands;
            }
        }
        public static void RunCmd(string IP, string cmd, string param)
        {
            try
            {
                bool valid = Commands.TryGetValue(cmd, out string jsonCmd);

                if (cmd == "volume")
                {
                    jsonCmd = jsonCmd.Replace("xx", param);
                }

                if (valid)
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        httpClient.Timeout = TimeSpan.FromSeconds(3);

                        StringContent jsonContent = new StringContent(jsonCmd, Encoding.UTF8, "application/json");
                        _ = httpClient.PostAsync($"http://{IP}:5380/devices/0/config", jsonContent).Result;
                    }
                }
                else
                {
                    Util.ErrorHandler.SendError($@"Command: ""{cmd}"" not found.");
                }
            }
            catch (Exception e)
            {
                Util.ErrorHandler.SendError($"Cannot send command to to minidsp-rs at {IP}\n\n{e.Message}");
            }
        }

        public static string getvol(string IP)
        {
            string volume = "??";
            string jsonStatus = "";

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(3);

                try
                {
                    jsonStatus = httpClient.GetStringAsync($"http://{IP}:5380/devices/0").Result;
                }
                catch (Exception e)
                {
                    Util.ErrorHandler.SendError($"Error querying minidsp-rs at: http://{IP}\n\n{e.Message}");
                }
            }

            try
            {
                JObject status = JObject.Parse(jsonStatus);
                volume = (string)status.SelectToken("master.volume");
            }
            catch { }

            return volume;
        }
    }
}