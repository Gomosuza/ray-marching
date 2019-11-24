using RayMarching.Scene.Camera;

namespace RayMarching.Scene
{
    public interface ITracingOptions
    {
        ICamera Camera { get; }
    }
}
