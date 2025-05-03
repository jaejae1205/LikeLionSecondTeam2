using UnityEngine;
using UnityEngine.EventSystems; // 추가

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

    private Vector2 moveInput;
    private bool isAttacking = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Debug.Log($"[Player] Start() 위치: {transform.position} at {Time.time}");
    }

    private void Update()
    {
        Debug.Log("[Player 현재 위치]: " + transform.position);

        if (!isAttacking)
        {
            // 입력 처리 (공격 중엔 이동 입력 무시)
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            // 애니메이터 Move 파라미터 설정
            bool isMoving = moveInput != Vector2.zero;
            animator.SetBool("Move", isMoving);

            if (moveInput.x != 0)
            {
                spriteRenderer.flipX = moveInput.x < 0;
            }
        }
        else
        {
            // 공격 중에는 이동 입력 차단
            moveInput = Vector2.zero;
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            // UI 위 클릭 무시
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // NPC 위 클릭 무시
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && hit.CompareTag("NPC"))
                return;

            StartAttack();
        }
    }

    private void FixedUpdate()
    {
        // 이동 처리
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    private void StartAttack()
    {
        isAttacking = true;
        animator.SetBool("Attack", true);

        // 효과음 재생 (예시 key값: "testSfx")
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx("testSfx");
        }
    }

    // Attack 애니메이션 마지막 프레임에서 이 함수를 Animation Event로 호출
    private void EndAttack()
    {
        animator.SetBool("Attack", false);
        isAttacking = false;
    }
}