using UnityEngine;

namespace FortuneWheel
{
    public class UIProfileController : MonoBehaviour
    {
        // カスタムの報酬カテゴリを表す列挙型
        public enum RewardEnum
        {
            None,
            Gold,
            Energy,
            Life,
            Gem1,
            Gem2,
            Gem3,
            Gem4,
            Money
        };

        // 報酬カテゴリとUI要素を関連付けるクラス
        [System.Serializable]
        public class MultiDimensional
        {
            public RewardEnum rewardType; // 報酬カテゴリ
            public GameObject UIElement; // 関連するUI要素
        }

        public MultiDimensional[] UIProfileElements; // 報酬カテゴリとUI要素のリスト

        private void Start()
        {
            SetActiveElements(); // 開始時に関連するUI要素をアクティブに設定
        }

        private void SetActiveElements()
        {
            // ルーレットの各セクションに対して処理を実行
            for (int i = 0; i < GameController.ins.PiecesOfWheel.Length; i++)
            {
                // セクションから収集された報酬カテゴリを取得
                GameController.RewardEnum collectedRewardType = GameController.ins.PiecesOfWheel[i].rewardCategory;

                // 収集された報酬カテゴリに基づいてUI要素の表示状態を設定
                switch (collectedRewardType)
                {
                    case GameController.RewardEnum.Gold:
                        {
                            FindThatElement(RewardEnum.Gold);
                        }
                        break;
                    case GameController.RewardEnum.Energy:
                        {
                            FindThatElement(RewardEnum.Energy);
                        }
                        break;
                    case GameController.RewardEnum.Life:
                        {
                            FindThatElement(RewardEnum.Life);
                        }
                        break;
                    case GameController.RewardEnum.Money:
                        {
                            FindThatElement(RewardEnum.Money);
                        }
                        break;
                    case GameController.RewardEnum.Gem1:
                        {
                            FindThatElement(RewardEnum.Gem1);
                        }
                        break;
                    case GameController.RewardEnum.Gem2:
                        {
                            FindThatElement(RewardEnum.Gem2);
                        }
                        break;
                    case GameController.RewardEnum.Gem3:
                        {
                            FindThatElement(RewardEnum.Gem3);
                        }
                        break;
                    case GameController.RewardEnum.Gem4:
                        {
                            FindThatElement(RewardEnum.Gem4);
                        }
                        break;
                    default:
                        UIProfileElements[i].UIElement.SetActive(false); // その他のカテゴリは非表示に設定
                        break;
                }
            }
        }

        // 指定された報酬カテゴリに対応するUI要素をアクティブに設定
        private void FindThatElement(RewardEnum type)
        {
            for (int i = 0; i < UIProfileElements.Length; i++)
            {
                if (UIProfileElements[i].rewardType == type)
                    UIProfileElements[i].UIElement.SetActive(true); // 該当するUI要素をアクティブに設定
            }
        }
    }
}
