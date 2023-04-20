using System;
using Enums;
using Messages;
using Mirror;
using Mirror.Experimental;
using RadarComponents;
using Steamworks;
using UnityEngine;

namespace FighterPlane
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(NetworkRigidbody))]
    [RequireComponent(typeof(FighterPlaneShoot))]
    [RequireComponent(typeof(FighterPlaneHealth))]
    [RequireComponent(typeof(FighterPlaneExplosion))]
    [RequireComponent(typeof(FighterPlaneController))]
    [RequireComponent(typeof(FighterPlanePlayerType))]
    [RequireComponent(typeof(FighterPlaneCatapultCamera))]
    [RequireComponent(typeof(FighterPlaneCatapultController))]
    
    public class FighterPlaneSettingsInitializer: NetworkBehaviour
    {
        [SerializeField] private PlayerSettings fighterPlayerSettings;
        [SerializeField] private FighterPlaneShoot fighterPlaneShoot;
        
        [Header("Radar Settings")]
        [SerializeField] private Target fighterPlaneAsTarget;
        [SerializeField] private PlayerLocator fighterPlaneLocator;
        
        [SyncVar(hook = nameof(OnSetPlayerName))] private string _playerName;

        public override void OnStartAuthority()
        {
            CmdSetPlayerName(SteamFriends.GetPersonaName());
            CmdActivateRadar();
        }
        
        public override void OnStartClient()
        {
            SetPlaneShootSettings();
        }

        public void SetFighterPlaneAsTarget()
        {
            fighterPlaneAsTarget.enabled = true;
        }
        
        private void SetPlaneShootSettings()
        {
            fighterPlaneShoot.SetLaserPrefab(fighterPlayerSettings.projectilePrefab);
            fighterPlaneShoot.SetDamagePerShot(fighterPlayerSettings.damagePerShot);
            fighterPlaneShoot.SetDelayBetweenShots(fighterPlayerSettings.delayBetweenShots);
            fighterPlaneShoot.SetAttackKey(fighterPlayerSettings.attackKey);
        }

        private void EnablePlayerRadar(bool activate)
        {
            fighterPlaneLocator.enabled = activate;
        }
        
        private void EnableEnemyAsTarget(bool activateOnThisPlane)
        {
            if (activateOnThisPlane)
            {
                SetFighterPlaneAsTarget();
                return;
            }
            
            FighterPlanePlayerType[] fighterPlanePlayerTypes = FindObjectsOfType<FighterPlanePlayerType>();
            FighterPlaneType currentFighterPlaneType = GetComponent<FighterPlanePlayerType>().FighterPlaneType;
            
            foreach (FighterPlanePlayerType fighterPlanePlayerType in fighterPlanePlayerTypes)
            {
                if (fighterPlanePlayerType.FighterPlaneType != currentFighterPlaneType)
                {
                    FighterPlaneSettingsInitializer enemySettings = fighterPlanePlayerType.GetComponent<FighterPlaneSettingsInitializer>();
                    enemySettings.SetFighterPlaneAsTarget();
                }
            }
        }
    
        private void OnSetPlayerName(string oldName, string newName)
        {
            if (newName == String.Empty) return;
            
            PlaneStateMessage planeStateMessage = new PlaneStateMessage();
            
            if (hasAuthority)
            {
                planeStateMessage.IsEnemy = false;
                planeStateMessage.PlayerName = _playerName;
                planeStateMessage.PlayerPlaneName = fighterPlayerSettings.playerModelName;
                planeStateMessage.PlayerSprite = fighterPlayerSettings.playerHealthUISprite;
            }
            else
            {
                planeStateMessage.IsEnemy = true;
                planeStateMessage.EnemyName = _playerName;
                planeStateMessage.EnemyPlaneName = fighterPlayerSettings.playerModelName;
                planeStateMessage.EnemySprite = fighterPlayerSettings.playerHealthUISprite;
            }
            
            Actions.OnSetDefaultPlaneBehaviour?.Invoke(planeStateMessage);
        }

        #region Server

        [Command]
        private void CmdSetPlayerName(string playerName)
        {
            _playerName = playerName;
        }
        
        [Command]
        private void CmdActivateRadar()
        {
            RpcSetRadarSettings();
        }

        #endregion

        #region Client

        [ClientRpc]
        private void RpcSetRadarSettings()
        {
            EnablePlayerRadar(hasAuthority);
            EnableEnemyAsTarget(!hasAuthority);
            Actions.OnFighterPlaneRadarActivate?.Invoke(true);
        }

        #endregion
    }
}
