using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.BveEx.VehicleStructure.Animation
{
    internal struct DiagramPoint : IComparable<DiagramPoint>
    {
        public double X { get; }
        public double Y { get; }

        public DiagramPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public int CompareTo(DiagramPoint other)
        {
            double diff = X - other.X;
            return Math.Sign(diff);
        }

        public override string ToString() => $"{X}, {Y}";
    }
}
