using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State { Idle, Patrol, Move, Attack, Damage, Death }
    private State currentState = State.Idle;

    [Header("공통 설정")]
    public float moveSpeed = 2f;
    public int maxHp = 100;
    private int currentHp;

    [Header("공격 설정")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackDelay = 1f;
    private float attackCooldown = 0f;

    [Header("추적 거리 설정")]
    public float maxChaseDistance = 8f;

    [Header("순찰 설정")]
    public float patrolRadius = 3f;
    public float idleDuration = 2f;
    private Vector2 patrolTarget;
    private float idleTimer = 0f;

    private Transform target;
    private Vector2 initialPosition;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHp = maxHp;
        initialPosition = transform.position;
        SetNewPatrolTarget();
    }

    private void Update()
    {
        if (currentState == State.Death) return;

        attackCooldown -= Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleDuration)
                {
                    ChangeState(State.Patrol);
                }
                LookForTarget();
                break;

            case State.Patrol:
                PatrolMove();
                LookForTarget();
                break;

            case State.Move:
                MoveToTarget();
                break;

            case State.Attack:
                TryAttack();
                break;

            case State.Damage:
                break;
        }
    }

    private void LookForTarget()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        if (target != null && Vector2.Distance(transform.position, target.position) < detectionRange)
        {
            ChangeState(State.Move);
        }
    }

    private void PatrolMove()
    {
        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        if (direction.x != 0)
            spriteRenderer.flipX = direction.x < 0;

        animator.SetBool("IsMoving", true);

        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            ChangeState(State.Idle);
            idleTimer = 0f;
            SetNewPatrolTarget();
        }
    }

    private void SetNewPatrolTarget()
    {
        float offsetX = Random.Range(-patrolRadius, patrolRadius);
        float offsetY = Random.Range(-patrolRadius, patrolRadius);
        patrolTarget = initialPosition + new Vector2(offsetX, offsetY);
    }

    private void MoveToTarget()
    {
        if (target == null)
        {
            ChangeState(State.Idle);
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > maxChaseDistance)
        {
            target = null;
            ChangeState(State.Idle);
            return;
        }

        if (distance <= attackRange)
        {
            ChangeState(State.Attack);
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        if (direction.x != 0)
            spriteRenderer.flipX = direction.x < 0;

        animator.SetBool("IsMoving", true);
    }

    private void TryAttack()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsMoving", false);

        if (attackCooldown <= 0f)
        {
            animator.SetTrigger("Attack");
            attackCooldown = attackDelay;
        }

        if (target == null || Vector2.Distance(transform.position, target.position) > attackRange)
        {
            ChangeState(State.Move);
        }
    }

    public void OnHit(int damage)
    {
        if (currentState == State.Death) return;

        currentHp -= damage;
        if (currentHp <= 0)
        {
            ChangeState(State.Death);
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            animator.SetTrigger("Death");
        }
        else
        {
            ChangeState(State.Damage);
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            animator.SetTrigger("Hit");
        }
    }

    public void EndDamage()
    {
        ChangeState(State.Idle);
    }

    public void EndDeath()
    {
        Destroy(gameObject);
    }

    private void ChangeState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        // 상태에 따라 이동 멈추고 Idle 상태로
        if (newState == State.Idle || newState == State.Attack || newState == State.Damage || newState == State.Death)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
        }
    }
}