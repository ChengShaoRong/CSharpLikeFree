
using CSharpLike;

namespace Microgame
{
    /// <summary>
    /// Fired when the player health reaches 0. This usually would result in a 
    /// PlayerDeath event.
    /// </summary>
    /// <typeparam name="HealthIsZero"></typeparam>
    public class HealthIsZero
    {
        public Health health;

        public void Execute()
        {
            Simulation.Schedule(typeof(PlayerDeath));
        }
    }
}