using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class TestPlayer : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [Header("이동 속도")]
    public float moveSpeed = 5f;

    [Header("플레이어 체력")]
    public int maxHp = 100;
    private int currentHp;

    private Vector2 moveInput;
    private bool isAttacking = false;

    private bool isHit = false;
    public float knockbackForce = 5f;
    public float hitFlashDuration = 0.1f;
    private Coroutine flashCoroutine;
    private Color originalColor;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHp = maxHp;
        originalColor = spriteRenderer.color;
        if (originalColor.a < 1f) originalColor.a = 1f;
    }

    private void Start()
    {
        StartCoroutine(InitializeHpUI());
    }

    private IEnumerator InitializeHpUI()
    {
        yield return new WaitUntil(() =>
            InGameUIManager.Instance != null &&
            InGameUIManager.Instance.hpText != null &&
            InGameUIManager.Instance.hpBarFillImage != null
        );

        InGameUIManager.Instance.UpdateHpUI(currentHp, maxHp);
    }

    private void Update()
    {
        if (!isAttacking)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            bool isMoving = moveInput != Vector2.zero;
            animator.SetBool("Move", isMoving);

            if (moveInput.x != 0)
                spriteRenderer.flipX = moveInput.x < 0;
        }
        else
        {
            moveInput = Vector2.zero;
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && hit.CompareTag("NPC"))
                return;

            StartAttack();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    private void StartAttack()
    {
        isAttacking = true;
        animator.SetBool("Attack", true);
        AudioManager.Instance?.PlaySfx("testSfx");
    }

    private void EndAttack()
    {
        animator.SetBool("Attack", false);
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"[TakeDamage] 받은 데미지: {damage}, 이전 체력: {currentHp}, 최대 체력: {maxHp}");

        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        Debug.Log($"[TakeDamage] 현재 체력: {currentHp}");
        InGameUIManager.Instance?.UpdateHpUI(currentHp, maxHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("[Player] 사망 처리 로직 호출됨");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHit) return;

        if (collision.collider.CompareTag("Enemy"))
        {
            isHit = true;

            MonsterTest monster = collision.collider.GetComponent<MonsterTest>();
            int damage = monster != null ? monster.attackPower : 1;

            TakeDamage(damage);

            Vector2 hitDir = (transform.position - collision.transform.position).normalized;
            rb.AddForce(hitDir * knockbackForce, ForceMode2D.Impulse);

            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashWhiteWithAlpha());

            Invoke(nameof(ResetHit), 0.5f);
        }
    }

    private IEnumerator FlashWhiteWithAlpha()
    {
        Color fadedWhite = new Color(1f, 1f, 1f, 0.3f);
        spriteRenderer.color = fadedWhite;

        yield return new WaitForSeconds(hitFlashDuration);

        spriteRenderer.color = originalColor;
    }

    private void ResetHit()
    {
        isHit = false;
    }

    // UIManager에서 접근할 수 있게 Getter 제공 (optional)
    public int GetCurrentHp() => currentHp;
    public int GetMaxHp() => maxHp;
}