using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayMarching.Configuration;
using RayMarching.Input;
using RayMarching.Scene.Camera;

namespace RayMarching.Components
{
    public class Player : GameComponent
    {
        private MouseState _previousMouseState, _currentMouseState;
        private readonly ICamera _camera;
        private readonly float _movementSpeed;
        private readonly IActionKeyMap _actionKeyMap;
        private readonly Settings _settings;

        public Player(
            Game game,
            ICamera camera,
            IActionKeyMap actionKeyMap,
            Settings settings,
            float movementSpeed = 1f
            ) : base(game)
        {
            _camera = camera;
            _movementSpeed = movementSpeed;
            _actionKeyMap = actionKeyMap;
            _settings = settings;
        }

        public override void Initialize()
        {
            base.Initialize();
            CenterMouse();
            _previousMouseState = _currentMouseState = Mouse.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Game.IsActive)
                return;

            _currentMouseState = Mouse.GetState();

            Rotate(gameTime);
            Move(gameTime);
        }

        private void Rotate(GameTime gameTime)
        {
            var delta = _currentMouseState.Position - _previousMouseState.Position;
            var x = delta.X * (_settings.Mouse.InvertXAxis ? -1 : 1);
            var y = delta.Y * (_settings.Mouse.InvertYAxis ? -1 : 1);
            if (x == 0 && y == 0)
                return;

            _camera.Rotate(x * _settings.Mouse.SensitivityX * (float)gameTime.ElapsedGameTime.TotalSeconds, y * _settings.Mouse.SensitivityY * (float)gameTime.ElapsedGameTime.TotalSeconds);

            CenterMouse();
            _previousMouseState = Mouse.GetState();
        }

        private void CenterMouse()
            => Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);

        private void Move(GameTime gameTime)
        {
            var movement = Vector3.Zero;
            if (IsPressed(nameof(InputAction.Forward)))
                movement += Vector3.UnitZ;
            if (IsPressed(nameof(InputAction.Backward)))
                movement -= Vector3.UnitZ;

            if (IsPressed(nameof(InputAction.Left)))
                movement -= Vector3.UnitX;
            if (IsPressed(nameof(InputAction.Right)))
                movement += Vector3.UnitX;

            if (IsPressed(nameof(InputAction.Up)))
                movement += Vector3.UnitY;
            if (IsPressed(nameof(InputAction.Down)))
                movement -= Vector3.UnitY;

            var movementSpeed = _movementSpeed * (IsPressed(nameof(InputAction.Sprint)) ? 3 : 1);
            if (movement != Vector3.Zero)
                _camera.Move(Vector3.Normalize(movement) * movementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        private bool IsPressed(string action)
            => _actionKeyMap.IsPressed(action);
    }
}
