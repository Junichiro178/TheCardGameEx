using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 手札にカードを生成する

    [SerializeField] Transform playerHandTransform;   
    [SerializeField] CardController cardPrefab;
    void Start()
    {
        CreateCard(playerHandTransform);
    }

     void CreateCard(Transform hand)
    {
        CardController card = Instantiate(cardPrefab, hand, false);
        card.Init(1);
    }

}
