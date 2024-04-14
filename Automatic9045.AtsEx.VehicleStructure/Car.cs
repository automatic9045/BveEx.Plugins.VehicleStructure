using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

using SlimDX;
using SlimDX.Direct3D9;

using BveTypes.ClassWrappers;
using SlimDX.XInput;

namespace Automatic9045.AtsEx.VehicleStructure
{
    internal class Car
    {
        private readonly Direct3DProvider Direct3DProvider;

        private readonly Structure Structure;
        private readonly IEnumerable<Door> Doors;

        private readonly IMatrixCalculator MatrixCalculator;

        public Car(Direct3DProvider direct3DProvider, Structure structure, IEnumerable<Door> doors, IMatrixCalculator matrixCalculator)
        {
            Direct3DProvider = direct3DProvider;

            Structure = structure;
            Doors = doors;

            MatrixCalculator = matrixCalculator;
        }

        public ILocator GetLocatorWithVibration(double vehicleLocation, double vehicleBlockLocation, Matrix firstCarOriginToFront, Matrix vehicleToBlock)
        {
            Matrix firstCarOriginToBlock = GetCarToBlock(vehicleLocation, vehicleBlockLocation);
            Matrix blockToFirstCarOrigin = Matrix.Invert(firstCarOriginToBlock);
            Matrix blockToFirstCarFront = blockToFirstCarOrigin * firstCarOriginToFront;
            Matrix vehicleFrontToFirstCarFront = vehicleToBlock * blockToFirstCarFront;

            return new Locator(vehicleFrontToFirstCarFront, firstCarOriginToBlock);
        }

        public ILocator GetLocatorWithVibration(double vehicleLocation, double vehicleBlockLocation, ILocator vibrationSource)
        {
            Matrix carToBlock = GetCarToBlock(vehicleLocation, vehicleBlockLocation);
            return new Locator((Locator)vibrationSource, carToBlock);
        }

        public ILocator GetLocator(double vehicleLocation, double vehicleBlockLocation)
        {
            Matrix carToBlock = GetCarToBlock(vehicleLocation, vehicleBlockLocation);
            return new Locator(carToBlock);
        }

        private Matrix GetCarToBlock(double vehicleLocation, double vehicleBlockLocation)
        {
            double location = vehicleLocation + Structure.Location;
            Matrix carToBlock = MatrixCalculator.GetTrackMatrix(Structure, location, vehicleBlockLocation);

            return carToBlock;
        }

        public void Draw(Matrix blockToCamera, ILocator locator)
        {
            Matrix transform = locator.Transform * blockToCamera;
            Direct3DProvider.Device.SetTransform(TransformState.World, transform);

            Structure.Model.Draw(Direct3DProvider.Instance, false);
            Structure.Model.Draw(Direct3DProvider.Instance, true);
        }


        internal interface ILocator
        {
            Matrix Transform { get; }

            ILocator Multiply(float coefficient);
        }

        private struct Locator : ILocator
        {
            private readonly Matrix Vibration;
            private readonly Matrix CarToBlock;

            public Matrix Transform => Vibration * CarToBlock;

            public Locator(Matrix vibration, Matrix carToBlock)
            {
                Vibration = vibration;
                CarToBlock = carToBlock;
            }

            public Locator(Matrix carToBlock) : this(Matrix.Identity, carToBlock)
            {
            }

            public Locator(Locator vibrationSource, Matrix carToBlock) : this(vibrationSource.Vibration, carToBlock)
            {
            }

            public ILocator Multiply(float coefficient)
            {
                Matrix transform = Vibration;
                transform.M41 *= coefficient;
                transform.M42 *= coefficient;
                transform.M43 *= coefficient;

                return new Locator(transform, CarToBlock);
            }
        }
    }
}
