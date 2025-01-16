using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Automatic9045.BveEx.VehicleStructure.Data
{
    public class Diagram
    {
        [XmlAttribute]
        public string Duration = "0:00:03";

        [XmlAttribute]
        public double NaturalFrequency = double.PositiveInfinity;

        [XmlAttribute]
        public double DampingRatio = 1;

        [XmlAttribute]
        public double RestitutionFactor = 0;

        public DiagramPoint[] DiagramPoints = new DiagramPoint[0];

        internal void Validate()
        {
            CheckValue(nameof(NaturalFrequency), NaturalFrequency, 0);
            CheckValue(nameof(DampingRatio), DampingRatio, 0, 1);
            CheckValue(nameof(RestitutionFactor), RestitutionFactor, 0, 1);


            void CheckValue(string name, double value, double min = double.NegativeInfinity, double max = double.PositiveInfinity)
            {
                if (value < min || max < value)
                {
                    string minText = double.MinValue < min ? $"{min} 以上" : null;
                    string maxText = max < double.MaxValue ? $"{max} 以下" : null;

                    IEnumerable<string> texts = new[] { minText, maxText }.Where(x => !(x is null));

                    throw new InvalidOperationException($"{name} の値 {value} は不正です。{string.Join(" ", texts)}の値を指定してください。");
                }
            }
        }
    }
}
