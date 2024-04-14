using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

namespace Automatic9045.AtsEx.VehicleStructure
{
    internal static class CarFactory
    {
        public static List<Car> Create(Direct3DProvider direct3DProvider, Data.Structure[] data, string baseDirectory, IMatrixCalculator matrixCalculator)
        {
            List<Car> cars = data
                .Select(x =>
                {
                    string modelPath = Path.Combine(baseDirectory, x.Model);
                    Model model = Model.FromXFile(modelPath);
                    Structure structure = new Structure(
                        x.Distance, string.Empty,
                        0, 0, x.Z, 0, 0, 0,
                        TiltOptions.TiltsAlongGradient | TiltOptions.TiltsAlongCant, x.Span, model);

                    Car car = new Car(direct3DProvider, structure, Enumerable.Empty<Door>(), matrixCalculator);
                    return car;
                })
                .ToList();

            return cars;
        }
    }
}
