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

    public void Init(int cardID)
    { 
        model = new CardModel(cardID);
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
