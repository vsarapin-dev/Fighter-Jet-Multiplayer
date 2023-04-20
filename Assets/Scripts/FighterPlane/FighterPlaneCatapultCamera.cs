using System.Collections;
using Enums;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

namespace FighterPlane
{
    public class FighterPlaneCatapultCamera: NetworkBehaviour
    {
        [SerializeField] private Transform catapultCamera;
        [SerializeField] private Transform cameraLookAtInTransitionToCockpit;
        [SerializeField] private Transform cameraLookAtWhileGlassOpening;
        [SerializeField] private float transitionTime = 2f;

        private Vector3 _startingPosition;
        private Quaternion _startingRotation;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private Quaternion _currentCatapultCameraRotation;
        
        private float _startCatapultCameraTime;
        private float _elapsedTime;
        private float _animationLookAtDuration;
        private bool _glitchAlreadyActivated = false;
        private bool _cameraReachedFinalPosition = false;
        private bool _startCameraTransition = false;
        private bool _startCameraLookAtCockpitGlass = false;
        
        // Glitch Volume
        private GameObject _glitchPostProcessGlobalVolumeGo;

        public bool CameraReachedFinalPosition => _cameraReachedFinalPosition;

        private void OnEnable()
        {
            Actions.OnSetDyingPlaneBehaviour += StartCameraTransition;
        }

        private void OnDisable()
        {
            Actions.OnSetDyingPlaneBehaviour -= StartCameraTransition;
        }
        
        private void Start()
        {
            _glitchPostProcessGlobalVolumeGo = GameObject.FindWithTag("GlitchVolume");
        }

        private void Update()
        {
            ProcessTransitionToPilotPlace();
            ProcessCameraLookAtCockpitGlass();
        }

        public void LookAtCockpit(float animationDuration)
        {
            _elapsedTime = 0f;
            _currentCatapultCameraRotation = transform.rotation;
            _startCatapultCameraTime = Time.time;
            _animationLookAtDuration = animationDuration;
            _startCameraLookAtCockpitGlass = true;
        }

        public void SetStartCameraTransform(Vector3 position, Quaternion rotation)
        {
            _startingPosition = transform.InverseTransformPoint(position);
            _startingRotation = rotation;
        }

        private void ProcessCameraLookAtCockpitGlass()
        {
            if (_startCameraLookAtCockpitGlass == false) return;
            
            _elapsedTime = Time.time - _startCatapultCameraTime;

            Vector3 targetDirection = (cameraLookAtWhileGlassOpening.position - catapultCamera.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            catapultCamera.rotation = Quaternion.Slerp(_currentCatapultCameraRotation, targetRotation, _elapsedTime / _animationLookAtDuration);

            if (_elapsedTime >= 2f)
            {
                catapultCamera.rotation = targetRotation;
                _startCameraLookAtCockpitGlass = false;
            }
        }

        private void ProcessTransitionToPilotPlace()
        {
            if (_startCameraTransition == false) return;

            if (_elapsedTime >= transitionTime)
            {
                catapultCamera.transform.localPosition = _targetPosition;
                catapultCamera.transform.rotation = _targetRotation;
                _startCameraTransition = false;
                _cameraReachedFinalPosition = true;
                return;
            }
            
            if (cameraLookAtInTransitionToCockpit != null)
            {
                ActivateGlitch();
                
                _elapsedTime += Time.deltaTime;

                float t = Mathf.Clamp01(_elapsedTime / transitionTime);

                catapultCamera.transform.localPosition = Vector3.Lerp(_startingPosition, _targetPosition, t);
                catapultCamera.transform.rotation = Quaternion.Lerp(_startingRotation, _targetRotation, t);
            }
        }

        private void ActivateGlitch()
        {
            if (_glitchAlreadyActivated) return;
            _glitchAlreadyActivated = true;

            if (_glitchPostProcessGlobalVolumeGo != null)
            {
                _glitchPostProcessGlobalVolumeGo.GetComponent<Volume>().enabled = true;
            }
        }

        private void StartCameraTransition(FighterPlaneType fighterPlaneType)
        {
            if (hasAuthority == false) return;
            
            FighterPlaneType currentFighterPlaneType = GetComponent<FighterPlanePlayerType>().FighterPlaneType;

            if (currentFighterPlaneType != fighterPlaneType) return;

            DefineStartAndTargetTransforms();
            _startCameraTransition = true;
        }

        private void DefineStartAndTargetTransforms()
        {
            if (cameraLookAtInTransitionToCockpit != null)
            {
                _targetPosition = cameraLookAtInTransitionToCockpit.localPosition;
                _targetRotation = cameraLookAtInTransitionToCockpit.rotation;
            }
        }
    }
}