using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Serialization;

namespace RokuCommands
{
    class RokuHttp
    {
        private static string ip_address = "10.0.0.24";



        public static async Task<IEnumerable<RokuApp>> GetAllChannels()
        {
            var client = new HttpClient();
            var requestUrl = string.Format("http://" + ip_address + ":8060/query/apps");
            var result = await client.GetStreamAsync(requestUrl);

            var serializer = new XmlSerializer(typeof(DeviceAppWrapper));
            var wrapper = (DeviceAppWrapper) serializer.Deserialize(result);

            return wrapper.Apps;

        }

        public static async Task<RokuActiveApp> GetActiveChannels()
        {
            var client = new HttpClient();
            var requestUrl = string.Format("http://" + ip_address + ":8060/query/apps");
            var result = await client.GetStreamAsync(requestUrl);

            var serializer = new XmlSerializer(typeof(RokuActiveApp));
            var activeApp = (RokuActiveApp) serializer.Deserialize(result);

            return activeApp;


        }

        //Launch an app with the app id returned from the app listing
        public static async Task LaunchChannel(string id)
        {
            var client = new HttpClient();
            var requestUrl = string.Format("http://" + ip_address + ":8060/launch/{0}",id);
            var result = await client.PostAsync(requestUrl,null);

        }

        //Emulate the roku remote by sending a key command
        //Reference here: https://sdkdocs.roku.com/display/sdkdoc/External+Control+Guide#ExternalControlGuide-KeypressKeyValues
        public static async Task PressKey(string key)
        {
            var client = new HttpClient();
            var requestUrl = string.Format("http://" + ip_address + ":8060/keypress/{0}",key);
            var result = await client.PostAsync(requestUrl,null);

        }
    }
}
