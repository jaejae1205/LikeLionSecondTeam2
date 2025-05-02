using UnityEngine;
using TMPro;

public class NPCController : MonoBehaviour
{
    [Header("이동 속도")]
    public float moveSpeed = 2f;

    [Header("플레이어와 멈추는 거리")]
    [SerializeField] private float stopDistance = 1.5f;

    [Header("사운드")]
    public AudioClip signSound;       // 사인 표시 효과음
    public AudioClip dialogueSound;   // 대사 넘길 때 효과음
    public AudioClip walkSound;       // 걷기 효과음 (루프)

    private AudioSource audioSource;  // 오디오 재생기

    private GameObject signObject;
    private Transform player;
    private Animator animator;

    private bool hasMovedToStartPosition = false;
    private bool signVisible = true;
    private bool dialogueEnded = false;

    public GameObject talkPanel;
    public TextMeshProUGUI text;

    private int clickCount = 0;

    private float walkSoundTimer = 0f; // 걷기 소리 간격 타이머

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsWalk", true);
        }

        foreach (Transform child in transform)
        {
            if (child.CompareTag("Sign"))
            {
                signObject = child.gameObject;
                signObject.SetActive(true);
                break;
            }
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 초기 상태에서 대화창 비활성화
        talkPanel.SetActive(false);
    }

    void Update()
    {
        if (!hasMovedToStartPosition)
        {
            float randX = Random.Range(0f, 8.4f);
            float randY = Random.Range(-4.3f, 4.3f);
            transform.position = new Vector3(randX, randY, 0f);
            hasMovedToStartPosition = true;
        }

        if (player != null && !dialogueEnded)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer > stopDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
                if (animator != null) animator.SetBool("IsWalk", true);

                // 걷기 사운드 재생 (속도 조정)
                if (walkSound != null && audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.clip = walkSound;
                    audioSource.loop = false; // 루프 모드 비활성화
                    audioSource.pitch = 1f; // 음의 높낮이는 그대로
                }

                // 1초마다 걷기 소리 재생
                walkSoundTimer += Time.deltaTime;
                if (walkSoundTimer >= 0.5f) // 1초가 지났다면
                {
                    walkSoundTimer = 0f; // 타이머 초기화
                    audioSource.PlayOneShot(walkSound);  // 소리 재생
                }

                if (signObject != null && signVisible && signObject.activeSelf)
                {
                    signObject.SetActive(false);
                }
            }
            else
            {
                if (animator != null) animator.SetBool("IsWalk", false);

                // 걷기 사운드 정지
                if (audioSource != null && audioSource.clip == walkSound && audioSource.isPlaying)
                {
                    audioSource.Stop();
                    audioSource.loop = false;
                    audioSource.clip = null;
                }

                if (signObject != null && signVisible && !signObject.activeSelf)
                {
                    signObject.SetActive(true);

                    if (signSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(signSound);
                    }
                }
            }

            if (transform.position.x < player.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }

        // 스페이스바를 누르면 대화 진행
        if (Input.GetKeyDown(KeyCode.Space) && !dialogueEnded && talkPanel.activeSelf)
        {
            AdvanceDialogue();
        }
    }

    // NPC 클릭 시 대화창 활성화
    void OnMouseDown()
    {
        if (dialogueEnded) return;

        // 이미 대화 중이면 클릭 무시
        if (talkPanel.activeSelf) return;

        if (signObject != null)
        {
            signObject.SetActive(false);
            signVisible = false;
        }

        if (animator != null)
        {
            animator.SetBool("IsWalk", false);
        }

        // 걷기 사운드 정지 (대화 중이므로)
        if (audioSource != null && audioSource.clip == walkSound && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null;
        }

        // 대화창 활성화 및 첫 대사 출력
        talkPanel.SetActive(true);
        AdvanceDialogue();
    }

    private void AdvanceDialogue()
    {
        if (dialogueEnded) return;

        if (dialogueSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(dialogueSound);
        }

        switch (clickCount)
        {
            case 0:
                text.text = "먼 길 오시느라 고생 많으셨습니다.\n저는 이 마을의 관리인 카이렐입니다.";
                break;
            case 1:
                text.text = "이곳은 신의 축복 아래 살아가는 조용한 마을이죠.\n하지만 요즘 마을에 이상한 일이 일어나고 있어요.";
                break;
            case 2:
                text.text = "주민들이 하나둘씩 사라지고 있습니다.\n마지막으로 목격된 곳은 마을 동쪽 외곽이었어요.";
                break;
            case 3:
                text.text = "괜찮다면 조사를 부탁드려도 될까요?";
                text.rectTransform.anchoredPosition = new Vector2(text.rectTransform.anchoredPosition.x, 0f);
                break;
            case 4:
                talkPanel.SetActive(false);
                dialogueEnded = true;
                gameObject.SetActive(false);
                return;
        }

        clickCount++;
    }
}
