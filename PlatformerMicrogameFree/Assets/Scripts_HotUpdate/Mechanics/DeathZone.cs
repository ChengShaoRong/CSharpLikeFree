using CSharpLike;
using UnityEngine;

namespace Microgame
{
    /// <summary>
    /// DeathZone components mark a collider which will schedule a
    /// PlayerEnteredDeathZone event when the player enters the trigger.
    /// </summary>
    public class DeathZone : LikeBehaviour
    {
        void OnTriggerEnter2D(Collider2D collider)
        {
            var p = HotUpdateBehaviour.GetComponentByType(collider.gameObject, typeof(PlayerController)) as PlayerController;
            if (p != null)
            {
                var ev = Simulation.Schedule(typeof(PlayerEnteredDeathZone)) as PlayerEnteredDeathZone;
                ev.deathzone = this;
            }
        }
    }
}