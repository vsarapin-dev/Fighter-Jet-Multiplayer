using System;
using System.Collections;
using Enums;
using Interfaces;
using Mirror;
using UnityEngine;

namespace FighterPlane
{
    [RequireComponent(typeof(FighterPlaneExplosion))]
    public class FighterPlaneHealth: NetworkBehaviour, IDamageable
    {
        [Header("Low hp audio settings")]
        [SerializeField] private AudioSource lowHpAudioSource;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float healthChangeDuration = 0.3f;

        [SerializeField] private int healthCriticalValue = 25;
        
        [SyncVar] private bool _isAlive = true;
        
        [SyncVar] private float _currentHealth;

        public bool IsAlive => _isAlive;
        public float Health => _currentHealth;

        public override void OnStartAuthority()
        {
            CmdSetDefaultHealthPoints();
            CmdSetIsAlive(_isAlive);
        }
        
        public void TakeDamage(float damage)
        {
            if (_currentHealth <= 0) return;
            
            if (isServer)
            {
                _currentHealth -= damage;
                RpcChangeHealth(_currentHealth);
            }
            else
            {
                CmdTakeDamage(damage);
            }
        }

        [Command]
        private void CmdTakeDamage(float damage)
        {
            TakeDamage(damage);
        }

        public void MakePlayerDead()
        {
            if (hasAuthority == false) return;
            
            CmdSetIsAlive(false);
            CmdCreateExplosionOnDeath();
        }

        private void ChangeHealthOnUi()
        {
            float healthFillAmountPercent = _currentHealth / maxHealth;
            if (hasAuthority)
            {
                Actions.OnChangeCurrentPlayerHealthOnUi?.Invoke(healthFillAmountPercent, healthChangeDuration);
            }
            else
            {
                Actions.OnChangeEnemyPlayerHealthOnUi?.Invoke(healthFillAmountPercent, healthChangeDuration);
            }
        }
        
        private void CheckIsBelowCriticalHealthValue()
        {
            if (_currentHealth <= healthCriticalValue && _currentHealth > 0)
            {
                Actions.OnSetCriticalPlaneBehaviour?.Invoke();
            }
        }

        private void CheckIsZeroHealthPoints()
        {
            if (_currentHealth <= 0)
            {
                FighterPlaneType fighterPlaneType = GetComponent<FighterPlanePlayerType>().FighterPlaneType;
                Actions.OnSetDyingPlaneBehaviour?.Invoke(fighterPlaneType);
            }
        }
        
        
        #region Server

        [Command]
        private void CmdSetDefaultHealthPoints()
        {
            _currentHealth = maxHealth;
        }

        [Command]
        private void CmdSetIsAlive(bool isAlive)
        {
            _isAlive = isAlive;
        }
        
        [Command]
        private void CmdCreateExplosionOnDeath()
        {
            FighterPlaneExplosion fighterPlaneExplosion = GetComponent<FighterPlaneExplosion>();
            fighterPlaneExplosion.Explode();
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client
        
        [ClientRpc]
        private void RpcChangeHealth(float health)
        {
            _currentHealth = health;
            
            ChangeHealthOnUi();

            if (hasAuthority == false) return;

            CheckIsBelowCriticalHealthValue();
            CheckIsZeroHealthPoints();
        }

        #endregion
    }
}
