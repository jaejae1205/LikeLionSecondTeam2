using UnityEngine;

public class SkillSub_FireBoom : MonoBehaviour
{
    private float damage;

    public void Initialize(float damage)
    {
        this.damage = damage;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) { return; }

        // collision.GetComponent<Enemy>().Damage(damage)
    }

    void DestroyEffect()
    {
        Destroy(gameObject);
    }

}
