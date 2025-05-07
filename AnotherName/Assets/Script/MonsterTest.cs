using UnityEngine;

public class MonsterTest : MonoBehaviour
{
    public int maxHp = 100;
    public int attackPower = 2; // 💡 인스펙터에서 설정 가능
    private int currentHp;

    private void Start()
    {
        currentHp = maxHp;
    }

    // ✅ 플레이어 데미지 처리 제거 → 충돌 판정은 TestPlayer에서만 처리함
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 이전에는 여기서 player.TakeDamage(attackPower)를 호출했을 수 있음
        // 이제는 완전히 제거하여 중복 호출 방지
    }

    // 필요 시 추후 Monster의 체력 감소, 사망 처리 등 추가 가능
}