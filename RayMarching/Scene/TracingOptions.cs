using RayMarching.Scene.Camera;

namespace RayMarching.Scene
{
    public class TracingOptions : ITracingOptions
    {
        public TracingOptions(ICamera camera)
        {
            Camera = camera;
        }

        public ICamera Camera { get; }
    }
}
