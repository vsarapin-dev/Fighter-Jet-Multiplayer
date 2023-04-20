using Interfaces;
using Messages;

namespace StateBehaviours.FighterPlane
{
    public class DyingPlaneStateBehaviour : IPlaneBehaviour
    {
        public void Enter(PlaneStateMessage planeStateMessage)
        {
            planeStateMessage.FighterPlaneUi.OnCriticalHealthPointsWarningText(false);
            planeStateMessage.FighterPlaneSounds.ProcessCriticalHealthPointsSound(false);
            
            planeStateMessage.FighterPlaneUi.ShowCatapultMessage(planeStateMessage.FighterPlaneType);
            planeStateMessage.FighterPlaneSounds.CatapultCountdownAndWarningSoundPlay(planeStateMessage.FighterPlaneType);
        }
    }
}