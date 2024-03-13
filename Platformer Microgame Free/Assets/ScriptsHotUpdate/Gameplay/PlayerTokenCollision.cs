
namespace PlatformerMicrogame
{
    /// <summary>
    /// Fired when a player collides with a token.
    /// </summary>
    /// <typeparam name="PlayerCollision"></typeparam>
    public class PlayerTokenCollision
    {
        public PlayerController player;
        public TokenInstance token;

        public void Execute()
        {
            player.audioSource.PlayOneShot(token.tokenCollectAudio);
            //AudioSource.PlayClipAtPoint(token.tokenCollectAudio, token.transform.position);
        }
    }
}