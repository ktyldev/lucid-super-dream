using System;

namespace Weapons.Scripts
{
    public interface IDamageable
    {
        event Action<float, bool> UpdateHealth;
        void Hit(float damage, bool crit = false);
    }
}