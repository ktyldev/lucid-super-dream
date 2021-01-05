using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Weapons.Spawning
{
    public enum SpawnType { Circle, Square, Polygon, Composite }
    public enum SpawnDir { None, Randomised, Spherised, Directional}
    [Serializable]
    public struct SpawnZone
    {
        [SerializeField] private int numToSpawn;
        [SerializeField] private Vector2 offset;
        
        [SerializeField] private SpawnType spawnType;
        [SerializeField] private SpawnDir spawnDir;
        
        [SerializeField] private float width;
        [SerializeField] private float height;
        [SerializeField] private bool surfaceOnly;
        
        [SerializeField] private bool evenDistribution;

        [SerializeField] private int numSides;
        [SerializeField] private int numPerSide;
        
        [SerializeField] private float radius;
        [SerializeField, Range(0, 360)] private float arc;

        [SerializeField] private SpawnZone[] composite;
        
        public SpawnDir SpawnDir => spawnDir;
        
        public int NumToSpawn => numToSpawn;

        public void GetPoint(Transform transform, Action<Vector2, Vector2> onGetPoint)
        {
            switch (spawnType)
            {
                case SpawnType.Circle:
                    SpawnCircle(transform, onGetPoint);
                    break;
                case SpawnType.Square:
                    SpawnSquare(transform, onGetPoint);
                    break;
                case SpawnType.Composite:
                    SpawnComposite(transform, onGetPoint);
                    break;
                case SpawnType.Polygon:
                    SpawnPoly(transform, onGetPoint);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SpawnPoly(Transform transform, Action<Vector2, Vector2> onGetPoint)
        {
            var points = new Vector2[numSides];
            for (int i = 0; i < numSides; i++)
            {
                var angle = (i * (arc * Mathf.Deg2Rad) / numSides) +
                            (90 - transform.eulerAngles.y - arc/2) * Mathf.Deg2Rad;
                    
                points[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            }

            for (int i = 0; i < numSides; i++)
            {
                var next = i + 1;
                if (next == numSides)
                    next = 0;

                var direction = Vector2.Lerp(points[i], points[next], 0.5f).normalized;
                
                for (int j = 0; j < numPerSide; j++)
                {
                    var t = j / (float) numPerSide;
                    t += (1f / numPerSide)/2f;
                    var point = Vector2.Lerp(points[i], points[next], t);
                    point *= radius;
                    
                    var dir = Vector2.up;
                    if (spawnDir == SpawnDir.Directional)
                        dir = direction;
                    else if (spawnDir == SpawnDir.Spherised)
                        dir = point.normalized;
                    else if (spawnDir == SpawnDir.Randomised)
                        dir = Random.insideUnitCircle.normalized;
                    
                    onGetPoint?.Invoke(point, dir);
                }
            }
        }

        private void SpawnComposite(Transform transform, Action<Vector2, Vector2> onGetPoint)
        {
            for (int i = 0; i < numToSpawn; i++)
            {
                composite[Random.Range(0, composite.Length)].GetPoint(transform, onGetPoint);
            }
        }
        
        private void SpawnCircle(Transform transform, Action<Vector2, Vector2> onGetPoint)
        {
            Assert.IsNotNull(onGetPoint);
            
            for (int i = 0; i < numToSpawn; i++)
            {
                Vector2 point;
                if (!evenDistribution)
                {
                    var angle = (Random.Range(-arc / 2f, arc / 2f) + 90 - transform.eulerAngles.y) * Mathf.Deg2Rad;

                    point.x = Mathf.Cos(angle);
                    point.y = Mathf.Sin(angle);

                    if (!surfaceOnly)
                        point *= Random.Range(0, 1f);

                }
                else
                {
                    var angle = (i * (arc * Mathf.Deg2Rad) / numToSpawn) +
                                (90 - transform.eulerAngles.y - arc/2) * Mathf.Deg2Rad;
                    
                    point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                    
                    if (!surfaceOnly)
                        point *= Random.Range(0, 1f);
                    
                }
                
                
                var dir = Vector2.up;
                if (spawnDir == SpawnDir.Spherised || spawnDir == SpawnDir.Directional)
                    dir = point.normalized;
                else if (spawnDir == SpawnDir.Randomised)
                    dir = Random.insideUnitCircle.normalized;

                
                onGetPoint((point * radius) + offset, dir);
            }
        }
        
        private void SpawnSquare(Transform transform, Action<Vector2, Vector2> onGetPoint)
        {
            Assert.IsNotNull(onGetPoint);
            
            for (int i = 0; i < numToSpawn; i++)
            {
                var point = new Vector2
                {
                    x = Random.Range(-.5f, .5f),
                    y = Random.Range(-.5f, .5f)
                };

                if (surfaceOnly)
                {
                    int axis = Random.Range(0, 2);
                    point[axis] = point[axis] < 0f ? -0.5f : 0.5f;
                }

                point.x *= width;
                point.y *= height;

                
                var dir = Vector2.up;
                if (spawnDir == SpawnDir.Spherised || spawnDir == SpawnDir.Directional)
                    dir = point.normalized;
                else if (spawnDir == SpawnDir.Randomised)
                    dir = Random.insideUnitCircle.normalized;

                
                onGetPoint(point + offset, dir);
            }
        }

        public void DrawGizmos(Color color, Transform transform)
        {
            switch (spawnType)
            {
                case SpawnType.Circle:
                    DrawGizmosCircle(color, transform);
                    break;
                case SpawnType.Square:
                    DrawGizmosSquare(color, transform);
                    break;
                case SpawnType.Composite:
                    DrawComposite(color, transform);
                    break;
                case SpawnType.Polygon:
                    DrawGizmosPoly(color,transform);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawComposite(Color color, Transform transform)
        {
            foreach (var zone in composite)
                zone.DrawGizmos(color, transform);
        }
        
        private void DrawGizmosCircle(Color color, Transform transform)
        {
            Gizmos.color = color;
            SpawnCircle(transform, (point, dir) =>
            {
                Gizmos.DrawSphere(new Vector3(point.x, 0, point.y), 0.25f);
                if (color.a > 0.5f)
                    Gizmos.DrawRay(new Vector3(point.x, 0, point.y), new Vector3(dir.x, 0, dir.y));
            });
        }

        private void DrawGizmosPoly(Color color, Transform transform)
        {
            Gizmos.color = color;
            SpawnPoly(transform, (point, dir) =>
            {
                Gizmos.DrawSphere(transform.position + new Vector3(point.x, 0, point.y), 0.25f);
                if (color.a > 0.5f)
                    Gizmos.DrawRay(transform.position + new Vector3(point.x, 0, point.y), new Vector3(dir.x, 0, dir.y));
            });
        }
        
        private void DrawGizmosSquare(Color color, Transform transform)
        {
            Gizmos.color = color;
            SpawnSquare(transform, (point, dir) =>
            {
                Gizmos.DrawSphere(new Vector3(point.x, 0, point.y), 0.25f);
                if (color.a > 0.5f)
                    Gizmos.DrawRay(new Vector3(point.x, 0, point.y), new Vector3(dir.x, 0, dir.y));
            });
        }
    }
}