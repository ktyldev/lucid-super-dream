using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Ktyl.Util;
using UnityEngine;
using Weapons.Scripts;

public class LoseLife : MonoBehaviour
{
    [SerializeField] private Renderer[] healthMarkers;
    [SerializeField] private Renderer polygon;

    [SerializeField] private TunnelController _tunnelController;
    
    private EntityHealth _health;
    
    private void Awake()
    {
        _health = GetComponent<EntityHealth>();
    }

    public void LifeLost(int livesLeft)
    {
        _tunnelController.LoseLifeShake();
        
        healthMarkers[livesLeft].transform.DOScale(Vector3.zero,  1f).SetEase(Ease.InBack).SetUpdate(true);
        _health.enabled = false;
        var timeVal = Time.timeScale;
        Time.timeScale = 0;
        WaitUtils.Wait(0.1f,  false, () => Time.timeScale = timeVal);
        
        var sequence = DOTween.Sequence();
        for (int i = 0; i < 5; i++)
        {
            sequence.Append(polygon.material.DOFloat(1,"_FlashAmount", 0.2f).SetUpdate(true));
            sequence.Append(polygon.material.DOFloat(0,"_FlashAmount", 0.2f).SetUpdate(true));
        }

        sequence.Play().OnComplete(() =>
        {
            _health.enabled = true;
        });
    }
}

