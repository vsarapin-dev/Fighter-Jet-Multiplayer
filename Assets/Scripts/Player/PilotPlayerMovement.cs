using System;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PilotPlayerMovement: NetworkBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private bool jumpingEnabled = true;
        [SerializeField] private float jumpSpeed;
        [Range(0.0f, 0.5f)]
        [SerializeField] private float fallRate;
        [SerializeField] private bool  slopLimitEnabled;
        [SerializeField] private float slopeLimit;
        [Header("Sounds")]
        [SerializeField] private AudioClip footStepClip;
        [SerializeField] private AudioClip landingClip;
        [SerializeField] private AudioClip jumpingClip;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float footStepsVol;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float footStepsPitch;
        
        
        private bool _isRunning = false;
        private bool _isJumping = false;
        private bool _isLanding = false;
        private bool _canJump = true;
        private bool _prevGrounded = false;
        private bool _grounded;

        private Rigidbody _rb;
        private CapsuleCollider _capsule;
        private float _horizontalMovement;
        private float _verticalMovement;
        private Vector3 _moveDirection;
        private float _distanceToPoints;
        private Vector3 _YAxisGravity;
        private AudioSource _audioSource;
        private bool _isGrounded;

        #region Properties
        public bool IsRunning
        {
            get => _isRunning;
            set => _isRunning = value;
        }

        public bool IsLanding
        {
            get => _isLanding;
            set => _isLanding = value;
        }
        #endregion

        private void Start()
        {
            if (hasAuthority == false) return;
                
            _rb = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (hasAuthority == false) return;
            
            //jumping
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && jumpingEnabled && _canJump)
            {
                _isJumping = true;
                Jump();
                PlayJumpingSound();
            }

            _horizontalMovement = 0; _verticalMovement = 0;

            //Calculate FPcontroller movement direction through WASD and arrows Input
            if (AllowMovement(transform.right * Input.GetAxis("Horizontal")))
            {
                _horizontalMovement = Input.GetAxis("Horizontal");
            }
            if (AllowMovement(transform.forward * Input.GetAxis("Vertical")))
            {
                _verticalMovement = Input.GetAxis("Vertical");
            }
            //normalize vector so movement in two axis simultanesly is balanced.
            _moveDirection = (_horizontalMovement * transform.right + _verticalMovement * transform.forward).normalized;

            //Toggle run & jump
            IsRunning = Input.GetKey(KeyCode.LeftShift);

            if (!_prevGrounded && IsGrounded())
            {
                PlayLandingSound();
            }

            _prevGrounded = IsGrounded();
        }

        private void FixedUpdate()
        {
            if (hasAuthority == false) return;
            
            /* When calculating the moveDirection , the Y velocity always stays 0. 
             As a result the player is falling very slowly. 
             To solve this we add to the rb velocity the Y axis velocity */

            _YAxisGravity = new Vector3(0, _rb.velocity.y - fallRate, 0);
            if (!_isJumping) { Move(); }
            _rb.velocity += _YAxisGravity;
        }

        #region Player Movement

        public void Move()
        {
            if (!IsRunning)
            {
                _rb.velocity = _moveDirection * walkSpeed * Time.fixedDeltaTime * 100;
            }
            else
            {
                _rb.velocity = _moveDirection * runSpeed * Time.fixedDeltaTime * 100;
            }

            PlayFootStepsSound();
        }

        public void Jump()
        {
            if (_canJump)
            {
                _rb.AddForce(new Vector3(0, jumpSpeed, 0), ForceMode.Impulse);
            }
        }

        #endregion

        #region Sounds
        public void PlayFootStepsSound()
        {
            if (IsGrounded() && _rb.velocity.magnitude > 5.0f && !_audioSource.isPlaying)
            {
                if (IsRunning)
                {
                    _audioSource.volume = footStepsVol;
                    //audioSource.pitch = Random.Range(footstepspitch, footstepspitch + 0.15f);
                    _audioSource.pitch = footStepsPitch;
                }
                else
                {
                    _audioSource.volume = footStepsVol / 2;
                    _audioSource.pitch = .8f;
                }

                _audioSource.PlayOneShot(footStepClip);
            }
        }

        public void PlayLandingSound()
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.PlayOneShot(landingClip);
            }
        }

        public void PlayJumpingSound()
        {
            _audioSource.PlayOneShot(jumpingClip);
        }
        #endregion

        #region RayCasting

        // make a capsule cast to check weather there is an obstavle in front of the player ONLY when jumping.
        public bool AllowMovement(Vector3 castDirection)
        {
            Vector3 point1;
            Vector3 point2;
            if (!IsGrounded())
            {
                // The distance from the bottom and top of the capsule
                _distanceToPoints = _capsule.height / 2 - _capsule.radius;
                /*Top and bottom capsule points respectively, transform.position is used to get points relative to 
                   local space of the capsule. */
                point1 = transform.position + _capsule.center + Vector3.up * _distanceToPoints;
                point2 = transform.position + _capsule.center + Vector3.down * _distanceToPoints;
                float radius = _capsule.radius * .95f;
                float capsuleCastDist = 0.1f;

                if (Physics.CapsuleCast(point1, point2, radius, castDirection, capsuleCastDist))
                {
                    return false;
                }
            }
            if (slopLimitEnabled && IsGrounded())
            {
                float castDist = _capsule.height;
                RaycastHit hit;
                if (Physics.Raycast(transform.position + _capsule.center, Vector3.down, out hit, castDist)
                    && IsGrounded())
                {
                    float currentsSlope = Vector3.Angle(hit.normal, transform.forward) - 90.0f;
                    if (currentsSlope > slopeLimit)
                    {
                        _canJump = false;
                        return false;
                    }
                }
            }
            _canJump = true;
            return true;
        }

        // Make a sphere cast with down direction to define weather the player is touching the ground.
        public bool IsGrounded()
        {
            Vector3 capsule_bottom = transform.position + _capsule.center + Vector3.down * _distanceToPoints;
            float radius = 0.1f;
            float maxDist = 1.0f;
            RaycastHit hitInfo;
            if (Physics.SphereCast(capsule_bottom, radius, Vector3.down, out hitInfo, maxDist))
            {
                _isJumping = false;
                return true;
            }
            return false;
        }

        #endregion
        }
}