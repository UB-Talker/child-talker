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



        public static async Task<IEnumerable<RokuApp>> GetAllChannels()
        {
            string ipAddress = Child_Talker.Properties.HardareIntegration.Default.Roku_IP;
            var client = new HttpClient();
            var requestUrl = string.Format("http://" + ipAddress + ":8060/query/apps");
            var result = await client.GetStreamAsync(requestUrl);

            var serializer = new XmlSerializer(typeof(DeviceAppWrapper));
            var wrapper = (DeviceAppWrapper) serializer.Deserialize(result);

            return wrapper.Apps;

        }

        public static async Task<RokuActiveApp> GetActiveChannels()
        {
            string ipAddress = Child_Talker.Properties.HardareIntegration.Default.Roku_IP;
            var client = new HttpClient();
            var requestUrl = string.Format("http://" + ipAddress + ":8060/query/apps");
            var result = await client.GetStreamAsync(requestUrl);

            var serializer = new XmlSerializer(typeof(RokuActiveApp));
            var activeApp = (RokuActiveApp) serializer.Deserialize(result);

            return activeApp;


        }

        //Launch an app with the app id returned from the app listing
        public static async Task LaunchChannel(string id)
        {
            string ipAddress = Child_Talker.Properties.HardareIntegration.Default.Roku_IP;
            var client = new HttpClient();
            var requestUrl = string.Format("http://" + ipAddress + ":8060/launch/{0}",id);
            var result = await client.PostAsync(requestUrl,null);

        }

        //Emulate the roku remote by sending a key command
        //Reference here: https://sdkdocs.roku.com/display/sdkdoc/External+Control+Guide#ExternalControlGuide-KeypressKeyValues
        public static async Task PressKey(string key)
        {
            string ipAddress = Child_Talker.Properties.HardareIntegration.Default.Roku_IP;
            var client = new HttpClient();
            var requestUrl = string.Format("http://" + ipAddress + ":8060/keypress/{0}",key);
            var result = await client.PostAsync(requestUrl,null);

        }
    }
}
