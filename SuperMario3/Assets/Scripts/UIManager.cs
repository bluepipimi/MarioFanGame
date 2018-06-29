using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text ScoreUI;
    public Text CoinUI;
    public Text WorldUI;
    public Text TimeUI;

    public int Time;

	// Use this for initialization
	void Start () {
            
	}
	
	// Update is called once per frame
	void Update () {
	    	
	}

    public void Coin(int i)
    {
        if (i < 10)
            CoinUI.text = "x0" + i;
        else // 11, 12, ... 100, 101, ...
        {
            int a = i % 100;
            if (a < 10)
            {
                CoinUI.text = "x0" + a;
            }
            else
            {
                CoinUI.text = "x" + a;
            }
        }
    }

    public void Timer(float time)
    {
        TimeUI.text = "Time \n" + time.ToString("f0");
    }

    
}
