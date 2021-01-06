using UnityEngine;

[CreateAssetMenu]
public abstract class BaseBulletBehaviour : ScriptableObject
{
    public abstract void DoBehaviour(Transform bullet, float size, Vector3 pos);
}