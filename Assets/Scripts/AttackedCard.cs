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
        // attackerとdefenderを戦わせる
        GameManager.instance.CardsBattle(attacker, defender); 
    }
}
