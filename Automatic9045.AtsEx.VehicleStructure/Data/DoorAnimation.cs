using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Automatic9045.AtsEx.VehicleStructure.Data
{
    public class DoorAnimation
    {
        [XmlAttribute]
        public string Key = null;

        public Diagram OpenDiagram = new Diagram();
        public Diagram CloseDiagram = new Diagram();
    }
}
