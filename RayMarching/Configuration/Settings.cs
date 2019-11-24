namespace RayMarching.Configuration
{
    public class Settings
    {
        public VideoSettings Video { get; set; } = new VideoSettings();

        public KeyboardSettings Keybindings { get; set; } = new KeyboardSettings();

        public MouseSettings Mouse { get; set; } = new MouseSettings();
    }
}
