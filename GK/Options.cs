namespace GK
{
    public class Options
    {
        public bool ShowWireframe { get; set; } = false;
        public bool ShowDebugHUD { get; set; } = true;
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
