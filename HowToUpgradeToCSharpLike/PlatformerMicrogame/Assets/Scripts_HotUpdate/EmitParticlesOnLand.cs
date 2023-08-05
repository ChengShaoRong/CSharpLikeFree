using UnityEngine;
using CSharpLike;

namespace Microgame
{
    public class EmitParticlesOnLand : LikeBehaviour
    {
        ParticleSystem p;

        void Start()
        {

            p = gameObject.GetComponent<ParticleSystem>();

            if (GetBoolean("emitOnLand"))
            {
                Simulation.OnExecute(typeof(PlayerLanded), (object obj) =>
                {
                    p.Play();
                });
            }

            if (GetBoolean("emitOnEnemyDeath"))
            {
                Simulation.OnExecute(typeof(EnemyDeath), (object obj) =>
                {
                    p.Play();
                });
            }

        }
    }
}
