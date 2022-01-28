using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// カードデータそのもの、とその処理
public class CardModel
{
    public string name;
    public int hp;
    public int at;
    public int cost;
    public Sprite icon;
    public bool isAlive = true;

    public CardModel(int cardID)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardEntityList/Card"+cardID);
        name = cardEntity.name;
        hp = cardEntity.hp;
        at = cardEntity.at;
        cost = cardEntity.cost;
        icon = cardEntity.icon;
    }

    void Damage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            hp = 0;
            isAlive = false;
        }
    }

    public void Attack(CardController card)
    {
        card.model.Damage(at);
    }
}
