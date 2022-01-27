using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    CardView view;// 見かけ(view)に関することを操作
    CardModel model; // データ(model)に関することを操作

    private void Awake()
    {
        view = GetComponent<CardView>();
    }

    public void Init(int cardID)
    { 
        model = new CardModel(cardID);
        view.Show(model);
    }

}
