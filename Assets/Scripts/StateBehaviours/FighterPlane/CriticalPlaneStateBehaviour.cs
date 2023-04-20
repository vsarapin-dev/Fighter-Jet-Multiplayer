using Interfaces;
using Messages;

namespace StateBehaviours.FighterPlane
{
    public class CriticalPlaneStateBehaviour : IPlaneBehaviour
    {
        public void Enter(PlaneStateMessage planeStateMessage)
        {
            planeStateMessage.FighterPlaneUi.OnCriticalHealthPointsWarningText(true);
            planeStateMessage.FighterPlaneSounds.ProcessCriticalHealthPointsSound(true);
        }
    }
}