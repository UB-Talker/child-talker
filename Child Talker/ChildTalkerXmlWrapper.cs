using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Child_Talker
{
    [Serializable]
    [XmlSerializerAssembly("Child_Talker.XmlSerializers")]
    public class ChildTalkerXmlWrapper
    {
        [XmlArrayItem]
        public List<ChildTalkerXml> Children { get; set; }
    }
}
