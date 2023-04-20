using System.Threading.Tasks;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PilotPlayerCamera : NetworkBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform playerBody;
        [SerializeField] private float XMinRotation;
        [SerializeField] private float XMaxRotation;
        [SerializeField] private int waitBeforeCanControlCamera = 1500;
        [Range(1.0f, 10.0f)] [SerializeField] private float Ysensitivity;
        [Range(1.0f, 10.0f)] [SerializeField] private float Xsensitivity;
    
        private float _rotAroundX;
        private float _rotAroundY;
        
        private Transform _target;
        private bool _canRotateCamera;
        private bool _timerStartedBeforeGiveCameraControl;


        private void Start()
        {
            if (hasAuthority == false)
            {
                cameraTransform.gameObject.SetActive(false);
                _target = null;
                return;
            }
            Cursor.lockState = CursorLockMode.Locked;
            _rotAroundX = transform.eulerAngles.x;
            _rotAroundY = transform.eulerAngles.y;
        }

        private void Update()
        {
            if (hasAuthority == false) return;
            
            CameraLookAtPlane();
            CameraRotate();
        }
        
        public void SetLookAtTransform(Transform target)
        {
            _target = target;
        }

        private void CameraLookAtPlane()
        {
            if (_target != null)
            {
                _canRotateCamera = false;
                transform.LookAt(_target);
            }
            else
            {
                WaitTillCanControlCamera();
            }
        }

        private async void WaitTillCanControlCamera()
        {
            if (_timerStartedBeforeGiveCameraControl == false)
            {
                _timerStartedBeforeGiveCameraControl = true;
                await Task.Delay(waitBeforeCanControlCamera);
                _canRotateCamera = true;
            }
        }

        private void CameraRotate()
        {
            if (_canRotateCamera == false) return;
            
            _rotAroundX += Input.GetAxis("Mouse Y") * Xsensitivity;
            _rotAroundY += Input.GetAxis("Mouse X") * Ysensitivity;

            _rotAroundX = Mathf.Clamp(_rotAroundX, XMinRotation, XMaxRotation);

            CameraRotation();
        }
        
        private void CameraRotation()
        {
            playerBody.rotation = Quaternion.Euler(0, _rotAroundY, 0);
            cameraTransform.transform.rotation = Quaternion.Euler(-_rotAroundX, _rotAroundY, 0);
        }
    }
}