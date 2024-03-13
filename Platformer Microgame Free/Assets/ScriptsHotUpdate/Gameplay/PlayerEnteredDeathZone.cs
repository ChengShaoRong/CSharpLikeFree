using CSharpLike;

namespace PlatformerMicrogame
{
    /// <summary>
    /// Fired when a player enters a trigger with a DeathZone component.
    /// </summary>
    /// <typeparam name="PlayerEnteredDeathZone"></typeparam>
    public class PlayerEnteredDeathZone
    {
        public DeathZone deathzone;

        public void Execute()
        {
            Simulation.Schedule(typeof(PlayerDeath));
        }
    }
}