using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(Transform target , float Amount);
    public void Die();
}

public interface IWeapon
{
    public void HandleDeflection();
    public void HandleBlocking();
    public void PlayWeaponParticles(Vector3 hitPosition);
}