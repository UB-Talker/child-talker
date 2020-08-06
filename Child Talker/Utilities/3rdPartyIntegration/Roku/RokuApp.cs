using System.Xml.Serialization;
using System.Collections.Generic;

namespace RokuCommands
{
    [XmlRoot("active-app")]
    public class RokuActiveApp
    {
        [XmlElement("app")]
        public RokuApp App {get;set;}

        [XmlElement("screensaver")]
        public RokuScreenSaver ScreenSaver {get;set;}
    }

    public class RokuScreenSaver
    {
        [XmlAttribute("id")]
        public string Id {get;set;}

        [XmlAttribute("type")]
        public string Type {get;set;}

        [XmlAttribute("version")]
        public string Version {get;set;}

        [XmlText]
        public string Name {get;set;}
    }

    public class RokuApp
    {
        [XmlAttribute("id")]
        public string Id {get;set;}

        [XmlText]
        public string Name {get;set;}

        [XmlAttribute("type")]
        public string Type {get;set;}

        [XmlAttribute("version")]
        public string Version{get;set;}
    }

    [XmlRoot("apps")]
    public class DeviceAppWrapper
    {
        [XmlElement("app")]
        public List<RokuApp> Apps {get;set;}

    }
}