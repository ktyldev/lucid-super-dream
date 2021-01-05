using System;
using DG.Tweening;
using UnityEngine;

namespace Weapons.Scripts
{
    public class EntityHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth;
        public float MaxHealth => maxHealth;
    
        private float _currentHealth;
        public event Action<float, bool> UpdateHealth;
        public event Action OnHit;
        
        public Action Die;

        public event Action OnBecameInvincible;
        public event Action OnBecameVulnerable;
        
        private float _shieldtimer = 0;

        private void Awake()
        {
            Reset();
        }

        private void OnEnable()
        {
            OnBecameVulnerable?.Invoke();
        }

        private void OnDisable()
        {
            OnBecameInvincible?.Invoke();
        }

        public void Reset()
        {
            _currentHealth = maxHealth;
        }

        private void Update()
        {
            if (_shieldtimer >= 0) 
                _shieldtimer -= Time.deltaTime;
        }

        public void Hit(float damage, bool crit = false)
        {
            if (gameObject == null) return;
            if ((!enabled && damage > 0) || _currentHealth <= 0) return;

            if (_shieldtimer >= 0)
                damage = Mathf.Min(0, damage);

            _currentHealth -= damage;
            if (damage > 0)
                OnHit?.Invoke();
            UpdateHealth?.Invoke(_currentHealth / maxHealth, crit);
            if (_currentHealth <= 0)
                Die?.Invoke();
        }
    }
}