using System.Collections;
using UnityEngine;
using CSharpLike;

namespace PlatformerMicrogame
{
    public class PlatformerSpeedPad : LikeBehaviour
    {
        public float maxSpeed;

        [Range(0, 5)]
        public float duration = 1f;

        void OnTriggerEnter2D(Collider2D other)
        {
            var rb = other.attachedRigidbody;
            if (rb == null) return;
            var player = HotUpdateBehaviour.GetComponentByType(rb.gameObject, typeof(PlayerController)) as PlayerController;// rb.GetComponent<PlayerController>();
            if (player == null) return;
            //player.StartCoroutine("PlayerModifier", player, duration);
            behaviour.MemberCallDelay("PlayerModifierStep1", player, duration, 0f);
        }

        //IEnumerator PlayerModifier(PlayerController player, float lifetime)
        //{
        //    var initialSpeed = player.maxSpeed;
        //    player.maxSpeed = maxSpeed;
        //    yield return new WaitForSeconds(lifetime);
        //    player.maxSpeed = initialSpeed;
        //}

        float _initialSpeed = 0f;
        void PlayerModifierStep1(PlayerController player, float lifetime)
        {
            _initialSpeed = player.maxSpeed;
            player.maxSpeed = maxSpeed;
            behaviour.MemberCallDelay("PlayerModifierStep2", player, lifetime);
        }
        void PlayerModifierStep2(PlayerController player)
        {
            player.maxSpeed = _initialSpeed;
        }
    }
}
