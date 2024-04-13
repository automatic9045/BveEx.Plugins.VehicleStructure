using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BveTypes.ClassWrappers;

namespace Automatic9045.AtsEx.VehicleStructure
{
    internal static class StructureFactory
    {
        public static List<Structure> Create(Data.Structure[] data, string baseDirectory)
        {
            List<Structure> structures = data
                .Select(x =>
                {
                    string modelPath = Path.Combine(baseDirectory, x.Model);
                    Model model = Model.FromXFile(modelPath);
                    Structure result = new Structure(
                        x.Distance, string.Empty,
                        0, 0, x.Z, 0, 0, 0,
                        TiltOptions.TiltsAlongGradient | TiltOptions.TiltsAlongCant, x.Span, model);
                    return result;
                })
                .ToList();

            return structures;
        }
    }
}
