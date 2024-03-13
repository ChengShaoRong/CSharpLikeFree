
namespace PlatformerMicrogame
{
    /// <summary>
    /// This event is fired when user input should be enabled.
    /// </summary>
    public class EnablePlayerInput
    {
        public void Execute()
        {
            var player = GameController.Model.player;
            player.controlEnabled = true;
        }
    }
}