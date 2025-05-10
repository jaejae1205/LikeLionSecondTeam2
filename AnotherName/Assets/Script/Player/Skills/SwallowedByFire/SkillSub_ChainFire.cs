using System.Collections.Generic;
using UnityEngine;

public class SkillSub_ChainFire : MonoBehaviour
{
    private HashSet<GameObject> alreadyHittedTargets = new();
    public HashSet<GameObject> AlreadyHittedTargets => alreadyHittedTargets;

    private float damage;

    public void Initialize(float damage)
    {
        this.damage = damage;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) { return; }

        DamageToTarget(collision.gameObject);
    }

    void DamageToTarget(GameObject target)
    {
        if (alreadyHittedTargets.Contains(target)) { return; }

        // var enemy = target.GetComponent<Enemy>();
        // enemy?.Damage(DAMAGE_PERCENT * damage);

        alreadyHittedTargets.Add(target);
    }

    void DestroyEffect()
    {
        Destroy(gameObject);
    }

}
