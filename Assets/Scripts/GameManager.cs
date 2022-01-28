using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] Transform playerHandTransform,
                               enemyHandTransform,
                               enemyFieldTransform,
                               playerFieldTransform;
    [SerializeField] CardController cardPrefab;

    // シングルトン化（どこからでもアクセスできるようにする）
    public static GameManager instance;

    // プレイヤーのターンかどうか
    bool isPlayerTurn;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

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
        card.Init(3);
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

        // ドローする
        if (isPlayerTurn)
        {
            CreateCard(playerHandTransform);
        }
        else
        {
            CreateCard(enemyHandTransform);
        }

        TurnCalculation();
    }

    void PlayerTurn()
    {
        Debug.Log("Playerのターン");

    }

    void EnemyTurn()
    {
        Debug.Log("Enemyのターン");

        /*　場にカードを出す */
        // 手札のカードリストを取得
        CardController[] handcardList = enemyHandTransform.GetComponentsInChildren<CardController>();
        // 場に出すカードを選択
        CardController enemyCard = handcardList[0];
        // カードを移動
        enemyCard.movement.SetCardTransform(enemyFieldTransform);

        /* 攻撃 */
        // フィールドのカードリストを取得する
        CardController[] fieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        // attackerカードを選択
        CardController attacker = fieldCardList[0];
        // defenderカードを選択
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        CardController defender = playerFieldCardList[0];
        // attackerとdefenderを戦わせる
        CardsBattle(attacker, defender);


        ChangeTurn();
    }

    public void CardsBattle(CardController attacker, CardController defender)
    {
        Debug.Log("CardsBattle");
        Debug.Log("attacker HP:"+ attacker.model.hp);
        Debug.Log("defender HP:" + defender.model.hp);
        attacker.Attack(defender);
        defender.Attack(attacker);
        Debug.Log("attacker HP:" + attacker.model.hp);
        Debug.Log("defender HP:" + defender.model.hp);
        attacker.CheckAlive();
        defender.CheckAlive();
    }

}
