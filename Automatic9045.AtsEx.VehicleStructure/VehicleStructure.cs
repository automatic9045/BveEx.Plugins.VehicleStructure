using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX;
using SlimDX.Direct3D9;

using BveTypes.ClassWrappers;

namespace Automatic9045.AtsEx.VehicleStructure
{
    internal class VehicleStructure
    {
        private readonly Direct3DProvider Direct3DProvider;
        private readonly IReadOnlyList<Car> Cars;
        private readonly bool Vibrate;
        private readonly Matrix FirstCarOriginToFront;

        private readonly List<float> VibrationCoefficients = new List<float>();

        public VehicleStructure(Direct3DProvider direct3DProvider, IReadOnlyList<Car> cars, bool vibrate, Matrix firstCarOriginToFront)
        {
            Direct3DProvider = direct3DProvider;
            Cars = cars;
            Vibrate = vibrate;
            FirstCarOriginToFront = firstCarOriginToFront;

            Random Random = new Random();
            for (int i = 0; i < Cars.Count; i++)
            {
                float coefficient = i == 0 ? 1 : 0.2f + (float)Random.NextDouble();
                VibrationCoefficients.Add(coefficient);
            }
        }

        public void DrawTrains(double vehicleLocation, Matrix vehicleToBlock, Matrix blockToCamera)
        {
            int vehicleBlockLocation = (int)vehicleLocation / 25 * 25;
            Car.ILocator firstCarLocator = null;

            for (int i = 0; i < Cars.Count; i++)
            {
                Car car = Cars[i];

                Car.ILocator carLocator;
                if (Vibrate)
                {
                    if (i == 0)
                    {
                        carLocator = car.GetLocatorWithVibration(vehicleLocation, vehicleBlockLocation, FirstCarOriginToFront, vehicleToBlock);
                        firstCarLocator = carLocator;
                    }
                    else
                    {
                        carLocator = car.GetLocatorWithVibration(vehicleLocation, vehicleBlockLocation, firstCarLocator);
                    }
                }
                else
                {
                    carLocator = car.GetLocator(vehicleLocation, vehicleBlockLocation);
                }

                carLocator = carLocator.Multiply(VibrationCoefficients[i]);
                car.Draw(blockToCamera, carLocator);
            }
        }
    }
}
