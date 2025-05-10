using System;
using System.Collections;
using UnityEngine;

public class Skill_Swallow : SkillBase
{
    const float DAMAGE_PERCENT = 0.05f;
    const float BUFF_AMOUNT = 1.5f;
    const float BUFF_DURATION = 5f;
    const float MUJEOK_TIME = 2f;
    [SerializeField] private GameObject fireBoomPrefab;
    
    protected override void Activate(Player player)
    {
        Instantiate(fireBoomPrefab, player.transform.position, Quaternion.identity);
        fireBoomPrefab.GetComponent<SkillSub_ChainFire>().Initialize(DAMAGE_PERCENT * player.CurrentAttackPower);

        foreach (StatType stat in Enum.GetValues(typeof(StatType)))
        {
            var buff = new StatBuff(stat, BUFF_AMOUNT, BUFF_DURATION);
            player.ApplyBuff(buff);
        }
    }

    IEnumerator Mujeok(Player player)
    {
        player.isMujeok = true;
        yield return new WaitForSeconds(MUJEOK_TIME);
        player.isMujeok = false;
    }

}
