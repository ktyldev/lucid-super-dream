using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using Weapons.Scripts;

public class FireAtTargets : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private ObjectPool<Transform> bulletPool;
    [SerializeField] private Transform bulletPos;
    
    [SerializeField] private UnityEvent OnFire;
    [SerializeField] private Vector3Event OnBulletCollide;
    [SerializeField] private int numTargets = 4;
    
    private void Awake()
    {
        weapon = Instantiate(weapon);
        weapon.Init();
        UpdateManager.OnUpdate += weapon.Update;
        UpdateManager.OnFixedUpdate += weapon.FixedUpdate;
        UpdateManager.OnLateUpdate += weapon.LateUpdate;
        weapon.BulletCollision += BulletCollide;
    }
    
    private void OnDestroy()
    {
        UpdateManager.OnUpdate -= weapon.Update;
        UpdateManager.OnFixedUpdate -= weapon.FixedUpdate;
        UpdateManager.OnLateUpdate -= weapon.LateUpdate;
        weapon.BulletCollision -= BulletCollide;
    }

    private void BulletCollide(Vector3 pos)
    {
        OnBulletCollide?.Invoke(pos);
    }

    public void Fire(List<Transform> targets)
    {
        FireIE(targets).Run();
    }

    private IEnumerator FireIE(List<Transform> targets)
    {
        var currentTargets = new List<Transform>(targets);
        for (var i = 0; i < currentTargets.Count; i++)
        {
            bulletPos.LookAt(currentTargets[i]);
            if (weapon.Fire(bulletPool, bulletPos))
                OnFire?.Invoke();

            if (i % numTargets == 0)
                yield return null;
        }
    }
    
    private void OnDrawGizmos()
    {
        weapon.DrawGizmos(bulletPos);
    }
}
