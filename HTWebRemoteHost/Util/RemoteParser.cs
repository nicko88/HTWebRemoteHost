using HTWebRemoteHost.RemoteFile;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace HTWebRemoteHost.Util
{
    class RemoteParser
    {
        public static string GetRemoteHTML(string remoteNum, bool withTabs)
        {
            Remote remote = JSONLoader.LoadRemoteJSON(remoteNum);

            if (withTabs)
            {
                return GetHTMLHeader(remote) + GetHTMLRemoteTabs(remoteNum) + GenerateHTMLButtons(remote, remoteNum) + Environment.NewLine + "</div><script>null;</script></body></html>";
            }
            else
            {
                return GetHTMLHeader(remote) + GenerateHTMLButtons(remote, remoteNum) + Environment.NewLine + "</div><script>null;</script></body></html>";
            }
        }

        public static string GetGroupListHTML()
        {
            string header = ConfigHelper.GetEmbeddedResource("grouplistHeader.html");

            List<string> groups = new List<string>();
            List<(string, string, string)> remotes = new List<(string, string, string)>();

            string[] remoteFiles = Directory.GetFiles(ConfigHelper.WorkingPath, "HTWebRemoteButtons*").CustomSort().ToArray();

            bool hasGroups = false;
            foreach (string file in remoteFiles)
            {
                JObject oRemote = JObject.Parse(File.ReadAllText(file));

                string remoteID = (string)oRemote.SelectToken("RemoteID");
                string remoteName = (string)oRemote.SelectToken("RemoteName");
                string remoteGroup = (string)oRemote.SelectToken("RemoteGroup");
                bool? hidden = (bool?)oRemote.SelectToken("HideRemote");

                if (!hidden.HasValue || !hidden.Value)
                {
                    remotes.Add((remoteID, remoteName, remoteGroup));

                    if (!string.IsNullOrEmpty(remoteGroup) && !groups.Contains(remoteGroup))
                    {
                        groups.Add(remoteGroup);
                        hasGroups = true;
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<div class=""container body-content"">");

            int count = 0;
            foreach (string group in groups)
            {
                sb.AppendLine($@"<h4 style=""color: #FFFFFF;"">{group}</h4>");
                sb.AppendLine(@"<div class=""form-group ngroup"">");

                count = 0;
                foreach ((string, string, string) remote in remotes)
                {
                    if (remote.Item3 == group)
                    {
                        if (count % 2 == 0)
                        {
                            sb.AppendLine(@"<div class=""nrow"">");
                        }
                        sb.AppendLine($@"<div class=""nitem"" style=""flex-grow: 1;""><a href=""{remote.Item1}""><button class=""btn"" style=""background-color: #424548; color: #FFFFFF;"">{remote.Item2}</button></a></div>");
                        if (count % 2 != 0)
                        {
                            sb.AppendLine(@"</div>");
                        }
                        count++;
                    }
                }

                if (count % 2 != 0)
                {
                    sb.AppendLine(@"</div>");
                }

                sb.AppendLine(@"</div>");
            }

            if (hasGroups)
            {
                return header + Environment.NewLine + sb.ToString() + Environment.NewLine + "</div></body></html>";
            }
            else
            {
                return GetRemoteHTML("1", true);
            }
        }

        private static string GetHTMLRemoteTabs(string currentRemoteNum)
        {
            JObject oCurrentRemote = JObject.Parse(File.ReadAllText(Path.Combine(ConfigHelper.WorkingPath, $"HTWebRemoteButtons{currentRemoteNum}.json")));
            string currentRemoteGroup = (string)oCurrentRemote.SelectToken("RemoteGroup");

            StringBuilder sb = new StringBuilder();

            try
            {
                string[] remoteFiles = Directory.GetFiles(ConfigHelper.WorkingPath, "HTWebRemoteButtons*").CustomSort().ToArray();

                if (remoteFiles.Length > 1)
                {
                    sb.AppendLine(@"<div class=""nav-container"">");
                    sb.AppendLine(@"<ul class=""nav nav-tabs sticky-top"">");

                    try
                    {
                        if (File.ReadAllText(Path.Combine(ConfigHelper.WorkingPath, "HTWebRemoteSettings.txt")).Contains("GroupListButton=True"))
                        {
                            sb.AppendLine($@"<li class=""nav-item"" style=""display: contents;""><a class=""nav-link"" href=""/"">");
                            sb.AppendLine($@"<svg width=""24"" height=""24"" viewBox=""0 0 16 16"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">");
                            sb.AppendLine($@"<path d=""M8.707 1.5a1 1 0 0 0-1.414 0L.646 8.146a.5.5 0 0 0 .708.708L2 8.207V13.5A1.5 1.5 0 0 0 3.5 15h9a1.5 1.5 0 0 0 1.5-1.5V8.207l.646.647a.5.5 0 0 0 .708-.708L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293zM13 7.207V13.5a.5.5 0 0 1-.5.5h-9a.5.5 0 0 1-.5-.5V7.207l5-5z"" fill=""white"" />");
                            sb.AppendLine("</svg></a></li>");
                        }
                    }
                    catch { }

                    foreach (string file in remoteFiles)
                    {
                        if (file.Contains(".json"))
                        {
                            JObject oRemote = JObject.Parse(File.ReadAllText(file));

                            string remoteNum = (string)oRemote.SelectToken("RemoteID");
                            string remoteName = (string)oRemote.SelectToken("RemoteName");
                            string remoteGroup = (string)oRemote.SelectToken("RemoteGroup");

                            bool hideRemote = false;
                            try
                            {
                                hideRemote = (bool)oRemote.SelectToken("HideRemote");
                            }
                            catch { }

                            if (currentRemoteGroup != remoteGroup)
                            {
                                hideRemote = true;
                            }

                            if (!hideRemote)
                            {
                                if (string.IsNullOrEmpty(remoteName))
                                {
                                    remoteName = remoteNum;
                                }

                                sb.AppendLine(@"<li class=""nav-item"">");
                                if (remoteNum == currentRemoteNum)
                                {
                                    sb.AppendLine($@"<a class=""nav-link active"" href=""{remoteNum}"">{remoteName}</a>");
                                }
                                else
                                {
                                    sb.AppendLine($@"<a class=""nav-link"" href=""{remoteNum}"">{remoteName}</a>");
                                }
                                sb.AppendLine("</li>");
                            }
                        }
                    }

                    sb.AppendLine("</ul>");
                    sb.AppendLine("</div>");
                    sb.AppendLine();
                }
            }
            catch { }

            return sb.ToString();
        }

        private static string GenerateHTMLButtons(Remote remote, string RemoteNum)
        {
            bool groupStarted = false;
            bool buttonRowStarted = false;
            RemoteItem.RemoteItemType prevItemType = RemoteItem.RemoteItemType.Group;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(@"<div class=""container body-content"">");

            if (remote.RemoteItems != null)
            {
                for (int i = 0; i < remote.RemoteItems.Count; i++)
                {
                    RemoteItem item = remote.RemoteItems[i];

                    if (item.ItemType == RemoteItem.RemoteItemType.Group)
                    {
                        if (prevItemType == RemoteItem.RemoteItemType.Button || prevItemType == RemoteItem.RemoteItemType.Blank)
                        {
                            sb.AppendLine("</div>");
                            buttonRowStarted = false;
                        }

                        if (groupStarted)
                        {
                            sb.AppendLine("</div>");
                        }

                        string textColor = "#FFFFFF";
                        if (!string.IsNullOrEmpty(item.Color))
                        {
                            textColor = item.Color;
                        }

                        sb.AppendFormat(@"<h4 style=""color: {0};"">{1}</h4>" + Environment.NewLine, textColor, item.Label);
                        sb.AppendLine(@"<div class=""form-group ngroup"">");
                        groupStarted = true;
                        prevItemType = RemoteItem.RemoteItemType.Group;
                    }
                    else if (item.ItemType == RemoteItem.RemoteItemType.Blank)
                    {
                        if (!buttonRowStarted)
                        {
                            sb.AppendLine(@"<div class=""nrow"">");
                        }

                        sb.AppendFormat(@"<div class=""nitem"" style=""flex-grow: {0};""><button class=""btn""></button></div>" + Environment.NewLine, item.RelativeSize);
                        buttonRowStarted = true;
                        prevItemType = RemoteItem.RemoteItemType.Blank;
                    }
                    else if (item.ItemType == RemoteItem.RemoteItemType.NewRow)
                    {
                        if (buttonRowStarted)
                        {
                            sb.AppendLine("</div>");
                        }
                        else if (prevItemType == RemoteItem.RemoteItemType.NewRow)
                        {
                            sb.AppendLine("<br/>");
                        }
                        buttonRowStarted = false;
                        prevItemType = RemoteItem.RemoteItemType.NewRow;
                    }
                    else
                    {
                        string btnColorHex = ConfigHelper.ConvertLegacyColor(item.Color);

                        if (!buttonRowStarted)
                        {
                            sb.AppendLine(@"<div class=""nrow"">");
                        }

                        bool query = false;
                        try
                        {
                            query = item.Commands[0].Cmd.StartsWith("query:");
                        }
                        catch { }

                        string textColor = "#FFFFFF";
                        if (UseDarkColor(btnColorHex, 0.5))
                        {
                            textColor = "#000000";
                        }

                        string btnHeight = "";
                        if (item.Height > 0)
                        {
                            btnHeight = $" height: {item.Height}px;";
                        }

                        if (!query)
                        {
                            if (!item.Holdable)
                            {
                                sb.AppendFormat(@"<div class=""nitem"" style=""flex-grow: {0};""><button onclick=""sendbtn('{1}', '{2}', '{3}')"" class=""btn"" style=""background-color: {4}; color: {5};{6}"">{7}</button></div>" + Environment.NewLine, item.RelativeSize, remote.RemoteID, i, item.ConfirmPopup, btnColorHex, textColor, btnHeight, item.Label);
                            }
                            else
                            {
                                sb.AppendFormat(@"<div class=""nitem"" style=""flex-grow: {0};""><button onpointerdown=""sendbtn('{1}', '{2}', '{3}', 250)"" onpointerup=""clearInterval(intervalID)"" onpointerout=""clearInterval(intervalID)"" class=""btn"" style=""background-color: {4}; color: {5};{6}"">{7}</button></div>" + Environment.NewLine, item.RelativeSize, remote.RemoteID, i, item.ConfirmPopup, btnColorHex, textColor, btnHeight, item.Label);
                            }
                        }
                        else
                        {
                            sb.AppendFormat(@"<div class=""nitem"" style=""flex-grow: {0};""><button onclick=""sendquery('{1}', '{2}', '{3}', '{4}')"" class=""btn"" style=""background-color: {5}; color: {6};{7}"">{8}</button></div>" + Environment.NewLine, item.RelativeSize, remote.RemoteID, item.Commands[0].DeviceName, item.Commands[0].Cmd, item.ConfirmPopup, btnColorHex, textColor, btnHeight, item.Label);
                        }

                        buttonRowStarted = true;
                        prevItemType = RemoteItem.RemoteItemType.Button;
                    }
                }
            }
            else
            {
                sb.AppendLine($@"<p style=""color: white;"">No remote found for Remote #{RemoteNum}</p>");
            }

            if (groupStarted)
            {
                sb.AppendLine("</div>");
            }

            sb.AppendLine("</div>");
            return sb.ToString();
        }

        private static string GetHTMLHeader(Remote remote)
        {
            string header = ConfigHelper.GetEmbeddedResource("remoteHeader.html");

            if (string.IsNullOrEmpty(remote.RemoteBackColor))
            {
                remote.RemoteBackColor = "#000000";
            }

            int strength = remote.RemoteShadingStrength == null ? 1 : remote.RemoteShadingStrength.Value;
            int navstrength = strength;
            if (strength == 0)
            {
                navstrength = 1;
            }
            Color navColor = AddColors(ColorTranslator.FromHtml(remote.RemoteBackColor), Color.FromArgb(0.15 * 255 * navstrength > 255 ? 255 : Convert.ToInt32(0.15 * 255 * navstrength), 255, 255, 255));

            header = header.Replace("background-color: black;", $"background-color: {remote.RemoteBackColor};");

            if (UseDarkColor(remote.RemoteBackColor, 0.11))
            {
                header = header.Replace("background-color: rgba(255, 255, 255, 0.15);", $"background-color: rgba(0, 0, 0, {0.15 * strength});");
                header = header.Replace("background-color: rgba(255, 255, 255, 0.10);", $"background-color: rgba(0, 0, 0, {0.10 * strength});");
                header = header.Replace("background-color: #ffffff26;", $"background-color: rgba(0, 0, 0, {0.15 * navstrength});");
                header = header.Replace("background-color: #ffffff1a;", $"background-color: rgba(0, 0, 0, {0.10 * navstrength});");

                navColor = AddColors(ColorTranslator.FromHtml(remote.RemoteBackColor), Color.FromArgb(0.15 * 255 * navstrength > 255 ? 255 : Convert.ToInt32(0.15 * 255 * navstrength), 0, 0, 0));
            }
            else
            {
                header = header.Replace("background-color: rgba(255, 255, 255, 0.15);", $"background-color: rgba(255, 255, 255, {0.15 * strength});");
                header = header.Replace("background-color: rgba(255, 255, 255, 0.10);", $"background-color: rgba(255, 255, 255, {0.10 * strength});");
                header = header.Replace("background-color: #ffffff26;", $"background-color: rgba(255, 255, 255, {0.15 * navstrength});");
                header = header.Replace("background-color: #ffffff1a;", $"background-color: rgba(255, 255, 255, {0.10 * navstrength});");
            }

            if (UseDarkColor(ColorTranslator.ToHtml(navColor), 0.4))
            {
                header = header.Replace("--navTextColor: white;", "--navTextColor: black;");
            }

            if (strength == 0)
            {
                header = header.Replace("border-radius: 0 0 6px 6px;", "border-radius: 6px;");
                if (UseDarkColor(remote.RemoteBackColor, 0.5))
                {
                    header = header.Replace("border: 0px solid gray;", "border: 1px solid black;");
                }
                else
                {
                    header = header.Replace("border: 0px solid gray;", "border: 1px solid gray;");
                }
            }

            if (remote.ButtonHeight > 0)
            {
                header = header.Replace("height: 42px;", $"height: {remote.ButtonHeight}px;");
            }

            try
            {
                if (File.ReadAllText(Path.Combine(ConfigHelper.WorkingPath, "HTWebRemoteSettings.txt")).Contains("BottomTabs=True"))
                {
                    header = header.Replace("flex-direction: column;", "flex-direction: column-reverse;");
                    header = header.Replace("justify-content: flex-start;", "justify-content: space-between;");
                }
                else
                {
                    header = header.Replace("env(safe-area-inset-bottom)", "0");
                }
            }
            catch { }

            return header;
        }

        private static bool UseDarkColor(string hexColor, double threshold)
        {
            Color c = ColorTranslator.FromHtml(hexColor);
            double luminance = (0.299 * c.R + 0.587 * c.G + 0.114 * c.B) / 255;

            if (luminance > threshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static Color AddColors(Color background, Color alphaMask)
        {
            if (alphaMask.R == 255)
            {
                return Color.FromArgb(255,
                                      background.R + alphaMask.A > 255 ? 255 : background.R + alphaMask.A,
                                      background.G + alphaMask.A > 255 ? 255 : background.G + alphaMask.A,
                                      background.B + alphaMask.A > 255 ? 255 : background.B + alphaMask.A);
            }
            else
            {
                return Color.FromArgb(255,
                                      background.R - alphaMask.A < 0 ? 0 : background.R - alphaMask.A,
                                      background.G - alphaMask.A < 0 ? 0 : background.G - alphaMask.A,
                                      background.B - alphaMask.A < 0 ? 0 : background.B - alphaMask.A);
            }
        }
    }
}