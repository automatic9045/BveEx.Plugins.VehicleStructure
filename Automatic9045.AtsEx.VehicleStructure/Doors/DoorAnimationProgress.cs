using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.VehicleStructure.Animation;

namespace Automatic9045.AtsEx.VehicleStructure.Doors
{
    internal class DoorAnimationProgress
    {
        private readonly Diagram Diagram;
        private readonly TimeSpan Duration;
        private readonly double NaturalFrequency;
        private readonly double DampingRatio;
        private readonly double RestitutionFactor;

        public DoorAnimationProgress(Diagram diagram, TimeSpan duration, double naturalFrequency, double dampingRatio, double restitutionFactor)
        {
            Diagram = diagram;
            Duration = duration;
            NaturalFrequency = naturalFrequency;
            DampingRatio = dampingRatio;
            RestitutionFactor = restitutionFactor;
        }

        public ProgressInfo Tick(TimeSpan elapsed, ProgressInfo previous, bool vibrate)
        {
            double deltaProgress = elapsed.TotalSeconds / Duration.TotalSeconds;
            double newProgressValue = Math.Max(0, Math.Min(1, previous.ProgressValue + deltaProgress));

            double targetOpenRate = Diagram.GetValue(newProgressValue);

            if (vibrate)
            {
                double acceleration = Math.Max(-1000, Math.Min(1000, -NaturalFrequency * NaturalFrequency * (previous.OpenRate - targetOpenRate) - 2 * DampingRatio * NaturalFrequency * previous.Velocity));
                double velocity = previous.Velocity + acceleration * elapsed.TotalSeconds;
                double openRate = Math.Max(0, Math.Min(1, previous.OpenRate + velocity * elapsed.TotalSeconds));

                return new ProgressInfo(newProgressValue, openRate < 0.001 || 0.999 < openRate ? -velocity * RestitutionFactor : velocity, openRate);
            }
            else
            {
                double velocity = (targetOpenRate - previous.OpenRate) / elapsed.TotalSeconds;

                return new ProgressInfo(newProgressValue, velocity, targetOpenRate);
            }
        }
    }
}
