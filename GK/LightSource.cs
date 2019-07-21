using GK.Math3D;
using System;

namespace GK
{
    class LightSource : Transformable
    {
        private float _intensity = 1;
        public float Intensity { get => _intensity; set => Math.Min(1, Math.Max(0, value)); }
    }
}
