using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    CardView view;// 見かけ(view)に関することを操作
    public CardModel model; // データ(model)に関することを操作
    public CardMovement movement; // 移動(movement)に関することを操作

    private void Awake()
    {
        view = GetComponent<CardView>();
        movement = GetComponent<CardMovement>();
    }

    public void Init(int cardID, bool isPlayer)
    { 
        model = new CardModel(cardID, isPlayer);
        view.Show(model);
    }

    // モンスターを攻撃可能にする処理
    public void SetCanAttack(bool canAttack)
    {
        model.canAttack = canAttack;
        view.DisplaySelectablePanel(canAttack);
    }

    public void Attack(CardController enemyCard)
    {
        model.Attack(enemyCard);
        SetCanAttack(false);
    }

    // フィールドに置かれた時の処理
    public void OnField(bool isPlayer)
    {
        // マナコスト消費
        GameManager.instance.ReduceManaCost(model.cost, isPlayer);

        //　フィールドのカードであることを示す
        model.isFieldCard = true;

        if (model.ability == ABILITY.HASTE)
        {
            SetCanAttack(true);
        }
    }

    // カードの生存チェック
    public void CheckAlive()
    {
        if (model.isAlive)
        {
            view.Refresh(model);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

}
