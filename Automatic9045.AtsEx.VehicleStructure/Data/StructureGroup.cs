﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Automatic9045.AtsEx.VehicleStructure.Data
{
    [XmlRoot]
    public class StructureGroup
    {
        public Structure[] Structures = new Structure[0];
    }
}
