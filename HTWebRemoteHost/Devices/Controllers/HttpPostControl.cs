using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HTWebRemoteHost.Devices.Controllers
{
    class HttpPostControl
    {
        public static void RunCmd(string IP, string cmd, string param, string auth)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(3);

                if (!string.IsNullOrEmpty(auth))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(auth)));
                }

                HttpResponseMessage result;
                try
                {
                    StringContent postData = new StringContent(param, Encoding.UTF8, "application/json");

                    result = httpClient.PostAsync($"{IP}{cmd}", postData).Result;

                    if (!result.IsSuccessStatusCode)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    Util.ErrorHandler.SendError($"Error sending http POST request to: {IP}{cmd}\n\n{e.Message}");
                }
            }
        }
    }
}