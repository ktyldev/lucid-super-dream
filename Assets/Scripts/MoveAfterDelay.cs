using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveAfterDelay : MonoBehaviour
{
    [SerializeField] private float delayMin;
    [SerializeField] private float delayMax;
    private void OnEnable()
    {
        transform.DOMoveZ(-15, 2.0f).SetEase(Ease.InQuint).SetDelay(Random.Range(delayMin, delayMax));
    }
}
