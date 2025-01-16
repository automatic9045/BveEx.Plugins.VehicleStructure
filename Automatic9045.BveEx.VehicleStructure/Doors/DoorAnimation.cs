using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.BveEx.VehicleStructure.Animation;

namespace Automatic9045.BveEx.VehicleStructure.Doors
{
    internal class DoorAnimation : ICloneable
    {
        public static readonly DoorAnimation Default;

        static DoorAnimation()
        {
            DiagramPoint[] points = new DiagramPoint[] { new DiagramPoint(0, 0), new DiagramPoint(1, 1) };
            Diagram diagram = new Diagram(points);

            DoorAnimationProgress openProgress = new DoorAnimationProgress(diagram, TimeSpan.FromSeconds(3), double.PositiveInfinity, 1, 0);
            DoorAnimationProgress closeProgress = new DoorAnimationProgress(diagram, TimeSpan.FromSeconds(-3), double.PositiveInfinity, 1, 0);

            Default = new DoorAnimation(openProgress, closeProgress);
        }


        private readonly DoorAnimationProgress OpenProgress;
        private readonly DoorAnimationProgress CloseProgress;

        private bool IsOpen = false;
        private ProgressInfo ProgressInfo = default;
        public double ProgressValue => ProgressInfo.ProgressValue;
        public double OpenRate => ProgressInfo.OpenRate;

        public DoorAnimation(DoorAnimationProgress openProgress, DoorAnimationProgress closeProgress)
        {
            OpenProgress = openProgress;
            CloseProgress = closeProgress;
        }

        public object Clone() => new DoorAnimation(OpenProgress, CloseProgress);

        public void Tick(TimeSpan elapsed, bool isOpen, bool vibrate)
        {
            DoorAnimationProgress progress = isOpen ? OpenProgress : CloseProgress;

            if (isOpen != IsOpen)
            {
                ProgressInfo newProgressInfo = progress.Tick(elapsed, ProgressInfo, vibrate);
                if (Math.Abs(newProgressInfo.OpenRate - ProgressInfo.OpenRate) < 0.01)
                {
                    ProgressInfo = newProgressInfo;
                }
                else
                {
                    _ = progress.TryGetProgressValue(ProgressInfo.OpenRate, out double progressValue);
                    ProgressInfo fixedProgressInfo = new ProgressInfo(progressValue, ProgressInfo.Velocity, ProgressInfo.OpenRate);

                    ProgressInfo = progress.Tick(elapsed, fixedProgressInfo, vibrate);
                }

                IsOpen = isOpen;
            }
            else
            {
                ProgressInfo = progress.Tick(elapsed, ProgressInfo, vibrate);
            }
        }
    }
}
