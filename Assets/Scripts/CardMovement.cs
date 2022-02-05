using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// カードの動きを司る
public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform defaultParent;

    // ドラッグアンドドロップ可能かどうかのフラグ
    public bool isDragable;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // カードのコストとプレイヤーのマナコストを比較
        CardController card = GetComponent<CardController>();
        if (card.model.cost <= GameManager.instance.playerManaCost )
        {
            isDragable = true;
        }
        else
        {
            isDragable = false;
        }

        // プレイヤーのマナコストよりも高い場合処理終了
        if (!isDragable)
        {
            return;
        }

        defaultParent = transform.parent;
        transform.SetParent(defaultParent.parent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // プレイヤーのマナコストよりも高い場合処理終了
        if (!isDragable)
        {
            return;
        }

        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        // プレイヤーのマナコストよりも高い場合処理終了
        if (!isDragable)
        {
            return;
        }

        transform.SetParent(defaultParent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void SetCardTransform(Transform parentTransform)
    {
        defaultParent = parentTransform;
        transform.SetParent(defaultParent);
    }
}
