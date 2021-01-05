using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Weapons.Scripts;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class ImpulseOnEntityHit : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 0.5f;
    [SerializeField] private EntityHealth health;
    
    private float cooldownTimer;
    private CinemachineImpulseSource _source;

    private void Awake()
    {
        _source = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
    }

    private void OnEnable()
    {
        health.UpdateHealth += UpdateHealth;
    }

    private void OnDisable()
    {
        health.UpdateHealth -= UpdateHealth;
    }

    private void UpdateHealth(float _, bool crit)
    {
        if (cooldownTimer < cooldownTime)
            return;
        cooldownTimer = 0;
        _source.GenerateImpulse(crit ? 5 : 1);
    }
}

/*
 * TODO:
 * - shields & animation
 * - laser attack
 * - multiple phases
 * - menu & pause menu
 * - second and third levels :
 *     - throws chunks of stuff at you, blocks part of the track? 
 *     - lots of mini bosses and bullet circles 
 */
