using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace RayMarching.Components
{
    public class FpsCounter : DrawableGameComponent
    {
        private readonly int[] _recentFpsCounts;

        private int _fpsCount;
        private TimeSpan _elapsedTime;
        private static readonly TimeSpan _oneSecond = TimeSpan.FromSeconds(1);
        private int _fpsInsertIndex;

        public FpsCounter(
            Game game,
            int lastNSecondsToMonitor = 5
            ) : base(game)
        {
            _recentFpsCounts = new int[lastNSecondsToMonitor];
        }

        public int CurrentFps { get; private set; }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _elapsedTime += gameTime.ElapsedGameTime;
            if (_elapsedTime >= _oneSecond)
            {
                _elapsedTime -= _oneSecond;
                _recentFpsCounts[_fpsInsertIndex] = _fpsCount;
                _fpsCount = 0;
                _fpsInsertIndex = (_fpsInsertIndex + 1) % _recentFpsCounts.Length;
                // initially values are empty, so don't include in avg
                CurrentFps = (int)Math.Round(_recentFpsCounts.TakeWhile(x => x > 0).Average());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _fpsCount++;
            base.Draw(gameTime);
        }
    }
}
