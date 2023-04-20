using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Mirror;
using UnityEngine;

namespace FighterPlane
{
    public class FighterPlaneShoot : NetworkBehaviour, IShootable
    {
        [SerializeField] private List<Transform> laserStartPositions = new List<Transform>();

        // Shoot Settings
        private GameObject _laserPrefab;
        private float _damagePerShot;
        private float _delayBetweenShots;
        private KeyCode _attackKey;

        private bool _canCreateLaser = true;

        public float DamagePerShot => _damagePerShot;
        public float DelayBetweenShots => _delayBetweenShots;
        public KeyCode AttackKey => _attackKey;

    
        public void SetLaserPrefab(GameObject laserPrefab)
        {
            _laserPrefab = laserPrefab;
        }

        public void SetDamagePerShot(float damagePerShot)
        {
            _damagePerShot = damagePerShot;
        }
    
        public void SetDelayBetweenShots(float delayBetweenShots)
        {
            _delayBetweenShots = delayBetweenShots;
        }
    
        public void SetAttackKey(KeyCode attackKey)
        {
            _attackKey = attackKey;
        }

        private void Update()
        {
            if (hasAuthority == false) return;
        
            ProcessShoot();
        }

        private void ProcessShoot()
        {
            if (Input.GetKey(AttackKey))
            {
                CmdShoot();
            }
        }

        [Command]
        private void CmdShoot()
        {
            if (_canCreateLaser == false) return;

            StartCoroutine(CreateLaser());
        }

        private IEnumerator CreateLaser()
        {
            _canCreateLaser = false;
            foreach (Transform laserStartPosition in laserStartPositions)
            {
                GameObject laser = Instantiate(_laserPrefab, laserStartPosition.position, laserStartPosition.rotation);
                FighterPlaneLaser fighterPlaneLaser = laser.GetComponent<FighterPlaneLaser>();
                fighterPlaneLaser.ProjectileDamage = DamagePerShot;
                NetworkServer.Spawn(laser, connectionToClient);
            }
            yield return new WaitForSeconds(DelayBetweenShots);
            _canCreateLaser = true;
        }
    }
}
