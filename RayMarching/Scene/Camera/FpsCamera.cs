using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RayMarching.Configuration;
using System;

namespace RayMarching.Scene.Camera
{
    public class FpsCamera : ICamera
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Vector3 _initialDirection;
        private Vector3 _direction;
        private float _horizontalRotation, _verticalRotation;
        private readonly Settings _settings;

        public FpsCamera(
            GraphicsDevice graphicsDevice,
            Settings settings,
            Vector3 pos,
            Vector3 initialDirection)
        {
            _graphicsDevice = graphicsDevice;
            _settings = settings;
            Position = pos;
            _direction = _initialDirection = Vector3.Normalize(initialDirection);
        }

        public Vector3 Position { get; private set; }

        public Vector3 Direction => _direction;

        public bool IsDirty { get; private set; }

        private Vector3 Up
            => Vector3.Normalize(Vector3.Cross(_direction, Right));

        private Vector3 Right
            => Vector3.Normalize(Vector3.Cross(Vector3.Up, _direction)) * _graphicsDevice.Viewport.AspectRatio;

        public Ray GetRayForRasterPosition(int x, int y, int width, int height)
        {
            const int fov = 120;
            // x is in range 0 - width, we need it to be -1 to 1
            // y is in range 0 - height, we need it to be -1 to 1
            var scalarOffsetX = -1f + x / (float)width * 2f;
            // needs to be the inverse of x since x is left to right both in world and raster space
            // but -1 y is down in world space but up in raster space
            var scalarOffsetY = 1f - y / (float)height * 2f;

            float fovAdjust = 1f / (float)Math.Tan(fov / 2f * Math.PI / 180f);
            var dir = _direction + scalarOffsetX * Right * fovAdjust + scalarOffsetY * Up * fovAdjust;
            dir.Normalize();
            return new Ray(Position, dir);
        }

        public void Move(Vector3 direction)
        {
            if (direction == Vector3.Zero)
                return;

            Position += Right * direction.X;
            Position += Up * direction.Y;
            Position += _direction * direction.Z;
            IsDirty = true;
        }

        public void Rotate(float x, float y)
        {
            if (x == 0 && y == 0)
                return;

            _horizontalRotation += x;
            _verticalRotation = MathHelper.Clamp(_verticalRotation + y, -MathHelper.PiOver2 + 0.0001f, MathHelper.PiOver2 - 0.0001f);

            _direction = Vector3.Transform(_initialDirection, Matrix.CreateRotationX(_verticalRotation) * Matrix.CreateRotationY(_horizontalRotation));
            IsDirty = true;
        }
    }
}
