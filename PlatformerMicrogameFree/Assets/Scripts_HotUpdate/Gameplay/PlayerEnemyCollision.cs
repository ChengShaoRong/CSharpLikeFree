using CSharpLike;

namespace Microgame
{
    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerEnemyCollision
    {
        public EnemyController enemy;
        public PlayerController player;

        public void Execute()
        {
            var willHurtEnemy = player.GetBounds().center.y >= enemy.GetBounds().max.y;

            if (willHurtEnemy)
            {
                var enemyHealth = HotUpdateBehaviour.GetComponentByType(enemy.gameObject, typeof(Health)) as Health;
                if (enemyHealth != null)
                {
                    enemyHealth.Decrement();
                    if (!enemyHealth.IsAlive())
                    {
                        (Simulation.Schedule(typeof(EnemyDeath)) as EnemyDeath).enemy = enemy;
                        player.Bounce(2);
                    }
                    else
                    {
                        player.Bounce(7);
                    }
                }
                else
                {
                    (Simulation.Schedule(typeof(EnemyDeath)) as EnemyDeath).enemy = enemy;
                    player.Bounce(2);
                }
            }
            else
            {
                Simulation.Schedule(typeof(PlayerDeath));
            }
        }
    }
}