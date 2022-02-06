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
    List<int> playerDeck = new List<int>() { 6, 5, 2, 1, 3, 4 },
              enemyDeck  = new List<int>() { 6, 5, 2, 4, 3, 1 };

    // HPテキストの取得
    [SerializeField] Text playerHeroHpText;
    [SerializeField] Text enemyHeroHpText;

    // HPの数値
    int playerHeroHp;
    int enemyHeroHp;

    [SerializeField] Transform playerHero;

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
        playerHeroHp = 4;
        enemyHeroHp = 4;
        RefreshHeroHP();

        // マナの定義
        playerManaCost = playerDefaultManaCost = 10;
        enemyManaCost = enemyDefaultManaCost = 10;
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
        playerDeck = new List<int>() { 6, 5, 2, 1, 3, 4 };
        enemyDeck = new List<int>() { 6, 5, 2, 4, 3, 1 };

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
        if (hand.name == "PlayerHand")
        {
            card.Init(cardID, true);
        }
        else
        {
            card.Init(cardID, false);
        }
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
            StartCoroutine(EnemyTurn());
            
        }
    }

    // カウントダウンを行う
    IEnumerator CountDown()
    {
        // 時間定義
        timeCount = 10;
        resetTimeCount();

        while (timeCount > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timeCount--;
            resetTimeCount();
        }
        ChangeTurn();
    }

    // 敵フィールドのカードを取得する関数
    public CardController[] GetEnemyFieldCards()
    {
        return enemyFieldTransform.GetComponentsInChildren<CardController>();
    }

    // ターンの切り替え
    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        // 全てのカードを攻撃可能フラグを取り除く（ターンが切り替わったため）
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, false);
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(enemyFieldCardList, false);

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

    void SettingCanAttackView(CardController[] fieldCardList, bool canAttack)
    {
        foreach (CardController card in fieldCardList)
        {
            // カードを攻撃可能にする
            card.SetCanAttack(canAttack);
        }
    }

    void PlayerTurn()
    {
        Debug.Log("Playerのターン");

        // フィールドのカードを攻撃可能にする
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, true);


    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemyのターン");

        // フィールドのカードを攻撃可能にする
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(enemyFieldCardList, true);

        yield return new WaitForSeconds(1.0f);

        /*　場にカードを出す */
        // 手札のカードリストを取得
        CardController[] handcardList = enemyHandTransform.GetComponentsInChildren<CardController>();

        // マナコスト以下のカードが有れば、場に出し続ける
        while (Array.Exists(handcardList, card => card.model.cost <= enemyManaCost))
        {
            // コスト以下のカードを取得
            CardController[] lowerThanManaCardList = Array.FindAll(handcardList, card => card.model.cost <= enemyManaCost);

            // 場に出すカードを選択
            CardController enemyCard = lowerThanManaCardList[0];

            // カードを移動
            StartCoroutine(enemyCard.movement.MoveToField(enemyFieldTransform)) ;

            // 敵カードのアビリティ処理
            enemyCard.OnField(false);

            // カードリストを更新する
            handcardList = enemyHandTransform.GetComponentsInChildren<CardController>();
            yield return new WaitForSeconds(1.0f);
        }

        yield return new WaitForSeconds(1.0f);

        /* 攻撃 */
        // フィールドのカードリストを取得する
        CardController[] fieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        // 攻撃可能カードがあれば、攻撃を繰り返す
        while (Array.Exists(fieldCardList, card => card.model.canAttack))
        {
            // 攻撃可能カードを取得する
            CardController[] canAttackEnemyCardList = Array.FindAll(fieldCardList, card => card.model.canAttack);
            CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();

            // attackerカードを選択
            CardController attacker = canAttackEnemyCardList[0];

            // プレイヤー側にもしカードがあれば処理を続行
            if (playerFieldCardList.Length > 0)
            {
                // defenderカードを選択
                // Shieldカードがあればその中から攻撃対象を選ぶ
                if (Array.Exists(playerFieldCardList, card => card.model.ability == ABILITY.SHIELD))
                {
                    playerFieldCardList = Array.FindAll(playerFieldCardList, card => card.model.ability == ABILITY.SHIELD);
                }
                CardController defender = playerFieldCardList[0];
                // attackerとdefenderを戦わせる
                StartCoroutine(attacker.movement.MoveToTarget(defender.transform));
                yield return new WaitForSeconds(0.51f);
                CardsBattle(attacker, defender);
            }
            else
            {
                StartCoroutine(attacker.movement.MoveToTarget(playerHero.transform));
                yield return new WaitForSeconds(0.25f);
                AttackToHero(attacker, false);
                yield return new WaitForSeconds(0.25f);
                CheckHeroHP();
            }

            // フィールドカードの更新
            fieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();

            yield return new WaitForSeconds(1.0f);
        }

        yield return new WaitForSeconds(1.0f);

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

    public void CheckHeroHP()
    {
        if (playerHeroHp <= 0 || enemyHeroHp <= 0)
        {
            ShowResultPanel(playerHeroHp);
        }
    }

    // 結果画面の表示
    void ShowResultPanel(int heroHP)
    {
        StopAllCoroutines();
        resultPanel.SetActive(true);
        if (heroHP <= 0)
        {
            resultText.text = "You Lose";
        }
        else
        {
            resultText.text = "You Win";
        }
    }

}
