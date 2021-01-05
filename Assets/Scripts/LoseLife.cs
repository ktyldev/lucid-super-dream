using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Weapons.Scripts;

public class LoseLife : MonoBehaviour
{
    [SerializeField] private Renderer[] healthMarkers;
    [SerializeField] private Renderer polygon;
    [SerializeField] private WeaponHandler weapon;
    private Color[] _markerColors;
    
    private EntityHealth _health;
    private Color _polygonColor;
    private Color _polygonColorClear;
    
    private void Awake()
    {
        _health = GetComponent<EntityHealth>();
        _polygonColor = polygon.material.color;
        _polygonColorClear = _polygonColor;
        _polygonColorClear.a = 0;
        
        _markerColors = new Color[healthMarkers.Length];
        for (int i = 0; i < healthMarkers.Length; i++)
        {
            _markerColors[i] = healthMarkers[i].material.color;
            _markerColors[i].a = 0;
        }
    }

    public void LifeLost(int livesLeft)
    {
        healthMarkers[livesLeft].transform.DOScale(Vector3.one * 5f,  1f).SetEase(Ease.InQuint).SetUpdate(true);
        healthMarkers[livesLeft].material.DOColor(_markerColors[livesLeft], 1f).SetDelay(0.25f).SetUpdate(true);
        _health.enabled = false;
        weapon.enabled = false;
        var timeVal = Time.timeScale;
        Time.timeScale = 0;
        WaitUtils.Wait(0.1f,  false, () => Time.timeScale = timeVal);
        
        var sequence = DOTween.Sequence();
        for (int i = 0; i < 5; i++)
        {
            sequence.Append(polygon.material.DOColor(_polygonColorClear, 0.2f).SetUpdate(true));
            sequence.Append(polygon.material.DOColor(_polygonColor, 0.2f).SetUpdate(true));
        }

        sequence.Play().OnComplete(() =>
        {
            weapon.enabled = true;
            _health.enabled = true;
        });
    }
}

