using Messages;

namespace Interfaces
{
    public interface IPlaneBehaviour
    {
        public void Enter(PlaneStateMessage planeStateMessage);
    }
}