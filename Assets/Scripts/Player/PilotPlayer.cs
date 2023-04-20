using Enums;
using FighterPlane;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PilotPlayer : NetworkBehaviour
    {
        [SerializeField] private PilotPlayerCatapultAction _pilotPlayerCatapultAction;
        [SerializeField] private GameObject playerCameraGo;
        
        [SyncVar(hook = nameof(SetPlayerType))] public PilotType pilotType;

        private void Start()
        {
            if (hasAuthority == false)
            {
                playerCameraGo.SetActive(false);
                return;
            }
            
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        public void SetPlayerType(PilotType oldPilotType, PilotType newPilotType)
        {
            if (isServer)
            {
                pilotType = newPilotType;
            }
            else
            {
                CmdSetPlayerType(newPilotType);
            }
            
            CatapultPilotOnSpawn();
        }

        private void CatapultPilotOnSpawn()
        {
            if (pilotType != PilotType.None)
            {
                FighterPlanePlayerType[] fighterPlanePlayerTypes = FindObjectsOfType<FighterPlanePlayerType>();
                FighterPlanePlayerType currentPlanePlayerType = null;
                
                foreach (FighterPlanePlayerType planePlayerType in fighterPlanePlayerTypes)
                {
                    if (planePlayerType.PilotType == pilotType)
                    {
                        currentPlanePlayerType = planePlayerType;
                    }
                }

                if (currentPlanePlayerType != null)
                {
                    _pilotPlayerCatapultAction.StartCatapultBehaviour(currentPlanePlayerType.transform);
                    currentPlanePlayerType.GetComponent<FighterPlaneCatapultController>().MakePlayerDead(); 
                }
            }
        }

        #region Server

        [Command]
        private void CmdSetPlayerType(PilotType pilotType)
        {
            this.pilotType = pilotType;
        }

        #endregion
    }
}