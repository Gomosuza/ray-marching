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
            [nameof(InputAction.Exit)] = new[] { Keys.Escape },
            [nameof(InputAction.Forward)] = new[] { Keys.W, Keys.Up },
            [nameof(InputAction.Left)] = new[] { Keys.A, Keys.Left },
            [nameof(InputAction.Backward)] = new[] { Keys.S, Keys.Down },
            [nameof(InputAction.Right)] = new[] { Keys.D, Keys.Right },
            [nameof(InputAction.Up)] = new[] { Keys.Space },
            [nameof(InputAction.Down)] = new[] { Keys.LeftControl, Keys.RightControl },
            [nameof(InputAction.Sprint)] = new[] { Keys.LeftShift }
        };
    }
}
