using UnityEngine;
using CSharpLike;

namespace PlatformerMicrogame
{
    /// <summary>
    /// Marks a trigger as a VictoryZone, usually used to end the current game level.
    /// </summary>
    public class VictoryZone : LikeBehaviour
    {
        void OnTriggerEnter2D(Collider2D collider)
        {
            var p = HotUpdateBehaviour.GetComponentByType(collider.gameObject, typeof(PlayerController)) as PlayerController;
            if (p != null)
            {
                (Simulation.Schedule(typeof(PlayerEnteredVictoryZone)) as PlayerEnteredVictoryZone).victoryZone = this;
            }
        }
    }
}