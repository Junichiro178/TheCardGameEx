using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject resultPanel;
    [SerializeField] Text resultText;

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
    List<int> playerDeck = new List<int>() { 4, 3, 2, 1 },
              enemyDeck  = new List<int>() { 1, 3, 2, 4 };

    // HPテキストの取得
    [SerializeField] Text playerHeroHpText;
    [SerializeField] Text enemyHeroHpText;

    // HPの数値
    int playerHeroHp;
    int enemyHeroHp;

    // マナテキストの取得
    [SerializeField] Text playerManaCostText;
    [SerializeField] Text enemyManaCostText;

    // マナの数値
    public int playerManaCost;
    public int enemyManaCost;
    public int playerDefaultManaCost;
    public int enemyDefaultManaCost;

    // 時間管理
    [SerializeField] Text timeCountText;
    int timeCount;

    // マナコストの画面表示
    void ShowManaCost()
    {
        playerManaCostText.text = playerManaCost.ToString();
        enemyManaCostText.text = enemyManaCost.ToString();
    }

    void resetTimeCount()
    {
        timeCountText.text = timeCount.ToString();
    }

    // マナコストの消費
    public void ReduceManaCost(int cost, bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            playerManaCost -= cost;
        }
        else
        {
            enemyManaCost -= cost;
        }
        ShowManaCost();
    }

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

    // ゲームスタート
    void StartGame()
    {
        // リザルトパネルの非表示
        resultPanel.SetActive(false);

        // HeroHPの定義
        playerHeroHp = 1;
        enemyHeroHp = 1;
        RefreshHeroHP();

        // マナの定義
        playerManaCost = 1;
        enemyManaCost = 1;
        playerDefaultManaCost = 1;
        enemyDefaultManaCost = 1;

        ShowManaCost();

        InitHand();
        isPlayerTurn = true;
        TurnCalculation();
    }

    // ゲームリスタート
    public void RestartGame()
    {
        // 全てのカードの削除
        foreach (Transform card in playerHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in playerFieldTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyFieldTransform)
        {
            Destroy(card.gameObject);
        }

        // デッキの再取得
        playerDeck = new List<int>() { 4, 3, 2, 1 };
        enemyDeck = new List<int>() { 1, 3, 2, 4 };

        StartGame();

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
        StopAllCoroutines();
        StartCoroutine(CountDown());

        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            EnemyTurn();
        }
    }

    // カウントダウンを行う
    IEnumerator CountDown()
    {
        // 時間定義
        timeCount = 5;
        resetTimeCount();

        while (timeCount > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timeCount--;
            resetTimeCount();
        }
        ChangeTurn();
    }

    // ターンの切り替え
    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        // ドローする
        if (isPlayerTurn)
        {
            playerDefaultManaCost++;
            playerManaCost = playerDefaultManaCost;
            GiveCardsTohand(playerDeck, playerHandTransform);
        }
        else
        {
            enemyDefaultManaCost++;
            enemyManaCost = enemyDefaultManaCost;
            GiveCardsTohand(enemyDeck, enemyHandTransform);
        }
        ShowManaCost();
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

        // フィールドのカードを攻撃可能にする
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        foreach (CardController card in enemyFieldCardList)
        {
            // カードを攻撃可能にする
            card.SetCanAttack(true);
        }

        /*　場にカードを出す */
        // 手札のカードリストを取得
        CardController[] handcardList = enemyHandTransform.GetComponentsInChildren<CardController>();
        // コスト以下のカードを取得
        CardController[] lowerThanManaCardList = Array.FindAll(handcardList, card => card.model.cost <= enemyManaCost);

        // コスト以下の中から...
        if (lowerThanManaCardList.Length > 0)
        {
            // 場に出すカードを選択
            CardController enemyCard = lowerThanManaCardList[0];

            // カードを移動
            enemyCard.movement.SetCardTransform(enemyFieldTransform);

            // マナの消費
            ReduceManaCost(enemyCard.model.cost, false);

            //　フィールドのカードであることを示す
            enemyCard.model.isFieldCard = true;
        }



        /* 攻撃 */
        // フィールドのカードリストを取得する
        CardController[] fieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        // 攻撃可能カードを取得する
        CardController[] canAttackEnemyCardList = Array.FindAll(fieldCardList, card => card.model.canAttack);
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        // 攻撃可能なカード&防御可能カードがない場合は処理を通さない　
        if (canAttackEnemyCardList.Length > 0)
        {　
            // attackerカードを選択
            CardController attacker = canAttackEnemyCardList[0];

            // プレイヤー側にもしカードがあれば処理を続行
            if (playerFieldCardList.Length > 0)
            {
                // defenderカードを選択 
                CardController defender = playerFieldCardList[0];
                // attackerとdefenderを戦わせる
                CardsBattle(attacker, defender);
            }
            else
            {
                AttackToHero(attacker, false); 
            }
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
        CheckHeroHP();
    } 

    // HeroのHPを更新する
    void RefreshHeroHP()
    {
        playerHeroHpText.text = playerHeroHp.ToString();
        enemyHeroHpText.text = enemyHeroHp.ToString();
    }

    void CheckHeroHP()
    {
        if (playerHeroHp <= 0 || enemyHeroHp <= 0)
        {
            resultPanel.SetActive(true);
            if (playerHeroHp <= 0)
            {
                resultText.text = "You Lose";
            }
            else
            {
                resultText.text = "You Win"; 
            }
        }
    }

}
