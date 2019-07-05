using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Child_Talker
{
    [Serializable]
    [XmlSerializerAssembly("Child_Talker.XmlSerializers")]
    public class ChildTalkerXml
    {
        public enum Tile { folder, talker }

        [XmlAttribute]
        public Tile TileType { get; set; }

        [XmlAttribute]
        public string Text { get; set; }

        [XmlAttribute]
        public string ImagePath { get; set; }

        [XmlArrayItem]
        public List<ChildTalkerXml> Children { get; set; }

        [XmlAttribute]
        public bool InColor { get; set; }
    }
}
