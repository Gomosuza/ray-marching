using Microsoft.Xna.Framework.Input;
using RayMarching.Input;
using System.Collections.Generic;

namespace RayMarching.Configuration
{
    public class KeyboardSettings
    {
        /// <summary>
        /// Action -> set of keys that trigger it.
        /// </summary>
        public Dictionary<string, Keys[]> ActionKeyMap { get; set; } = new Dictionary<string, Keys[]>
        {
            [nameof(InputAction.Exit).ToLowerInvariant()] = new[] { Keys.Escape },
            [nameof(InputAction.Forward).ToLowerInvariant()] = new[] { Keys.W, Keys.Up },
            [nameof(InputAction.Left).ToLowerInvariant()] = new[] { Keys.A, Keys.Left },
            [nameof(InputAction.Backward).ToLowerInvariant()] = new[] { Keys.S, Keys.Down },
            [nameof(InputAction.Right).ToLowerInvariant()] = new[] { Keys.D, Keys.Right },
            [nameof(InputAction.Up).ToLowerInvariant()] = new[] { Keys.Space },
            [nameof(InputAction.Down).ToLowerInvariant()] = new[] { Keys.LeftControl, Keys.RightControl },
            [nameof(InputAction.Sprint).ToLowerInvariant()] = new[] { Keys.LeftShift }
        };
    }
}
