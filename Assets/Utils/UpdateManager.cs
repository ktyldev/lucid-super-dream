using System;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    public static event Action OnUpdate;
    public static event Action OnFixedUpdate;
    public static event Action OnLateUpdate;
    
    private static UpdateManager _instance;
        
    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        new GameObject("UpdateManager").AddComponent<UpdateManager>();
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
            
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        OnUpdate?.Invoke();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate?.Invoke();
    }

    private void LateUpdate()
    {
        OnLateUpdate?.Invoke();
    }
}