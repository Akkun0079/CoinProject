using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace  FortuneWheel
{
    public class GameController : MonoBehaviour
    {
        private static GameController _ins; // GameControllerクラスのインスタンスを格納する静的変数
        public static GameController ins
        {
            get
            {
                return _ins; // インスタンスを取得するプロパティ
            } // インスタンスにアクセスするためのプロパティ
        }  // 以下、リワードのカテゴリーを示す列挙型とその他の変数やクラスの定義が続きます

        public enum RewardEnum                  // Your Custom Reward Categories
        {
            None,
            Gold,
            Life,
            Energy,
            Gem1,
            Gem2,
            Gem3,
            Gem4,
            Money
        };
        // リワードのカテゴリーを表す列挙型です。ゲーム内のカスタムリワードの種類を定義します。

        [System.Serializable]
        public class PieceFeatures
        {
            public RewardEnum rewardCategory; // リワードの種類を示す列挙型
            public Color backgroundColor; // ピースの背景色
            public Sprite backgroundSprite; // ピースの背景スプライト
            [HideInInspector]
            public Sprite rewardIcon; // リワードのアイコンスプライト（非表示）
            public int rewardAmount; // リワードの数量
            public int rewardChance;   // このスロットがリワードの結果として選択される確率。このプロパティがすべてのスロットで100の場合、各リワードが8つのセクターのルーレットホイールで12.5％の確率で選択される可能性があります。
            public Texture confetiIcon; // コンフェティエフェクトのアイコン
        }
        // ルーレットホイールの各ピースの特徴を表すクラスです。リワードの種類、背景色、背景スプライト、リワードのアイコン、数量、確率などを定義します。

        [Header("Rewards Custom Settings")]
        [Space]
        public PieceFeatures[] PiecesOfWheel;
        // ルーレットホイールのピースごとの特徴を格納する配列です。各ピースのリワード情報や背景などがここに設定されます。


        [Header("Game Inputs")]
        [Space]
        public GameObject wheelParent; // ルーレットホイールの親オブジェクト
        public GameObject piecePrefab; // ルーレットホイールの1ピースを示すプレハブ
        public Sprite[] CustomBackgrounds; // カスタム背景スプライトの配列
        [System.Serializable]
        public class CategoryIcons
        {
            public RewardEnum category; // カテゴリーアイコンが関連付けられるリワードのカテゴリー
            public Sprite rewardIcon; // カテゴリーアイコンスプライト
        }
        public CategoryIcons[] categoryIcons;
        // リワードカテゴリーごとに関連付けられるアイコン情報を格納する配列です。
        
        public int turnCost; // ホイールを回すのに必要なコインの数
        public bool controlResultReward;  // リワード結果を制御するかどうかのフラグ
        public bool useCustomBackgrounds; // true の場合、パッケージのカスタムデザインを使用。false の場合、PiecesOfWheel 配列内のカラー選択を使用。

        [Header("Reward Popup Panel Inputs")]
        [Space]
        public Transform popupPanel;
        // リワードポップアップパネルの Transform コンポーネント。ポップアップ表示に関する設定がここに関連付けられます。

        [Header("UI Elements")]
　　　　　[Space]
　　　　　public Button turnButton; // ルーレットホイールを回すためのボタン
　　　　　public Image rewardImageHeader; // ヘッダー部分に表示されるリワードアイコンの Image コンポーネント
　　　　　public Image rewardImagePopup; // ポップアップパネルに表示されるリワードアイコンの Image コンポーネント
　　　　　public Text rewardTextHeader; // ヘッダー部分に表示されるリワードテキストの Text コンポーネント
　　　　　public Text rewardTextPopup; // ポップアップパネルに表示されるリワードテキストの Text コンポーネント
　　　　　public Text turnCostText; // ホイールを回すのに必要なコイン数を表示する Text コンポーネント
　　　　　public Text totalGoldText, totalEnergyText, totalLifeText, totalGem1Text, totalGem2Text, totalGem3Text, totalGem4Text, totalMoneyText;
　　　　　// ポップアップで表示されるコインの総量やリワードのテキストを表示する Text コンポーネント

　　　　　[Header("Effect Settings")]
　　　　　[Space]
　　　　　public ParticleSystem confetiEffect; // リワード獲得時のパーティクルエフェクト
　　　　　public Material confetiCurrency; // パーティクルエフェクトのマテリアル

　　　　　private float[] sectorsAngles = new float[] { -90, -135, -180, -225, -270, -315, -360, -45 }; // ルーレットホイールの各セクターの角度の配列（8つのセクターの場合）
　　　　　private float startAngle = 0; // ルーレットホイールの回転を開始する角度
　　　　　private float finalAngle; // ルーレットホイールの回転が停止する最終角度
　　　　　private float currentLerpRotationTime; // 現在の補完回転時間
　　　　　private float maxLerpRotationTime = 5f; // 最大補完回転時間

　　　　　private int totalGold, totalEnergy, totalLife, totalGem1, totalGem2, totalGem3, totalGem4, totalMoney; // 各種コインやリワードの総量
　　　　　private int previousGold, previousEnergy, previousLife, previousGem1, previousGem2, previousGem3, previousGem4, previousMoney; // アニメーション用に前のコイン総量を保持
　　　　　private int rewardIndex; // 選択されたリワードのインデックス
　　　　　private int randomChange; // 異なるイージング関数を使用して回転の多様性を作成するための値
　　　　　private int rewardMultiplier = 1; // リワードの乗数（デフォルトは1）。プレイヤーがx2ボタンを選択した場合、報酬を倍増させるために使用される。
　　　　　private int selectedRewardIndex; // 選択されたリワードのインデックス

　　　　　private bool isStarted; // ルーレット回転が開始されているかどうかのフラグ
　　　　　private bool firstSpin = true; // 最初のルーレット回転かどうかのフラグ
　　　　　private bool rewardSelected; // リワードが選択されたかどうかのフラグ
    　　 private string goldString = "totalGold"; // プレイヤーの総所持金を保存するためのキー名
　　　　　private string energyString = "totalEnergy"; // プレイヤーのエネルギー総量を保存するためのキー名
　　　　　private string lifeString = "totalLife"; // プレイヤーのライフ総量を保存するためのキー名
　　　　　private string moneyString = "totalMoney"; // プレイヤーのゲーム内通貨総量を保存するためのキー名
　　　　　private string gem1String = "totalGem1"; // プレイヤーのジェム1の総量を保存するためのキー名
　　　　　private string gem2String = "totalGem2"; // プレイヤーのジェム2の総量を保存するためのキー名
　　　　　private string gem3String = "totalGem3"; // プレイヤーのジェム3の総量を保存するためのキー名
　　　　　private string gem4String = "totalGem4"; // プレイヤーのジェム4の総量を保存するためのキー名


　　　　　private void Awake()
　　　　　{
    　　　　　if (_ins == null)
     　　　   _ins = this;

   　　　　　 // インスタンスの初期化
  　　　　　  // シングルトンパターンによって、GameManagerのインスタンスを生成または取得します
  　　　　　  // これにより、ゲーム内のあらゆる場所からGameManagerのインスタンスにアクセスできます

   　　　　　 // UI要素の初期化
　　　　　　　 previousGold = totalGold; // 前回の所持金の値を保持し、UIアニメーション用に使用します
   　　　　　 totalGoldText.text = totalGold.ToString(); // UIテキスト要素に現在の所持金を表示します
   　　　　　 turnCostText.text = turnCost.ToString(); // ターンコストをUIテキスト要素に表示します

  　　　　　  GetPlayerProgress(); // プレイヤーの進行状況を読み込みます
   　　　　　 CreateWheel(); // ルーレットホイールの生成処理を呼び出します
　　　　　}


      　　 public void CreateWheel()
　　　　　{
  　　　　　　float startingAngle = 0; // ルーレットの開始角度

    　　　　　// ルーレットの各パーツを作成
   　　　　　 for (int i = 0; i < PiecesOfWheel.Length; i++)
   　　　　　 {
    　　　    // パーツのゲームオブジェクトを生成
    　　　    GameObject pieceObj = Instantiate(piecePrefab, Vector3.zero, new Quaternion(0, 0, 0, 0), wheelParent.transform);

     　　　   // パーツの名前を設定
    　　　    pieceObj.transform.name = "Piece " + (i + 1);

    　　　    // パーツのローカル位置を設定
   　　　     pieceObj.transform.localPosition = new Vector3(0, 0, 0);

    　　　    // パーツを回転させて正しい位置に配置
    　　　    pieceObj.transform.Rotate(0, 0, Mathf.Abs(startingAngle), 0);

    　　　    // パーツの値を設定するためのコンポーネントに値を送信
    　　　    pieceObj.transform.GetComponent<PieceObject>().SetValues(i);

   　　　     // 次のパーツの角度を計算
   　　　     startingAngle += 45;
   　　　　　 }
　　　　　}


        public void TurnWheel()
        {
            turnButton.interactable = false;// ターン中にボタンを無効化

            if (totalGold >= turnCost) // プレイヤーの所持金がターンコスト以上ある場合
            {
                ClaimTurnCost();// ターンコストを消費

                randomChange = Random.Range(0, 3);// 乱数を生成
                currentLerpRotationTime = 0f;// 回転の補間時間をリセット

                #region Wheel Header 
                //Only at first spin you need to fill rewardImage to avoid showing blank image
                // 最初のスピンのみ、報酬イメージを初期化
                if (firstSpin)
                {
                    rewardImageHeader.sprite = PiecesOfWheel[6].rewardIcon; // Stopperは配列の7番目から始まります。その特徴は配列の6番目に保存されています。別のホイールを使用する場合はここを変更してください。
                    rewardTextHeader.text = PiecesOfWheel[6].rewardAmount.ToString();

                    rewardImageHeader.transform.gameObject.SetActive(true);
                    rewardTextHeader.transform.gameObject.SetActive(true);

                    firstSpin = false;
                }
                #endregion

                int fullCircles = Random.Range(5, 8);// ルーレットが何回転するかをランダムに決定
                float randomFinalAngle;

                if (!controlResultReward)  // 報酬の角度をランダムに選択して報酬を与える
                    randomFinalAngle = sectorsAngles[UnityEngine.Random.Range(0, sectorsAngles.Length)];
                else     // 報酬の角度をインスペクタで設定した割合に従って選択する
                    randomFinalAngle = sectorsAngles[GetRewardIndex()];

                finalAngle = -(fullCircles * 360 - randomFinalAngle); // ルーレットが停止する最終的な角度を計算
                isStarted = true;// ターンが開始されたことを示すフラグをセット
                StartCoroutine(TurnRoutine());// ターンのコルーチンを開始
            }
            else
            {
                Debug.LogWarning("Player does not have enough gold. Here you should open the shop for in app purchase.");
            }
        }

        public int GetRewardIndex() //Here final angle selection according to percentages that setted at inspector. Here there is a still change factor but random selection effect is minimized.
        {
            int randomValue = Random.Range(0, 100);// 0から99のランダムな値を取得

            for (int i = 0; i < PiecesOfWheel.Length; i++)
            {
                if (randomValue <= PiecesOfWheel[i].rewardChance && !rewardSelected)
                {
                    rewardSelected = true;// 報酬が選択されたことを示すフラグをセット
                    selectedRewardIndex = i;// 選択された報酬のインデックスを保存
                }
            }

            rewardSelected = false;// フラグをリセット
            return selectedRewardIndex;// 選択された報酬のインデックスを返す
        }

        private void ClaimTurnCost()
        {
            previousGold = totalGold;  // フリップアニメーションのために前の値を保存
            totalGold -= turnCost;    // ターンのコストを所持金から減算
            //StartCoroutine(UpdateRewardAmount());
        }

        private IEnumerator TurnRoutine()
        {
            while (isStarted)
            {
                float t = currentLerpRotationTime / maxLerpRotationTime;

                if (randomChange == 0)
                    t = 1f - (1f - t) * (1f - t);
                else
                    t = t * t * t * (t * (6f * t - 15f) + 10f);

                float angle = Mathf.Lerp(startAngle, finalAngle, t); //Linear Interpolation
                wheelParent.transform.eulerAngles = new Vector3(0, 0, angle);

                // Increment timer once per frame
                currentLerpRotationTime += Time.deltaTime;

                if (currentLerpRotationTime > maxLerpRotationTime || wheelParent.transform.eulerAngles.z == finalAngle)
                {
                    currentLerpRotationTime = maxLerpRotationTime;
                    isStarted = false;
                    startAngle = finalAngle % 360;

                    GiveAwardByAngle();
                }

                yield return new WaitForFixedUpdate();
            }
        }

        private void GiveAwardByAngle()
        {
            // Here you can set up rewards for every sector of wheel
            switch ((int)startAngle)
            {
                case 0:
                    rewardIndex = 6;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -45:
                    rewardIndex = 7;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -90:
                    rewardIndex = 0;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -135:
                    rewardIndex = 1;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -180:
                    rewardIndex = 2;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -225:
                    rewardIndex = 3;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -270:
                    rewardIndex = 4;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -315:
                    rewardIndex = 5;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -360:
                    rewardIndex = 6;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                default:
                    Debug.Log("There is no reward for this angle, please check angles");
                    break;
            }
        }

        IEnumerator RewardPopup(int rewardIndex)
        {
            yield return new WaitForSeconds(0.1f);
            rewardImagePopup.sprite = PiecesOfWheel[rewardIndex].rewardIcon;
            rewardTextPopup.text = PiecesOfWheel[rewardIndex].rewardAmount.ToString();
            popupPanel.gameObject.SetActive(true);

            StopCoroutine(TurnRoutine());

            //If player clicks "Claim", give that reward Call ClaimReward() function or if player selects "x2 Button", then show rewarded ad and then give double reward Call DoubleReward() function
        }

        public void CallRewardedAd()
        {
            Debug.LogWarning("Here you should call your show rewarded ad function and when rewarded ad completed call DoubleReward() function as result.");
        }

        private void DoubleReward()
        {
            rewardMultiplier = 2;
            Debug.Log("Rewards multiplier setted as " + rewardMultiplier);
            ClaimReward();
        }

        public void ClaimReward()
        {
            confetiEffect.GetComponent<ParticleSystemRenderer>().material.mainTexture = PiecesOfWheel[rewardIndex].confetiIcon;
            confetiEffect.Play();
            popupPanel.gameObject.SetActive(false);

            switch (PiecesOfWheel[rewardIndex].rewardCategory)
            {
                case RewardEnum.Gold:
                    {
                        totalGold += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Energy:
                    {
                        totalEnergy += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Life:
                    {
                        totalLife += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem1:
                    {
                        totalGem1 += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem2:
                    {
                        totalGem2 += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem3:
                    {
                        totalGem3 += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem4:
                    {
                        totalGem4 += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Money:
                    {
                        totalMoney += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                default:
                    Debug.Log("There is no reward for this angle, please check angles");
                    break;
            }

            rewardMultiplier = 1;   //Reset for next rewards
            turnButton.interactable = true; //Now player can turn wheel again
            StartCoroutine(UpdateRewardAmount());
        }

        private IEnumerator UpdateRewardAmount()
        {
            // Animation for increasing and decreasing of currencies amount
            const float seconds = 0.5f;
            float elapsedTime = 0;

            while (elapsedTime < seconds)
            {
                totalGoldText.text = Mathf.Floor(Mathf.Lerp(previousGold, totalGold, (elapsedTime / seconds))).ToString();
                totalLifeText.text = Mathf.Floor(Mathf.Lerp(previousLife, totalLife, (elapsedTime / seconds))).ToString();
                totalEnergyText.text = Mathf.Floor(Mathf.Lerp(previousEnergy, totalEnergy, (elapsedTime / seconds))).ToString();
                totalMoneyText.text = Mathf.Floor(Mathf.Lerp(previousMoney, totalMoney, (elapsedTime / seconds))).ToString();

                totalGem1Text.text = Mathf.Floor(Mathf.Lerp(previousGem1, totalGem1, (elapsedTime / seconds))).ToString();
                totalGem2Text.text = Mathf.Floor(Mathf.Lerp(previousGem2, totalGem2, (elapsedTime / seconds))).ToString();
                totalGem3Text.text = Mathf.Floor(Mathf.Lerp(previousGem3, totalGem3, (elapsedTime / seconds))).ToString();
                totalGem4Text.text = Mathf.Floor(Mathf.Lerp(previousGem4, totalGem4, (elapsedTime / seconds))).ToString();

                elapsedTime += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            previousGold = totalGold;
            previousLife = totalLife;
            previousEnergy = totalEnergy;
            previousMoney = totalMoney;
            previousGem1 = totalGem1;
            previousGem2 = totalGem2;
            previousGem3 = totalGem3;
            previousGem4 = totalGem4;

            totalGoldText.text = totalGold.ToString();
            totalLifeText.text = totalLife.ToString();
            totalEnergyText.text = totalEnergy.ToString();
            totalMoneyText.text = totalMoney.ToString();
            totalGem1Text.text = totalGem1.ToString();
            totalGem2Text.text = totalGem2.ToString();
            totalGem3Text.text = totalGem3.ToString();
            totalGem4Text.text = totalGem4.ToString();

            SavePlayerProgress(goldString, totalGold);
            SavePlayerProgress(energyString, totalEnergy);
            SavePlayerProgress(lifeString, totalLife);
            SavePlayerProgress(moneyString, totalMoney);
            SavePlayerProgress(gem1String, totalGem1);
            SavePlayerProgress(gem2String, totalGem2);
            SavePlayerProgress(gem3String, totalGem3);
            SavePlayerProgress(gem4String, totalGem4);
        }

        private void GetPlayerProgress()
        {
            //Gold
            if (PlayerPrefs.HasKey(goldString))
            {
                totalGold = PlayerPrefs.GetInt(goldString);
            }
            else
            {
                totalGold = 100; //Default Gold Value
                PlayerPrefs.SetInt(goldString, totalGold);
                PlayerPrefs.Save();
            }
            //Energy
            if (PlayerPrefs.HasKey(energyString))
            {
                totalEnergy = PlayerPrefs.GetInt(energyString);
            }
            else
            {
                totalEnergy = 0;
                PlayerPrefs.SetInt(energyString, totalEnergy);
            }
            //Life
            if (PlayerPrefs.HasKey(lifeString))
            {
                totalLife = PlayerPrefs.GetInt(lifeString);
            }
            else
            {
                totalLife = 0;
                PlayerPrefs.SetInt(lifeString, totalLife);
            }
            //Money
            if (PlayerPrefs.HasKey(moneyString))
            {
                totalMoney = PlayerPrefs.GetInt(moneyString);
            }
            else
            {
                totalMoney = 0;
                PlayerPrefs.SetInt(moneyString, totalMoney);
            }
            //Gem1
            if (PlayerPrefs.HasKey(gem1String))
            {
                totalGem1 = PlayerPrefs.GetInt(gem1String);
            }
            else
            {
                totalGem1 = 0;
                PlayerPrefs.SetInt(gem1String, totalGem1);
            }
            //Gem2
            if (PlayerPrefs.HasKey(gem2String))
            {
                totalGem2 = PlayerPrefs.GetInt(gem2String);
            }
            else
            {
                totalGem2 = 0;
                PlayerPrefs.SetInt(gem2String, totalGem2);
            }
            //Gem3
            if (PlayerPrefs.HasKey(gem3String))
            {
                totalGem3 = PlayerPrefs.GetInt(gem3String);
            }
            else
            {
                totalGem3 = 0;
                PlayerPrefs.SetInt(gem3String, totalGem3);
            }
            //Gem4
            if (PlayerPrefs.HasKey(gem4String))
            {
                totalGem4 = PlayerPrefs.GetInt(gem4String);
            }
            else
            {
                totalGem4 = 0;
                PlayerPrefs.SetInt(gem4String, totalGem4);
            }

            PlayerPrefs.Save();
            StartCoroutine(UpdateRewardAmount());
        }


        private void SavePlayerProgress(string st, int value)
        {
            PlayerPrefs.SetInt(st, value);
            PlayerPrefs.Save();
        }
    }
}