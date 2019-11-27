using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RayMarching.Configuration;
using RayMarching.Scene;
using RayMarching.Scene.Camera;
using System.Collections.Generic;

namespace RayMarching
{
    public class RayMarchingGame : Game
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly Settings _settings;

        public RayMarchingGame(Settings settings)
        {
            _settings = settings;
            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = _settings.Video.Width,
                PreferredBackBufferHeight = _settings.Video.Height
            };
        }

        protected override void Initialize()
        {
            base.Initialize();

            var collection = new ServiceCollection();
            collection.AddSingleton<Game>(this);
            collection.AddSingleton(_settings);
            collection.AddSingleton<ITracingOptions, TracingOptions>();
            collection.AddSingleton<RayMarching>();
            collection.AddSingleton(_graphicsDeviceManager);
            collection.AddSingleton(GraphicsDevice);
            collection.AddSingleton<IGraphicsDeviceManager>(_graphicsDeviceManager);
            collection.AddSingleton<IGraphicsDeviceService>(_graphicsDeviceManager);
            collection.AddSingleton<ICamera>(sp => new FpsCamera(GraphicsDevice, sp.GetRequiredService<Settings>(), new Vector3(0, 0, -5), new Vector3(0, 0, 1)));

            collection.Scan(scan =>
            {
                scan
                .FromAssemblyOf<RayMarchingGame>()
                .AddClasses(x => x.AssignableTo<GameComponent>())
                .AsSelfWithInterfaces()
                .WithSingletonLifetime();
            });

            using var serviceProvider = collection.BuildServiceProvider();

            foreach (var c in serviceProvider.GetRequiredService<IEnumerable<IGameComponent>>())
                Components.Add(c);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
