using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.VehicleStructure.Animation
{
    internal class Diagram
    {
        private readonly List<DiagramPoint> Points;

        public Diagram(IEnumerable<DiagramPoint> points)
        {
            Points = points.ToList();
            Points.Sort();
        }

        public double GetValue(double x)
        {
            DiagramPoint oldPoint = default;
            foreach (DiagramPoint point in Points)
            {
                if (x == point.X)
                {
                    return point.Y;
                }

                if (x < point.X)
                {
                    return oldPoint.Y + (point.Y - oldPoint.Y) * (x - oldPoint.X) / (point.X - oldPoint.X);
                }

                oldPoint = point;
            }

            return oldPoint.Y;
        }
    }
}
