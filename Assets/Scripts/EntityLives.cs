using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Weapons.Scripts;

public class EntityLives : MonoBehaviour
{
    [SerializeField] private int lives = 3;
    public IntEvent OnDie;
    public UnityEvent OnGameOver;
    
    private EntityHealth _health;

    private void Awake()
    {
        _health = GetComponent<EntityHealth>();
    }

    private void OnEnable()
    {
        _health.Die += Die;
    }
    
    private void OnDisable()
    {
        _health.Die -= Die;
    }

    private void Die()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/hit");
        
        --lives;
        if (lives > 0)
        {
            OnDie?.Invoke(lives);
            _health.Reset();
        }
        else
            OnGameOver?.Invoke();
    }
}
