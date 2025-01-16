using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Automatic9045.BveEx.VehicleStructure.Data
{
    public class DiagramPoint
    {
        [XmlAttribute]
        public double Progress = 0;

        [XmlAttribute]
        public double OpenRate = 0;

        public override string ToString() => $"{Progress}, {OpenRate}";
    }
}
