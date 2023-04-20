using UnityEngine;

namespace FighterPlane
{
    public class FighterPlaneDeathCollider : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 12 || other.gameObject.layer == 4)
            {
                transform.parent.GetComponent<FighterPlaneHealth>().MakePlayerDead();
            }
        }
    }
}
