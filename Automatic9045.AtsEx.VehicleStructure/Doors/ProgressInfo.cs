using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.VehicleStructure.Doors
{
    internal struct ProgressInfo
    {
        public double ProgressValue { get; }

        public double Velocity { get; }
        public double OpenRate { get; }

        public ProgressInfo(double progressValue, double velocity, double openRate)
        {
            ProgressValue = progressValue;

            Velocity = velocity;
            OpenRate = openRate;
        }
    }
}
