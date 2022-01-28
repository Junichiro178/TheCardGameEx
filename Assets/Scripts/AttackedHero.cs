using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 攻撃される側の処理
public class AttackedHero : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        /* 攻撃 */
        // attackerカードを選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();

        // attackerもしくはdefenderが取得できない場合攻撃を行わない
        if (attacker == null)
        {
            return;
        }

        // canAttackフラグが立っており、攻撃可能な場合のみ攻撃する
        if (attacker.model.canAttack)
        {
            //attackerがHeroに攻撃する
            GameManager.instance.AttackToHero(attacker, true); 
        }
    }
}
