using Microsoft.Xna.Framework;

namespace RayMarching.Components
{
    public class WindowTitle : GameComponent
    {
        private int _lastFps;
        private FpsCounter _fpsCounter;
        private int _lastWidth, _lastHeight;

        public WindowTitle(
            Game game,
            FpsCounter fpsCounter
            ) : base(game)
        {
            _fpsCounter = fpsCounter;
            _lastWidth = Game.GraphicsDevice.Viewport.Width;
            _lastHeight = Game.GraphicsDevice.Viewport.Height;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            bool dirty = false;
            if (_lastFps != _fpsCounter.CurrentFps ||
                _lastWidth != Game.GraphicsDevice.Viewport.Width ||
                _lastHeight != Game.GraphicsDevice.Viewport.Height)
            {
                _lastFps = _fpsCounter.CurrentFps;
                _lastWidth = Game.GraphicsDevice.Viewport.Width;
                _lastHeight = Game.GraphicsDevice.Viewport.Height;
                dirty = true;
            }
            if (dirty)
            {
                Game.Window.Title = $"RayMarching - {_lastFps:00} FPS ({_lastWidth}x{_lastHeight})";
            }
        }
    }
}
