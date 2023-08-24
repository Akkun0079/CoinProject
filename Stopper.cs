using UnityEngine;

namespace FortuneWheel
{
    public class Stopper : MonoBehaviour
    {
        private Animator anim; // アニメーターコンポーネントの参照を格納する変数

        private void Start()
        {
            anim = transform.GetComponent<Animator>(); // このオブジェクトのアニメーターコンポーネントを取得して変数に格納
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.tag == "PinPoint") // 衝突したオブジェクトのタグが "PinPoint" である場合
            {
                anim.SetBool("isPlay", true); // アニメーションのパラメーター "isPlay" を true に設定して再生開始

                // リワードヘッダーに関する情報を更新
                GameController.ins.rewardImageHeader.sprite = GameController.ins.PiecesOfWheel[collision.transform.parent.GetComponent<PieceObject>().index].rewardIcon;
                GameController.ins.rewardTextHeader.text = GameController.ins.PiecesOfWheel[collision.transform.parent.GetComponent<PieceObject>().index].rewardAmount.ToString();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.tag == "PinPoint") // 衝突したオブジェクトのタグが "PinPoint" である場合
            {
                anim.SetBool("isPlay", false); // アニメーションのパラメーター "isPlay" を false に設定して再生停止
            }
        }
    }
}
