using UnityEngine;
using CSharpLike;

namespace PlatformerMicrogame
{
    public class EmitParticlesOnLand : LikeBehaviour
    {
        public bool emitOnLand = true;
        public bool emitOnEnemyDeath = true;

        ParticleSystem p;

        void Start()
        {

            p = gameObject.GetComponent<ParticleSystem>();

            if (emitOnLand)
            {
                Simulation.OnExecute(typeof(PlayerLanded), (object obj) =>
                {
                    p.Play();
                });
            }

            if (emitOnEnemyDeath)
            {
                Simulation.OnExecute(typeof(EnemyDeath), (object obj) =>
                {
                    p.Play();
                });
            }

        }
    }
}
