using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlace : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        if (card != null)
        {
            card.movement.defaultParent = this.transform;


            // マナコスト消費
            GameManager.instance.ReduceManaCost(card.model.cost, true);
        }
    }
}
