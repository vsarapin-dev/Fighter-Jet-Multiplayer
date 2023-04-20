using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Mirror;
using Player;
using UnityEngine;

namespace FighterPlane
{
    public class FighterPlaneCatapultController : NetworkBehaviour
    {
        [Header("GameObjects")]
        [SerializeField] private FighterPlaneCatapultCamera fighterPlaneCatapultCameraScript;
        [SerializeField] private GameObject fighterPlaneCatapultCameraGo;
        [SerializeField] private GameObject fighterPlaneOnFireEffect;
        [SerializeField] private GameObject playerObjectToCreate;
        
        [Header("Catapult Settings")]
        [SerializeField] private KeyCode catapultKeyCode = KeyCode.C;
        [SerializeField] private Animator cockpitAnimator;
        [SerializeField] private Transform playerSpawnPositionOnCatapult;
        [SerializeField] private float delayBeforeExplosionAfterCatapulting = 2f;

        private FighterPlaneFollowCamera _fighterPlaneFollowCamera;
        private bool _catapultKeyIsPressed = false;
        private float _animationDuration = 0f;
        private IEnumerator _destroyPlaneCounterCoroutine;
        
        private void OnEnable()
        {
            Actions.OnSetDyingPlaneBehaviour += StartCatapultingBehaviourOnZeroHealth;
        }

        private void OnDisable()
        {
            Actions.OnSetDyingPlaneBehaviour -= StartCatapultingBehaviourOnZeroHealth;
        }

        private void Start()
        {
            if (hasAuthority == false) return;
            _fighterPlaneFollowCamera = FindObjectOfType<FighterPlaneFollowCamera>();
        }

        private void Update()
        {
            ProcessCatapulting();
        }
        
        public void MakePlayerDead()
        {
            CmdMakePlayerDead();
        }

        private void ProcessCatapulting()
        {
            if (hasAuthority == false) return;
            if (fighterPlaneCatapultCameraScript.CameraReachedFinalPosition == false) return;
            if (_catapultKeyIsPressed == true) return;
            
            if (Input.GetKeyDown(catapultKeyCode))
            {
                _catapultKeyIsPressed = true;
                StopCoroutine(_destroyPlaneCounterCoroutine);
                SetAnimationLookAtCockpitGlassDuration();
                LookAtCockpit();
                AnimateCockpitOpen();
                WaitTillAnimationEnd();
            }
        }

        private void LookAtCockpit()
        {
            fighterPlaneCatapultCameraScript.LookAtCockpit(_animationDuration);
        }
        
        private void AnimateCockpitOpen()
        {
            cockpitAnimator.Play("CockpitGlassOpenAnimation");
        }

        private void WaitTillAnimationEnd()
        {
            StartCoroutine(WaitTillAnimationEndCoroutine());
        }

        private IEnumerator WaitTillAnimationEndCoroutine()
        {
            yield return new WaitForSeconds(_animationDuration);
            DisableGlitchVolume();
            DisableCatapultCamera();
            DisableAllPlaneSoundsAndUI();
            CreatePlayer();
        }

        private void DisableCatapultCamera()
        {
            fighterPlaneCatapultCameraGo.SetActive(false);
        }
        
        private void DisableAllPlaneSoundsAndUI()
        {
            Actions.OnSetDisabledPlaneBehaviour?.Invoke();
        }
        
        private void DisableGlitchVolume()
        {
            GameObject glitchVolumeGo = GameObject.FindWithTag("GlitchVolume");

            if (glitchVolumeGo != null)
            {
                glitchVolumeGo.SetActive(false);
            }
        }
        
        private void CreatePlayer()
        {
            CmdCreatePlayer();
        }
        
        private void StartCatapultingBehaviourOnZeroHealth(FighterPlaneType fighterPlaneType)
        {
            if (hasAuthority == false) return;
            
            FighterPlaneType currentFighterPlaneType = GetComponent<FighterPlanePlayerType>().FighterPlaneType;

            if (currentFighterPlaneType != fighterPlaneType) return;

            //Start Timer To Destroy Plane If Catapulting Not Pressed
            _destroyPlaneCounterCoroutine = DestroyPlane(11.651f);
            StartCoroutine(_destroyPlaneCounterCoroutine);
            
            //Set Start Point Camera Transform Move From
            SetCatapultCameraStartTransform();
            
            //Deactivate Player Follow Camera
            _fighterPlaneFollowCamera.gameObject.SetActive(false);
            
            // Activate Camera Transition To Pilot Place
            fighterPlaneCatapultCameraGo.SetActive(true);
            
            // Activate Fire On Plane
            CmdShowFireOnPlane();
        }

        private void SetCatapultCameraStartTransform()
        {
            if (_fighterPlaneFollowCamera != null)
            {
                fighterPlaneCatapultCameraScript.SetStartCameraTransform(
                    _fighterPlaneFollowCamera.transform.position,
                    _fighterPlaneFollowCamera.transform.rotation
                );
            }
        }

        

        private void SetAnimationLookAtCockpitGlassDuration()
        {
            AnimationClip[] clips = cockpitAnimator.runtimeAnimatorController.animationClips;
            foreach(AnimationClip clip in clips)
            {
                if (clip.name == "CockpitGlassOpenAnimation")
                {
                    _animationDuration = clip.length;
                }
            }
        }

        #region Server

        [Command]
        private void CmdMakePlayerDead()
        {
            RpcMakePlayerDead(GetComponent<FighterPlanePlayerType>().FighterPlaneType);
        }
        
        [Command]
        private void CmdShowFireOnPlane()
        {
            RpcShowFireOnPlane(GetComponent<FighterPlanePlayerType>().FighterPlaneType);
        }
        
        [Command]
        private void CmdCreatePlayer()
        {
            GameObject player = Instantiate(
                playerObjectToCreate,
                playerSpawnPositionOnCatapult.position,
                Quaternion.identity
                );
            
            NetworkServer.Spawn(player, connectionToClient);

            PilotType pilotType = GetComponent<FighterPlanePlayerType>().PilotType;
            PilotPlayer pilotPlayer = player.GetComponent<PilotPlayer>();
            pilotPlayer.SetPlayerType(PilotType.None, pilotType);
        }

        private IEnumerator DestroyPlane(float waitForSeconds)
        {
            yield return new WaitForSeconds(waitForSeconds);
            GetComponent<FighterPlaneHealth>().MakePlayerDead();
        }

        #endregion

        #region Client

        [ClientRpc]
        private void RpcShowFireOnPlane(FighterPlaneType fighterPlaneType)
        {
            if (GetComponent<FighterPlanePlayerType>().FighterPlaneType == fighterPlaneType)
            {
                // Activate Fire On Plane
                fighterPlaneOnFireEffect.SetActive(true);
            }
        }
        
        [ClientRpc]
        private void RpcMakePlayerDead(FighterPlaneType fighterPlaneType)
        {
            FighterPlaneType currentFighterPlaneType = GetComponent<FighterPlanePlayerType>().FighterPlaneType;
            
            if (currentFighterPlaneType == fighterPlaneType)
            {
                StartCoroutine(DestroyPlane(delayBeforeExplosionAfterCatapulting));
            }
        }
        

        #endregion
    }
}
