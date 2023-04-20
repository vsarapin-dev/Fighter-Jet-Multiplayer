using Mirror;
using UnityEngine;

namespace FighterPlane
{
    public class FighterPlaneController : NetworkBehaviour
    {
        [Header("Fighter plane angles, speed, rigidbody")]
        [SerializeField] private float fighterPlaneSpeed = 4500f;
        [SerializeField] private float maxRollAngle = 90f;
        [SerializeField] private float pitchSpeed = 0.5f;
        [SerializeField] private float maxPitchAngle = 90f;
        [SerializeField] private float returnSpeed = 1f;
        [SerializeField] private Rigidbody rb;
    
        // Private plane control settings
        private FighterPlaneFollowCamera _sceneCamera;
        private bool _canControl = true;
        private float _pitchInput;
        private float _pitchAngle;
        private float _yawInput;
        private float _rollAngle;
        private float _currentYawAngle;

        

    
        private void Start() 
        {
            if (hasAuthority == false) return;

            FindCamera();
        }
    
        private void Update()
        {
            if (hasAuthority == false) return;
            if (_canControl == false) return;
        
            _pitchInput = -Input.GetAxis("Vertical");
            _yawInput = Input.GetAxis("Horizontal");
        }
    
        void FixedUpdate()
        {
            if (hasAuthority == false) return;
        
            ProcessMove();
        }

        
        // Methods
    
        private void ProcessMove()
        {
            float targetPitchAngle = _pitchInput * maxPitchAngle;

            _pitchAngle = Mathf.Lerp(_pitchAngle, targetPitchAngle, pitchSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(_pitchAngle, _currentYawAngle, _rollAngle);

            if (Mathf.Abs(_pitchInput) < 0.1f)
            {
                _pitchAngle = Mathf.Lerp(_pitchAngle, 0f, returnSpeed * Time.fixedDeltaTime);
            }

            _currentYawAngle += _yawInput;
            transform.rotation *= Quaternion.Euler(0f, _yawInput, 0f);

            float targetRollAngle = -_yawInput * maxRollAngle;
            _rollAngle = Mathf.Lerp(_rollAngle, targetRollAngle, Time.fixedDeltaTime);

            rb.AddRelativeForce(Vector3.forward);
        
            Vector3 AddPos = Vector3.forward;
            AddPos = rb.rotation * AddPos;
            rb.velocity = AddPos * (Time.fixedDeltaTime * fighterPlaneSpeed);
        }

        private void FindCamera()
        {
            _sceneCamera = FindObjectOfType<FighterPlaneFollowCamera>();
        
            if (_sceneCamera != null)
            {
                _sceneCamera.target = transform;
            }
        }
    }
}