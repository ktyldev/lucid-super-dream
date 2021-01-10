using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons.Scripts;

public class AddScoreOnDie : MonoBehaviour
{
    [SerializeField] private ulong value = 100;
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
        Score.Value += value + (ulong)Random.Range(-5, 5);
    }
}