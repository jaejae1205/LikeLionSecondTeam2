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

    // 피격 관련 변수
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
        if (originalColor.a < 1f)
            originalColor.a = 1f; // 투명한 경우 대비
    }

    void Start()
    {
        Debug.Log($"[Player] Start() 위치: {transform.position} at {Time.time}");
        InGameUIManager.Instance?.UpdateHpUI(currentHp, maxHp); // 체력 UI 초기화
    }

    private void Update()
    {
        Debug.Log("[Player 현재 위치]: " + transform.position);

        if (!isAttacking)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            bool isMoving = moveInput != Vector2.zero;
            animator.SetBool("Move", isMoving);

            if (moveInput.x != 0)
            {
                spriteRenderer.flipX = moveInput.x < 0;
            }
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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx("testSfx");
        }
    }

    private void EndAttack()
    {
        animator.SetBool("Attack", false);
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"[DEBUG] TakeDamage() 호출됨");

        currentHp -= damage;
        Debug.Log($"[Player] {damage} 데미지를 입음. 현재 체력: {currentHp}");

        InGameUIManager.Instance?.UpdateHpUI(currentHp, maxHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("[Player] 사망 처리 로직 호출됨");
        // 사망 연출 추가 가능
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[DEBUG] 충돌 감지됨: {collision.collider.name}");

        if (collision.collider.CompareTag("Enemy") && !isHit)
        {
            Debug.Log("[DEBUG] Enemy 태그 충돌로 데미지 처리 시작");

            TakeDamage(10);

            Vector2 hitDir = (transform.position - collision.transform.position).normalized;
            rb.AddForce(hitDir * knockbackForce, ForceMode2D.Impulse);

            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashWhiteWithAlpha());

            isHit = true;
            Invoke(nameof(ResetHit), 0.5f);
        }
    }

    // ✅ 흰색 + 투명도 점멸 효과
    private IEnumerator FlashWhiteWithAlpha()
    {
        Color fadedWhite = new Color(1f, 1f, 1f, 0.3f); // 투명한 흰색
        spriteRenderer.color = fadedWhite;

        yield return new WaitForSeconds(hitFlashDuration);

        spriteRenderer.color = originalColor;
    }

    private void ResetHit()
    {
        isHit = false;
    }
}