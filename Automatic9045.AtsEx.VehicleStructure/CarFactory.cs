using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

using Automatic9045.AtsEx.VehicleStructure.Doors;

namespace Automatic9045.AtsEx.VehicleStructure
{
    internal class CarFactory
    {
        private readonly Direct3DProvider Direct3DProvider;
        private readonly IMatrixCalculator MatrixCalculator;
        private readonly IDoorStateStore DoorStateStore;

        private readonly IReadOnlyDictionary<string, DoorAnimation> DoorAnimations;

        public CarFactory(Direct3DProvider direct3DProvider, IMatrixCalculator matrixCalculator, IDoorStateStore doorStateStore, IReadOnlyDictionary<string, DoorAnimation> doorAnimations)
        {
            Direct3DProvider = direct3DProvider;
            MatrixCalculator = matrixCalculator;
            DoorStateStore = doorStateStore;

            DoorAnimations = doorAnimations;
        }

        public List<Car> Create(Data.Structure[] data, string baseDirectory)
        {
            List<Car> cars = data
                .Select((carData, i) =>
                {
                    Model carModel = LoadModel(carData.Model);
                    Structure carStructure = new Structure(carData.Distance, string.Empty, 0, 0, carData.Z, 0, 0, 0, TiltOptions.TiltsAlongGradient | TiltOptions.TiltsAlongCant, carData.Span, carModel);

                    List<Door> doors = carData.Doors
                        .Select(doorData =>
                        {
                            Model doorModel = LoadModel(doorData.Model);

                            DoorSide doorSide;
                            switch (doorData.Side)
                            {
                                case Data.DoorSide.Left:
                                    doorSide = DoorSide.Left;
                                    break;
                                case Data.DoorSide.Right:
                                    doorSide = DoorSide.Right;
                                    break;
                                default:
                                    throw new NotSupportedException();
                            }

                            if (!DoorAnimations.TryGetValue(doorData.AnimationKey, out DoorAnimation doorAnimation))
                            {
                                doorAnimation = DoorAnimation.Default;
                            }

                            return new Door(Direct3DProvider, doorModel, doorData.X, doorData.Y, doorData.Z, i, doorSide, (DoorAnimation)doorAnimation.Clone(), doorData.OpenWidth);
                        })
                        .ToList();

                    Car car = new Car(Direct3DProvider, carStructure, doors, MatrixCalculator, DoorStateStore);
                    return car;
                })
                .ToList();

            return cars;


            Model LoadModel(string modelPath)
            {
                string modelAbsolutePath = Path.Combine(baseDirectory, modelPath);
                Model model = Model.FromXFile(modelAbsolutePath);
                return model;
            }
        }
    }
}
