using UnityEngine;
using UnityEngine.Events;
using Weapons.Scripts;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private ObjectPool<Transform> bulletPool;
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform bulletPos;
    
    [SerializeField] private ShootInputBase input;

    [SerializeField] private UnityEvent OnFire;
    [SerializeField] private Vector3Event OnBulletCollide;
    private bool _canShoot = false;

    private void Awake()
    {
        weapon = Instantiate(weapon);
        weapon.Init();
        UpdateManager.OnUpdate += weapon.Update;
        UpdateManager.OnFixedUpdate += weapon.FixedUpdate;
        UpdateManager.OnLateUpdate += weapon.LateUpdate;
        bulletPool.Initialised += SceneChangerOnFinishedLoading;
        weapon.BulletCollision += BulletCollide;
    }

    private void OnDestroy()
    {
        UpdateManager.OnUpdate -= weapon.Update;
        UpdateManager.OnFixedUpdate -= weapon.FixedUpdate;
        UpdateManager.OnLateUpdate -= weapon.LateUpdate;
        bulletPool.Initialised -= SceneChangerOnFinishedLoading;
        weapon.BulletCollision -= BulletCollide;
    }

    private void BulletCollide(Vector3 pos)
    {
        OnBulletCollide?.Invoke(pos);
    }
    
    private void SceneChangerOnFinishedLoading()
    {
        _canShoot = true;
    }

    private void Update()
    {
        if (!_canShoot) return;
        if (!input.IsShooting()) return;
        
        if (weapon.Fire(bulletPool, bulletPos))
            OnFire?.Invoke();
    }
        
    private void OnDrawGizmos()
    {
        weapon.DrawGizmos(bulletPos);
    }
}