
namespace PlatformerMicrogame
{

    /// <summary>
    /// This event is triggered when the player character enters a trigger with a VictoryZone component.
    /// </summary>
    /// <typeparam name="PlayerEnteredVictoryZone"></typeparam>
    public class PlayerEnteredVictoryZone
    {
        public VictoryZone victoryZone;

        public void Execute()
        {
            PlatformerModel model = GameController.Model;
            model.player.animator.SetTrigger("victory");
            model.player.controlEnabled = false;
        }
    }
}