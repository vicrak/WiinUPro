using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Shared
{
    public class UpdateChecker
    {
        private const string InfoUrl = "https://raw.githubusercontent.com/KeyPuncher/WiinUPro/master/app_info.json";

        /// <summary>
        /// Checks for updates.
        /// Callback signature: (bool success, bool updateAvailable, AppVersionInfo info)
        /// success: true if check completed successfully, false if error.
        /// updateAvailable: true if new version found.
        /// info: details of the new version (null if no update or error).
        /// </summary>
        public static void CheckForUpdates(string appName, string currentVersion, Action<bool, bool, AppVersionInfo> callback)
        {
            try
            {
                // Ensure TLS 1.2 is supported for GitHub
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

                WebClient client = new WebClient();
                client.DownloadStringCompleted += (s, e) =>
                {
                    try
                    {
                        if (e.Error != null)
                        {
                            callback(false, false, null);
                            return;
                        }

                        string json = e.Result;
                        AppInfo info = Deserialize<AppInfo>(json);

                        if (info != null && info.Apps != null)
                        {
                            foreach (var app in info.Apps)
                            {
                                if (app.Name.Equals(appName, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (IsNewerVersion(currentVersion, app.Version))
                                    {
                                        callback(true, true, app);
                                    }
                                    else
                                    {
                                        callback(true, false, app);
                                    }
                                    return;
                                }
                            }
                        }

                        // App not found in list, treat as successful check but no update
                        callback(true, false, null);
                    }
                    catch
                    {
                        callback(false, false, null);
                    }
                    finally
                    {
                        client.Dispose();
                    }
                };

                client.DownloadStringAsync(new Uri(InfoUrl));
            }
            catch
            {
                callback(false, false, null);
            }
        }

        private static bool IsNewerVersion(string current, string remote)
        {
            Version vCurrent, vRemote;
            if (Version.TryParse(current, out vCurrent) && Version.TryParse(remote, out vRemote))
            {
                return vRemote > vCurrent;
            }
            return false;
        }

        private static T Deserialize<T>(string json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }
    }
}
