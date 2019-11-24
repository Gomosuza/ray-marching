using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RayMarching.Scene;

namespace RayMarching
{
    public class SceneRenderer : DrawableGameComponent
    {
        private readonly ITracingOptions _tracingOptions;
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _pixel;

        private RenderTarget2D? _renderTarget;
        private SamplerState _samplerState = SamplerState.PointClamp;
        private readonly RayMarching _rayMarching;

        public SceneRenderer(
            Game game,
            RayMarching rayMarching,
            ITracingOptions tracingOptions
            ) : base(game)
        {
            _tracingOptions = tracingOptions;
            _rayMarching = rayMarching;

            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
            _pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ResizeRenderTarget(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }

        private void ResizeRenderTarget(int w, int h)
        {
            if (_renderTarget == null ||
                _renderTarget.Width != w ||
                _renderTarget.Height != h)
            {
                _renderTarget = _rayMarching.ChangeSize(w, h);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (_renderTarget == null)
                return;

            // fill rendertarget so we can see if tracing did not fill entirely
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(Color.Purple);
            GraphicsDevice.SetRenderTarget(null);

            _rayMarching.Draw(_renderTarget, _tracingOptions, gameTime);

            GraphicsDevice.SetRenderTarget(null);
            // simply upscale rendertarget to screen to draw scene
            _spriteBatch.Begin(SpriteSortMode.Immediate, null, _samplerState);
            _spriteBatch.Draw(_renderTarget, GraphicsDevice.Viewport.Bounds, Color.White);
            _spriteBatch.End();
        }
    }
}
