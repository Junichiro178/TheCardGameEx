using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 攻撃される側の処理
public class AttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        /* 攻撃 */
        // attackerカードを選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        // defenderカードを選択
        CardController defender = GetComponent<CardController>();

        // attackerもしくはdefenderが取得できない場合攻撃を行わない
        if (attacker == null || defender == null)
        {
            return;
        }
        // 同じプレイヤーのカード同士ならバトルしない
        if (attacker.model.isPlayerCard == defender.model.isPlayerCard)
        {
            return;
        }

        // canAttackフラグが立っており、攻撃可能な場合のみ攻撃する
        if (attacker.model.canAttack)
        {
            // attackerとdefenderを戦わせる
            GameManager.instance.CardsBattle(attacker, defender);
        }
    }
}
