using CSharpLike;

namespace PlatformerMicrogame
{
    /// <summary>
    /// Fired when the player has died.
    /// </summary>
    /// <typeparam name="PlayerDeath"></typeparam>
    public class PlayerDeath
    {
        public void Execute()
        {
            PlatformerModel model = GameController.Model;
            var player = model.player;
            if (player.health.IsAlive())
            {
                player.health.Die();
                model.virtualCamera.m_Follow = null;
                model.virtualCamera.m_LookAt = null;
                // player.collider.enabled = false;
                player.controlEnabled = false;

                if (player.audioSource && player.ouchAudio)
                    player.audioSource.PlayOneShot(player.ouchAudio);
                player.animator.SetTrigger("hurt");
                player.animator.SetBool("dead", true);
                Simulation.Schedule(typeof(PlayerSpawn), 2f);
            }
        }
    }
}