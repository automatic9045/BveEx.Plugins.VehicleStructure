using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;
using ObjectiveHarmonyPatch;
using SlimDX;

using AtsEx.PluginHost;
using AtsEx.PluginHost.Plugins;

using Automatic9045.AtsEx.VehicleStructure.Animation;
using Automatic9045.AtsEx.VehicleStructure.Doors;

namespace Automatic9045.AtsEx.VehicleStructure
{
    [PluginType(PluginType.VehiclePlugin)]
    public class PluginMain : AssemblyPluginBase
    {
        private static readonly string AssemblyLocation = Assembly.GetExecutingAssembly().Location;
        private static readonly string BaseDirectory = Path.GetDirectoryName(AssemblyLocation);

        private readonly Data.Config Config;
        private readonly HarmonyPatch DrawObjectsPatch;

        private Dictionary<string, DoorAnimation> DoorAnimations;
        private List<VehicleStructure> VehicleStructures;

        public PluginMain(PluginBuilder builder) : base(builder)
        {
            BveHacker.ScenarioCreated += OnScenarioCreated;

            string path = Path.Combine(BaseDirectory, "VehicleStructure.Config.xml");
            Config = Data.Config.Deserialize(path, true);
            foreach (Data.DoorAnimation doorAnimation in Config.DoorAnimations)
            {
                doorAnimation.OpenDiagram.Validate();
                doorAnimation.CloseDiagram.Validate();
            }

            FastMember.FastMethod drawObjectsMethod = BveHacker.BveTypes.GetClassInfoOf<ObjectDrawer>().GetSourceMethodOf(nameof(ObjectDrawer.Draw));
            DrawObjectsPatch = HarmonyPatch.Patch(nameof(BveHacker), drawObjectsMethod.Source, PatchType.Postfix);
            DrawObjectsPatch.Invoked += (sender, e) =>
            {
                if (!BveHacker.IsScenarioCreated) return PatchInvokationResult.DoNothing(e);

                UserVehicleLocationManager locationManager = BveHacker.Scenario.LocationManager;
                Vehicle vehicle = BveHacker.Scenario.Vehicle;
                MyTrack myTrack = BveHacker.Scenario.Route.MyTrack;

                double vehicleLocation = locationManager.Location;

                Matrix blockToCamera = vehicle.CameraLocation.TransformFromBlock;
                Matrix blockToVehicle =
                    vehicle.VibrationManager.Positioner.BlockToCarCenterTransform.Matrix
                    * vehicle.VibrationManager.CarBodyTransform.Matrix
                    * vehicle.VibrationManager.ViewPoint.GetTranslation();

                foreach (VehicleStructure vehicleStructure in VehicleStructures)
                {
                    vehicleStructure.DrawTrains(vehicleLocation, Matrix.Invert(blockToVehicle), blockToCamera);
                }

                return PatchInvokationResult.DoNothing(e);
            };
        }

        public override void Dispose()
        {
            BveHacker.ScenarioCreated -= OnScenarioCreated;

            DrawObjectsPatch.Dispose();
        }

        private void OnScenarioCreated(ScenarioCreatedEventArgs e)
        {
            DoorAnimations = Config.DoorAnimations
                .ToDictionary(item => item.Key, item =>
                {
                    DoorAnimationProgress openProgress = CreateProgress(item.OpenDiagram, false);
                    DoorAnimationProgress closeProgress = CreateProgress(item.CloseDiagram, true);

                    DoorAnimation doorAnimation = new DoorAnimation(openProgress, closeProgress);
                    return doorAnimation;


                    DoorAnimationProgress CreateProgress(Data.Diagram data, bool flipDurationSign)
                    {
                        IEnumerable<DiagramPoint> points = data.DiagramPoints.Select(pointData => new DiagramPoint(pointData.Progress, pointData.OpenRate));
                        Diagram diagram = new Diagram(points);

                        TimeSpan duration = TimeSpan.Parse(data.Duration);
                        if (flipDurationSign) duration = duration.Negate();

                        return new DoorAnimationProgress(diagram, duration, data.NaturalFrequency, data.DampingRatio, data.RestitutionFactor);
                    }
                });

            MatrixCalculator matrixCalculator = new MatrixCalculator(e.Scenario.Route);
            DoorStateStore doorStateStore = new DoorStateStore(e.Scenario.Vehicle.Doors);
            CarFactory carFactory = new CarFactory(Direct3DProvider.Instance, matrixCalculator, doorStateStore, DoorAnimations);

            VehicleStructures = Config.VehicleTrain.StructureGroups
                .Select(group =>
                {
                    List<Car> cars = carFactory.Create(group.Structures, BaseDirectory);
                    Matrix firstCarOriginToFront = Matrix.Translation(0, 0, (float)group.FirstStructureFront);

                    VehicleStructure result = new VehicleStructure(Direct3DProvider.Instance, cars, group.Vibrate, firstCarOriginToFront);
                    return result;
                })
                .ToList();
        }

        public override TickResult Tick(TimeSpan elapsed)
        {
            foreach (VehicleStructure vehicleStructure in VehicleStructures)
            {
                vehicleStructure.Tick(elapsed, BveHacker.Scenario.TimeManager.State != TimeManager.GameState.FastForward);
            }

            return new VehiclePluginTickResult();
        }


        private class MatrixCalculator : IMatrixCalculator
        {
            private readonly Route Route;

            public MatrixCalculator(Route route)
            {
                Route = route;
            }

            public Matrix GetTrackMatrix(LocatableMapObject mapObject, double to, double from)
                => Route.GetTrackMatrix(mapObject, to, from);
        }

        private class DoorStateStore : IDoorStateStore
        {
            private readonly DoorSet Doors;

            public DoorStateStore(DoorSet doors)
            {
                Doors = doors;
            }

            public bool IsOpen(int carIndex, DoorSide side)
            {
                SideDoorSet sideDoors = Doors.GetSide(side);
                CarDoor carDoor = sideDoors.CarDoors[carIndex];
                return carDoor.State == DoorState.Open;
            }
        }
    }
}
