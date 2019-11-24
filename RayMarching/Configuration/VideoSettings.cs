using System.ComponentModel.DataAnnotations;

namespace RayMarching.Configuration
{
    public class VideoSettings
    {
        [Range(100, int.MaxValue)]
        public int Width { get; set; } = 800;

        [Range(100, int.MaxValue)]
        public int Height { get; set; } = 480;
    }
}
