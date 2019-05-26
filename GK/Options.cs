using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK
{
    public class Options
    {
        public bool DrawWireframe { get; set; } = false;
        public bool ShowHUD { get; set; } = true;
        public bool ShowAxis { get; set; } = true;
        public float MovingSpeedPerSec { get; set; } = 3;
        public float RotatingSpeedPerSec { get; set; } = 0.5f;
        public static Options Instance { get; } = new Options();
        static Options()
        {
        }
        private Options()
        {
        }
    }
}
