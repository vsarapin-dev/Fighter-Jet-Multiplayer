using Interfaces;
using Messages;

namespace StateBehaviours.FighterPlane
{
    public class DisabledPlaneStateBehaviour: IPlaneBehaviour
    {
        public void Enter(PlaneStateMessage planeStateMessage)
        {
            planeStateMessage.FighterPlaneUi.DisableAllPlaneUi();
            planeStateMessage.FighterPlaneSounds.DisableAllPlaneSounds();
        }
    }
}