using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Automatic9045.BveEx.VehicleStructure.Data
{
    [XmlRoot]
    public class Door
    {
        [XmlAttribute]
        public string Model = null;

        [XmlAttribute]
        public double X = 0;

        [XmlAttribute]
        public double Y = 0;

        [XmlAttribute]
        public double Z = 0;

        [XmlAttribute]
        public DoorSide Side = DoorSide.Left;

        [XmlAttribute]
        public double OpenWidth = 0.65;

        [XmlAttribute]
        public string AnimationKey = string.Empty;
    }
}
