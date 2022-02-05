using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlace : MonoBehaviour, IDropHandler
{
    public enum TYPE
    {
        HAND,
        FIELD,
    }

    public TYPE type;

    public void OnDrop(PointerEventData eventData)
    {
        if (type == TYPE.HAND)
        {
            return;
        }

        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        if (card != null)
        {
            // カードを動かせる状態じゃない場合は処理中断
            if (!card.movement.isDragable)
            {
                return;
            }

            card.movement.defaultParent = this.transform;

            // フィールドに置かれたカードの動きなら処理を止める
            if (card.model.isFieldCard)
            {
                return;
            }

            // マナコスト消費
            GameManager.instance.ReduceManaCost(card.model.cost, true);

            //　フィールドのカードであることを示す
            card.model.isFieldCard = true;
        }
    }
}
