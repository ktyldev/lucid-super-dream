using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Jobs;
using Utils;
using Weapons.Scripts;
using Weapons.Spawning;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public class Weapon : ScriptableObject
{
    [BurstCompile]
    private struct BulletMoveJob : IJobParallelForTransform
    {
        public float DeltaTime;
        public NativeArray<Bullet> Bullets;

        public void Execute(int index, TransformAccess transform)
        {
            if (Bullets[index].IsAlive)
                transform.position += Bullets[index].Direction * Bullets[index].Speed * DeltaTime;
        }
    }

    private struct Bullet
    {
        public Vector3 Direction;
        public Vector3 PrevPos;
        public float Speed;
        public float Lifetime;
        public bool IsAlive;
        public int Idx;
    }

    public event Action<Vector3> BulletCollision;

    [SerializeField] private float fireRate;
    [SerializeField] private ParticleSystem.MinMaxCurve bulletSpeed;
    [SerializeField] private ParticleSystem.MinMaxCurve bulletLifetime;
    [SerializeField] private ParticleSystem.MinMaxCurve bulletSize;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask collidesWith;
    [SerializeField] private float accuracy;
    [SerializeField] private SpawnZone zone;
    [SerializeField] private bool manualFire;
    [SerializeField] private BaseBulletBehaviour behaviour;
    
    private List<Bullet> _bullets;
    private List<Transform> _bulletTransforms;
    private static Collider[] _results = new Collider[32];

    private ObjectPool<Transform> _pool;
    
    private BulletMoveJob _job;
    private JobHandle _jobHandle;

    private Bullet _currentBullet;
    private Transform _currentTransform;
    private TransformAccessArray _accessArray;

    private float _currentCooldown;
    
    public void Init()
    {
        _job = new BulletMoveJob();
        _bullets = new List<Bullet>();
        _bulletTransforms = new List<Transform>();
        _currentCooldown = fireRate;
    }

    public void Update()
    {
        _currentCooldown += Time.deltaTime;

        for (int i = 0; i < _bulletTransforms.Count; i++)
        {
            _currentBullet = _bullets[i];
            _currentBullet.PrevPos = _bulletTransforms[i].position;
            _bullets[i] = _currentBullet;
        }
        
        _job.DeltaTime = Time.deltaTime;
        _job.Bullets = _bullets.ToNativeArray(Allocator.Persistent);

        _accessArray = new TransformAccessArray(_bulletTransforms.ToArray());
        _jobHandle = _job.Schedule(_accessArray);
    }

    public void LateUpdate()
    {
        _jobHandle.Complete();
        _job.Bullets.Dispose();
        _accessArray.Dispose();
        // remove all the inactive bullets from our internal list so we don't accidentally update them multiple times
        for (var i = _bullets.Count - 1; i >= 0; i--)
        {
            _currentBullet = _bullets[i];
            _currentBullet.Lifetime -= Time.deltaTime;
            if (_currentBullet.Lifetime <= 0)
                _currentBullet.IsAlive = false;

            
            _bullets[i] = _currentBullet;
            
            if (_bullets[i].IsAlive && _bulletTransforms[i].gameObject.activeSelf) continue;
            
            _bulletTransforms[i].gameObject.SetActive(false);
            _pool.ReturnObject(_bulletTransforms[i], _bullets[i].Idx);
            _bullets.RemoveAt(i);
            _bulletTransforms.RemoveAt(i);
        }
    }

    public void FixedUpdate()
    {
        var toRemove = new List<int>();
        // loop through the bullets. Backwards, because that is marginally quicker
        for (var i = _bullets.Count - 1; i >= 0; i--)
        {
            _currentBullet = _bullets[i];
            _currentTransform = _bulletTransforms[i];
            
            // if the bullet is inactive, continue. We don't care about it
            if (!_currentTransform.gameObject.activeSelf || !_currentBullet.IsAlive) continue;

            // if it has hit something
            if (CheckCollision(_currentTransform, _currentBullet.PrevPos, _currentTransform.localScale.x, out var numHits))
            {
                // send the event
                BulletCollision?.Invoke(_currentTransform.position);
                
                // damage any damageable entities it has hit
                for (int j = 0; j < numHits; j++)
                {
                    var healthObject = _results[j].GetComponent<IDamageable>();
                    healthObject?.Hit(damage);
                }
                
                // deactivate the bullet
                _currentBullet.IsAlive = false;
                _currentTransform.gameObject.SetActive(false);
                _pool.ReturnObject(_currentTransform, _currentBullet.Idx);
                toRemove.Add(i);
            }
            
            // apply the changes we made
            _bullets[i] = _currentBullet;
        }
        foreach (var i in toRemove)
        {
            _bullets.RemoveAt(i);
            _bulletTransforms.RemoveAt(i);
        }
    }

    private bool CheckCollision(Transform instance, Vector3 prevPos, float size, out int numHits)
    {
        numHits = 0;
        
        // if it's inactive, we haven't hit anything
        if (!instance.gameObject.activeSelf) return false;
        if (instance.gameObject.name == "disabled") return false;
        
        // use the non allocating version so we don't have to allocate memory for every bullet
        numHits = Physics.OverlapCapsuleNonAlloc(prevPos, instance.position, size/2f, _results, collidesWith, QueryTriggerInteraction.Collide);

        return numHits > 0;
    }
    
    public bool Fire(ObjectPool<Transform> pool, Transform position)
    {
        if (_pool == null)
            _pool = pool;
        
        if (!manualFire)
        {
            if (_currentCooldown < fireRate) return false;
            _currentCooldown = 0;
        }
        
        SpawnBullets(position);
        return true;
    }
    
    /// <summary>
    /// Spawn the bullets.
    /// </summary>
    private void SpawnBullets(Transform transform)
    {
        // alter the direction based on accuracy
        var direction = Quaternion.Euler(0, Random.Range(-accuracy, accuracy), 0) * transform.forward;

        // for every bullet
        zone.GetPoint(transform, (point, dir) =>
        {
            // get the object in the pool
            var (bullet, idx) = _pool.GetObject();
            // if the pool returns no bullet, continue (it probably hasn't initialised yet)
            if (bullet == null) return;

            // enable the bullet
            bullet.gameObject.SetActive(true);

            var newPos = point;
            bullet.position = transform.position + newPos;
            
            // point the bullet in the right direction
            bullet.forward = dir;
            bullet.transform.localScale = Vector3.one * bulletSize.EvaluateMinMaxCurve();
            behaviour.DoBehaviour(bullet, bulletSize.EvaluateMinMaxCurve(), bullet.position);
            if (zone.SpawnDir != SpawnDir.Spherised)
            {
                var y = bullet.eulerAngles.y;
                bullet.forward = Quaternion.Euler(0, y, 0) * direction;
            }

            if (zone.SpawnDir == SpawnDir.None)
                bullet.forward = direction;
            // add the bullet to the list we're returning
            
            _bulletTransforms.Add(bullet);
            _bullets.Add(new Bullet
            {
                Direction = bullet.forward,
                PrevPos = bullet.position,
                IsAlive = true,
                Speed = bulletSpeed.EvaluateMinMaxCurve(),
                Lifetime = bulletLifetime.EvaluateMinMaxCurve(),
                Idx = idx
            });
        });
    }

    /// <summary>
    /// Draw gizmos for the weapon
    /// </summary>
    public void DrawGizmos(Transform transform)
    {
        #if UNITY_EDITOR
        var color = Color.cyan;
        color.a = Selection.activeObject == this ? 1 : 0.05f;
       zone.DrawGizmos(color, transform);

       Gizmos.color = Color.white;
       if (_bullets == null) return;
       for (int i = 0; i < _bulletTransforms.Count; i++)
       {
           var b = _bulletTransforms[i];
           if (b == null) continue;
           Gizmos.DrawSphere(b.position, b.localScale.x/2f );
           Gizmos.DrawSphere(_bullets[i].PrevPos, b.localScale.x/2f );
           Gizmos.DrawLine(b.position, _bullets[i].PrevPos);
           
       }
       #endif
    }
}
