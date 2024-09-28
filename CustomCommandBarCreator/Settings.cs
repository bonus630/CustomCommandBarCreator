using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CustomCommandBarCreator
{
    [Serializable]
    public class Settings
    {

        [XmlElement]
        public string Author { get;  set; }
        [XmlElement]
        public string Email { get;  set; }
        [XmlElement]
        public bool OpenSite { get;  set; }
        [XmlElement]
        public string URL { get;  set; }
        [XmlElement]
        public string SetupFolder { get;  set; }
        [XmlElement]
        public string LogoPath { get;  set; }
        [XmlElement]
        public string ReadmePath { get;  set; }
        [XmlElement]
        public string IconPath { get;  set; }

       

    }
}
