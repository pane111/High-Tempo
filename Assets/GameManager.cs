using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI timer;
    public TextMeshProUGUI gTimer;
    
    

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    Stopwatch sw=new Stopwatch();
    public bool playerGounded
    {
        set { if (value) { sw.Start(); } else { sw.Stop(); } }
    }

    DateTime startTime;

    TimeSpan eTime;

    

    TimeSpan gTime;

    void Start()
    {
        startTime = DateTime.Now;
        
    }

    // Update is called once per frame
    void Update()
    {
        eTime = DateTime.Now - startTime;
        timer.text = eTime.ToString(@"mm\:ss\:ff");
        gTime = sw.Elapsed;
        gTimer.text = gTime.ToString(@"mm\:ss\:ff");
    }
}
