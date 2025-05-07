using UnityEngine;

public class MonsterTest : MonoBehaviour
{
    public int maxHp = 100;
    public int attackPower = 10;
    private int currentHp;

    private void Start()
    {
        currentHp = maxHp;
    }

    // ✅ Trigger → Collision으로 변경
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            TestPlayer player = collision.collider.GetComponent<TestPlayer>();
            if (player != null)
            {
                player.TakeDamage(attackPower);
                Debug.Log($"[MonsterTest] 플레이어 충돌, 데미지: {attackPower}");
            }
        }
    }
}