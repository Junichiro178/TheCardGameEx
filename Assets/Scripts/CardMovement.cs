using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;


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
        if (!card.model.isFieldCard && card.model.cost <= GameManager.instance.playerManaCost )
        {
            isDragable = true;
        }
        else if (card.model.isFieldCard && card.model.canAttack)
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

    //　カードアニメーション
    public IEnumerator MoveToField(Transform field)
    {
        //一度親をCanvasに変更する
        transform.SetParent(defaultParent.parent);
        //DOTweenでカードを移動する
        transform.DOMove(field.position, 0.25f);
        yield return new WaitForSeconds(0.25f);
        defaultParent = field;
        transform.SetParent(defaultParent);
    }

    public IEnumerator MoveToTarget(Transform target)
    {
        // 現在の位置と場所を記憶する
        Vector3 currentPosition = transform.position;
        int siblingIndex = transform.GetSiblingIndex();

        //一度親をCanvasに変更する
        transform.SetParent(defaultParent.parent);
        //DOTweenでカードを攻撃対象に移動する
        transform.DOMove(target.position, 0.25f);

        yield return new WaitForSeconds(0.25f);

        // 元の位置に戻る
        transform.DOMove(currentPosition, 0.25f);
        yield return new WaitForSeconds(0.25f);
        transform.SetParent(defaultParent);
        transform.SetSiblingIndex(siblingIndex);
    }

    void Start()
    {
        defaultParent = transform.parent;
    }
}
