using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour where T : Component
{
    [SerializeField] private int numObjects;
    [SerializeField] private T prefab;
    [SerializeField] private int numToSpawnPerFrame = 10;
    
    private List<T> _pool;
    private List<GameObject> _poolGameObjects;

    private List<int> _indicesToRemove;
    
    public event Action<T> OnObjectFound;
    public event Action Initialised;

    public (T, int) GetObject()
    {
        T foundObject = null;
        int idx = -1;
        if (_pool == null) return (null, -1);

        for (var index = _pool.Count - 1; index >= 0; index--)
        {
            if (!_poolGameObjects[index].activeSelf)
            {
                idx = index;
                foundObject = _pool[index];
                break;
            }
        }

        if (foundObject == null)
            Debug.LogWarning("No more objects in pool!");
        else
            OnObjectFound?.Invoke(foundObject);

        return (foundObject, idx);
    }

    private void LateUpdate()
    {
        for (int i = _indicesToRemove.Count - 1; i >= 0; i--)
        {
            var obj = _pool[i];
            _pool.RemoveAt(i);
            _poolGameObjects.RemoveAt(i);
            
            _pool.Add(obj);
            _poolGameObjects.Add(obj.gameObject);
        }
        _indicesToRemove.Clear();
    }

    public void ReturnObject(T obj, int idx)
    {
        _indicesToRemove.Add(idx);
    }
    
    public IEnumerator Start()
    {
        _indicesToRemove = new List<int>();
        _pool = new List<T>(numObjects);
        _poolGameObjects = new List<GameObject>(numObjects);
        for (int i = 0; i < numObjects; ++i)
        {
            var instance = Instantiate(prefab, transform);
            instance.gameObject.SetActive(false);
            _pool.Add(instance);
            _poolGameObjects.Add(instance.gameObject);

            if (i % numToSpawnPerFrame == 0)
                yield return null;
        }
        
        Initialised?.Invoke();
    }
}