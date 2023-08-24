using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // シングルトンインスタンスのGameManager
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get { return instance; }
    }
    public int coin;

    public int[] coinValuel;

    public int sideCoin;

    public int income;

    // Start is called before the first frame update
    void Start()
    {
        coinValuel[0] = 1;
        coinValuel[1] = 2;
        coinValuel[2] = 3;

        // GameManagerが既に存在する場合、このインスタンスを破棄する
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // GameManagerが存在しない場合、このインスタンスをシングルトンとして設定し、シーン間で破棄されないようにする
        instance = this;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
