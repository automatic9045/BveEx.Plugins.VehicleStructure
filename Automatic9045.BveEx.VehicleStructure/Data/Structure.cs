﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Automatic9045.BveEx.VehicleStructure.Data
{
    [XmlRoot]
    public class Structure
    {
        [XmlAttribute]
        public string Model = null;

        [XmlAttribute]
        public double Distance = 0;

        [XmlAttribute]
        public double Span = 0;

        [XmlAttribute]
        public double Z = 0;

        public Door[] Doors = new Door[0];
    }
}
