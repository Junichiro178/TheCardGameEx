using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    // 見かけ(view)に関することを操作

    CardModel model; // データ(model)に関することを操作

    public void Init(int cardID)
    { 
        model = new CardModel(cardID); 
    }

}
