using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHp = 30f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 10f;

    private float currentHp;
    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D enemyCollider;  // 전체 적의 콜라이더 추가

    private bool isDead = false;
    private bool hasDealtDamageThisAttack = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<BoxCollider2D>(); // 전체 콜라이더 참조
        currentHp = maxHp;
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", true);

            if (!hasDealtDamageThisAttack && IsInAnimation("Attack"))
            {
                TryDealDamage();
            }
        }
        else if (distanceToPlayer <= detectionRange)
        {
            anim.SetBool("isAttacking", false);
            anim.SetBool("isWalking", true);
            MoveTowardsPlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", false);
        }

        if (!IsInAnimation("Attack"))
        {
            hasDealtDamageThisAttack = false;
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;

        // Flip based on direction
        if ((dir.x > 0 && transform.localScale.x < 0) || (dir.x < 0 && transform.localScale.x > 0))
        {
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private bool IsInAnimation(string name)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    private void TryDealDamage()
    {
        // 공격 중일 때 적의 콜라이더 범위 내에 플레이어가 있는지 확인
        if (enemyCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.Damage(attackDamage);  // 플레이어에게 데미지를 입힘
                hasDealtDamageThisAttack = true;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("isHit");
        }
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isDead", true);
        anim.SetBool("isWalking", false);
        anim.SetBool("isAttacking", false);

        Destroy(gameObject, 2f);
    }

    public void OnHitAnimationEnd()
    {
        anim.ResetTrigger("isHit");
    }

    private void OnDrawGizmosSelected()
    {
        if (enemyCollider == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(enemyCollider.bounds.center, enemyCollider.bounds.size);  // 적의 콜라이더 범위 시각화
    }
}
