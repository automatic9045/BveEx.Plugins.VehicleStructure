using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.VehicleStructure.Animation;

namespace Automatic9045.AtsEx.VehicleStructure.Doors
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
            ProgressInfo = isOpen ? OpenProgress.Tick(elapsed, ProgressInfo, vibrate) : CloseProgress.Tick(elapsed, ProgressInfo, vibrate);
        }
    }
}
