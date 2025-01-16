using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

namespace Automatic9045.BveEx.VehicleStructure
{
    internal interface IDoorStateStore
    {
        bool IsOpen(int carIndex, DoorSide side);
    }
}
