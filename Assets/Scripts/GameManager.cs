using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // デッキ
    List<int> playerDeck = new List<int>() { 1, 2, 3, 4 },
              enemyDeck  = new List<int>() { 4, 3, 2, 1 };

    [SerializeField] Text playerHeroHpText;
    [SerializeField] Text enemyHeroHpText;

    // HPの数値
    int playerHeroHp;
    int enemyHeroHp;




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
        // HeroHPの定義
        playerHeroHp = 20;
        enemyHeroHp = 20;
        RefreshHeroHP();

        InitHand();
        isPlayerTurn = true;
        TurnCalculation();
    }

    void InitHand()
    {
        // カードをプレイヤーに３枚配る
        for (int i = 0; i < 3; i++)
        {
            GiveCardsTohand(playerDeck, playerHandTransform);
            GiveCardsTohand(enemyDeck, enemyHandTransform);
        }
    }

    // デッキの先頭のカードを認識する
    void GiveCardsTohand(List<int> deck, Transform hand)
    {
        // デッキゼロならドローしない
        if (deck.Count == 0)
        {
            return;
        }

        int cardID = deck[0];
        deck.RemoveAt(0); 
        CreateCard(cardID, hand);
    }

    // IDを基にカードを生成する
     void CreateCard(int cardID, Transform hand)
    {
        CardController card = Instantiate(cardPrefab, hand, false);
        card.Init(cardID); 
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
            GiveCardsTohand(playerDeck, playerHandTransform);
        }
        else
        {
            GiveCardsTohand(enemyDeck, enemyHandTransform);
        }

        TurnCalculation();
    }

    void PlayerTurn()
    {
        Debug.Log("Playerのターン");

        // フィールドのカードを攻撃可能にする
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        foreach (CardController card in playerFieldCardList)
        {
            // カードを攻撃可能にする
            card.SetCanAttack(true);
        }


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
        // 攻撃可能カードを取得する
        CardController[] canAttackEnemyCardList = Array.FindAll(fieldCardList, card => card.model.canAttack);
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        // 攻撃可能なカード&防御可能カードがない場合は処理を通さない　
        if (canAttackEnemyCardList.Length > 0 && playerFieldCardList.Length > 0)
        {　
            // attackerカードを選択
            CardController attacker = canAttackEnemyCardList[0];
            // defenderカードを選択 
            CardController defender = playerFieldCardList[0];
            // attackerとdefenderを戦わせる
            CardsBattle(attacker, defender);
        }

        ChangeTurn();
    }

    // カードの先頭の処理
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

    // Heroへの攻撃
    public void AttackToHero(CardController attacker, bool isPlayerCard)
    {
        if (isPlayerCard) 
        {
            enemyHeroHp -= attacker.model.at;
        } 
        else
        {
            playerHeroHp -= attacker.model.at;
        }
        attacker.SetCanAttack(false);
        RefreshHeroHP();
    } 

    // HeroのHPを更新する
    void RefreshHeroHP()
    {
        playerHeroHpText.text = playerHeroHp.ToString();
        enemyHeroHpText.text = enemyHeroHp.ToString();
    }

}
