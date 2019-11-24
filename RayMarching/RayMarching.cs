using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.ComputeShader;
using RayMarching.Scene;
using System;
using System.Linq;

namespace RayMarching
{
    public class RayMarching
    {
        private readonly static uint[] _pow = Enumerable.Range(1, 20).Select(x => (uint)Math.Pow(2, x)).ToArray();
        private readonly ComputeShader _shader;
        private readonly GraphicsDevice _graphicsDevice;

        public RayMarching(
            GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _shader = new ComputeShader(graphicsDevice, "Shaders/raymarching.glslcs");
        }

        public RenderTarget2D ChangeSize(int newWidth, int newHeight)
            => new RenderTarget2D(_graphicsDevice, newWidth, newHeight, false, SurfaceFormat.Rgba64, DepthFormat.None, 0, RenderTargetUsage.DiscardContents, true, 1);

        public void Draw(RenderTarget2D renderTarget, ITracingOptions tracingOptions, GameTime gameTime)
        {
            _shader.Begin(renderTarget);

            // compute shader requires batching in power of 2, so must input next power of 2
            // shader will auto. discard anything outside of texture range
            var x = _pow.First(x => x >= renderTarget.Width);
            var y = _pow.First(y => y >= renderTarget.Height);

            var width = renderTarget.Width - 1;
            var height = renderTarget.Height - 1;
            // inject parameters
            _shader.SetParameter("eye", tracingOptions.Camera.Position);
            _shader.SetParameter("direction", tracingOptions.Camera.Direction);
            _shader.SetParameter("time", (float)gameTime.TotalGameTime.TotalSeconds);

            // hardcoded to chunks of 8x8 in compute shader
            _shader.Execute(x / 8, y / 8, 1);
            _shader.End();
        }
    }
}
