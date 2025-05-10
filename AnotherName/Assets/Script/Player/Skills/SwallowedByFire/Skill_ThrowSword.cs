using System.Collections.Generic;
using UnityEngine;

public class Skill_ThrowSword : SkillBase
{
    const float DAMAGE_PERCENT = 2.3f;
    const float BUFF_AMOUNT = 1.03f;
    const float BUFF_DURATION = 3f;

    private int targetCount;

    [SerializeField] private GameObject chainFirePrefab;

    protected override void Activate(Player player)
    {
        Instantiate(chainFirePrefab, player.transform.position, Quaternion.identity);
        chainFirePrefab.GetComponent<SkillSub_ChainFire>().Initialize(DAMAGE_PERCENT * player.CurrentAttackPower);
        
        targetCount = chainFirePrefab.GetComponent<SkillSub_ChainFire>().AlreadyHittedTargets.Count;

        var buff = new StatBuff(StatType.MoveSpeed, BUFF_AMOUNT * targetCount, BUFF_DURATION);
        player.ApplyBuff(buff);
    }

}
