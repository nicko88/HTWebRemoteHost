using System;
using System.Net.Http;
using System.Text;

namespace HTWebRemoteHost.Devices.Controllers
{
    class PhilipsHueControl
    {
        private static string _IP;
        private static string _authKey;

        public static void RunCmd(string IP, string cmd, string param, string authKey)
        {
            _IP = IP;
            _authKey = authKey;

            //ignore these commands meant for Windows GUI
            if (cmd == "lights" || cmd == "zones" || cmd == "rooms")
            {
                return;
            }

            try
            {
                if (cmd.Contains("light") || cmd.Contains("room") || cmd.Contains("zone"))
                {
                    LightCMD(cmd, param);
                }
                else if (cmd.Contains("scene"))
                {
                    SceneCMD(cmd, param);
                }
            }
            catch (Exception e)
            {
                Util.ErrorHandler.SendError($"Error sending command to Philips Hue Bridge at: {IP}\n\n{e.AllMessages()}");
            }
        }

        private static void LightCMD(string cmd, string param)
        {
            string[] cmdVals = cmd.Split('=');

            string target = "light";
            if (cmdVals[0] == "zone" || cmdVals[0] == "room")
            {
                target = "grouped_light";
            }

            string API_URL = $"https://{_IP}/clip/v2/resource/{target}/{cmdVals[1]}";

            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            string[] cmds = param.Split(',');
            foreach (string s in cmds)
            {
                string[] vals = s.Split('=');

                switch (vals[0])
                {
                    case "on":
                        sb.Append(string.Format(@"""on"":{{""on"":{0}}},", vals[1]));
                        break;
                    case "br":
                        sb.Append(string.Format(@"""dimming"":{{""brightness"":{0}}},", vals[1]));
                        break;
                    case "ct":
                        sb.Append(string.Format(@"""color_temperature"":{{""mirek"": {0}}},", vals[1]));
                        break;
                    case "xy":
                        string[] xy = vals[1].Split(':');
                        sb.Append(string.Format(@"""color"":{{""xy"":{{""x"":{0},""y"":{1}}}}},", xy[0], xy[1]));
                        break;
                    case "inc":
                        sb.Append(string.Format(@"""dimming_delta"":{{""action"":""up"",""brightness_delta"":{0}}},", vals[1]));
                        break;
                    case "dec":
                        sb.Append(string.Format(@"""dimming_delta"":{{""action"":""down"",""brightness_delta"":{0}}},", vals[1]));
                        break;
                    case "tt":
                        sb.Append(string.Format(@"""dynamics"":{{""duration"":{0}}},", vals[1]));
                        break;
                    default:
                        break;
                }
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");

            SendHueData(API_URL, sb.ToString());
        }

        private static void SceneCMD(string cmd, string param)
        {
            string[] cmdVals = cmd.Split('=');

            string API_URL = $"https://{_IP}/clip/v2/resource/scene/{cmdVals[1]}";

            StringBuilder sb = new StringBuilder();
            sb.Append(@"{""recall"":{");

            if (param.Contains("dynamic"))
            {
                sb.Append(@"""action"":""dynamic_palette""");
            }
            else
            {
                sb.Append(@"""action"":""active""");
            }

            if (param.Contains("tt="))
            {
                string[] vals = param.Split('=');
                sb.Append(string.Format(@",""duration"":{0}", vals[1]));
            }

            sb.Append("}}");

            SendHueData(API_URL, sb.ToString());
        }

        private static string SendHueData(string URL, string payload)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => { return true; };

            using (HttpClient httpClient = new HttpClient(httpClientHandler))
            {
                httpClient.Timeout = TimeSpan.FromSeconds(3);
                httpClient.DefaultRequestHeaders.Add("hue-application-key", _authKey);
                StringContent postData = new StringContent(payload, Encoding.UTF8, "application/json");

                HttpResponseMessage result = httpClient.PutAsync(URL, postData).Result;

                string jsonResponse = result.Content.ReadAsStringAsync().Result;
                if (jsonResponse.Contains("unauthorized user"))
                {
                    Util.ErrorHandler.SendError("Unauthorized User\n\nPlease use the 'Get Hue Auth' button in the 'Configure Devices' screen to pair with the Hue Bridge.");
                }
                if (!result.IsSuccessStatusCode)
                {
                    Util.ErrorHandler.SendError($"Error sending command to Philips Hue Bridge at: {_IP}\n\nStatusCode: {result.StatusCode}\n\n{result.Content.ReadAsStringAsync().Result}");
                }

                return jsonResponse;
            }
        }

        private static string GetHueData(string URL)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => { return true; };

            using (HttpClient httpClient = new HttpClient(httpClientHandler))
            {
                httpClient.Timeout = TimeSpan.FromSeconds(3);
                httpClient.DefaultRequestHeaders.Add("hue-application-key", _authKey);

                HttpResponseMessage result = httpClient.GetAsync(URL).Result;

                string jsonResponse = result.Content.ReadAsStringAsync().Result;
                if (jsonResponse.Contains("unauthorized user"))
                {
                    Util.ErrorHandler.SendError("Unauthorized User\n\nPlease use the 'Get Hue Auth' button in the 'Configure Devices' screen to pair with the Hue Bridge.");
                }
                if (!result.IsSuccessStatusCode)
                {
                    Util.ErrorHandler.SendError($"Error getting data from Philips Hue Bridge at: {_IP}\n\nStatusCode: {result.StatusCode}\n\n{result.Content.ReadAsStringAsync().Result}");
                }

                return jsonResponse;
            }
        }
    }
}