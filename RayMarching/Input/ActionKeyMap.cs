using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayMarching.Configuration;
using System.Linq;

namespace RayMarching.Input
{
    public class ActionKeyMap : GameComponent, IActionKeyMap
    {
        private KeyboardState _currentState;
        private readonly Settings _settings;

        public ActionKeyMap(
            Game game,
            Settings settings
            ) : base(game)
        {
            _settings = settings;
        }

        public override void Initialize()
        {
            base.Initialize();

            _currentState = Keyboard.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _currentState = Keyboard.GetState();
        }

        public bool IsPressed(string action)
            => _settings.Keybindings.ActionKeyMap[action].Any(_currentState.IsKeyDown);
    }
}
