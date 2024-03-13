using UnityEngine;
using CSharpLike;

namespace PlatformerMicrogame
{
    public class PlatformerJumpPad : LikeBehaviour
    {
        public float verticalVelocity;

        void OnTriggerEnter2D(Collider2D other)
        {
            var rb = other.attachedRigidbody;
            if (rb == null) return;
            var player = HotUpdateBehaviour.GetComponentByType(rb.gameObject, typeof(PlayerController)) as PlayerController;// rb.GetComponent<PlayerController>();
            if (player == null) return;
            AddVelocity(player);
        }

        void AddVelocity(PlayerController player)
        {
            player.velocity.y = verticalVelocity;
        }
    }
}