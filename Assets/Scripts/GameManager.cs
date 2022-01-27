using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] Transform playerHandTransform,
                               enemyHandTransform;
    [SerializeField] CardController cardPrefab;

    // プレイヤーのターンかどうか
    bool isPlayerTurn;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        InitHand();
        isPlayerTurn = true;
        TurnCalculation();
    }

    void InitHand()
    {
        // カードをプレイヤーに３枚配る
        for (int i = 0; i < 3; i++)
        {
            CreateCard(playerHandTransform);
            CreateCard(enemyHandTransform);
        }
    }

     void CreateCard(Transform hand)
    {
        CardController card = Instantiate(cardPrefab, hand, false);
        card.Init(2);
    }

    // ターンを計算する
    void TurnCalculation()
    {
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            EnemyTurn();
        }
    }

    // ターンの切り替え
    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        TurnCalculation();
    }

    void PlayerTurn()
    {
        Debug.Log("Playerのターン");
    }

    void EnemyTurn()
    {
        Debug.Log("Enemyのターン");

        ChangeTurn();
    }

}
