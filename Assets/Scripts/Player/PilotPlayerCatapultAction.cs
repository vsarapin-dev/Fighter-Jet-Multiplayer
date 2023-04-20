using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PilotPlayerCatapultAction : NetworkBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float arcHeight;

        [SyncVar] private Vector3 _positionToLand;
        [SyncVar] private bool _canCatapult = false;
        private Vector3 _startPosition;
        private float _stepScale;
        private float _stepSize = 0.1f;
        private float _progress;
        private List<Transform> _landingPlaces = new List<Transform>();
        

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            if (hasAuthority == false) return;
            if (_canCatapult == false) return;
            if (_positionToLand == Vector3.zero) return;

            _progress = Mathf.Min(_progress + Time.deltaTime * _stepSize, 1.0f);
            float parabola = 1.0f - 4.0f * (_progress - 0.5f) * (_progress - 0.5f);
            Vector3 nextPos = Vector3.Lerp(_startPosition, _positionToLand, _progress);
            nextPos.y += parabola * arcHeight;

            transform.position = nextPos;

            if (_progress == 1.0f)
            {
                _canCatapult = false;
            }
        }
        
        public void StartCatapultBehaviour(Transform planeTransform)
        {
            SetPlayerLookAtPlanePosition(planeTransform);
            FillLandingPlaces(); 
            CanCatapult();
        }

        private void CanCatapult()
        {
            if (hasAuthority == false) return;
            
            CmdCanCatapult();
        }
        
        private void FillLandingPlaces()
        {
            _landingPlaces.Clear();

            string playerType = GetComponent<PilotPlayer>().pilotType.ToString().ToLower();
            GameObject landingPlacesInScene = GameObject.FindWithTag("PlayerLandingPlaces");
            foreach (Transform child in landingPlacesInScene.transform)
            {
                if (child.name.ToLower().Contains(playerType))
                {
                    _landingPlaces.Add(child);
                }
            }

            if (_landingPlaces.Count > 0)
            {
                if (hasAuthority == false) return;
                
                CmdSetLandingPlace(_landingPlaces[0].position);
            }

            SetStepScale();
        }
        
        private void SetPlayerLookAtPlanePosition(Transform planeTransform)
        {
            PilotPlayerCamera pilotCameraScript = transform.GetComponent<PilotPlayerCamera>();
            pilotCameraScript.SetLookAtTransform(planeTransform);
        }

        private void SetStepScale()
        {
            float distance = Vector3.Distance(_startPosition, _positionToLand);
            _stepScale = speed / distance;
        }

        #region Server

        [Command]
        private void CmdCanCatapult()
        {
            _canCatapult = true;
        }
        
        [Command]
        private void CmdSetLandingPlace(Vector3 position)
        {
            _positionToLand = position;
        }

        #endregion
    }
}