using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlimDX;
using SlimDX.Direct3D9;

using BveTypes.ClassWrappers;

namespace Automatic9045.BveEx.VehicleStructure.Doors
{
    internal class Door
    {
        private readonly Direct3DProvider Direct3DProvider;

        private readonly Model Model;
        private readonly Matrix Location;

        private readonly DoorAnimation Animation;
        private readonly double OpenWidth;

        public int CarIndex { get; }
        public DoorSide Side { get; }

        public Door(Direct3DProvider direct3DProvider, Model model, double x, double y, double z, int carIndex, DoorSide side, DoorAnimation animation, double openWidth)
        {
            Direct3DProvider = direct3DProvider;

            Model = model;
            Location = Matrix.Translation((float)x, (float)y, (float)z);

            CarIndex = carIndex;
            Side = side;

            Animation = animation;
            OpenWidth = openWidth;
        }

        public void Tick(TimeSpan elapsed, bool isOpen, bool vibrate)
        {
            Animation.Tick(elapsed, isOpen, vibrate);
        }

        public void Draw()
        {
            Matrix world = Direct3DProvider.Device.GetTransform(TransformState.World);
            Matrix animation = Matrix.Translation(0, 0, (float)(OpenWidth * Animation.OpenRate));
            Direct3DProvider.Device.SetTransform(TransformState.World, animation * Location * world);

            Model.Draw(Direct3DProvider, false);
            Model.Draw(Direct3DProvider, true);

            Direct3DProvider.Device.SetTransform(TransformState.World, world);
        }
    }
}
