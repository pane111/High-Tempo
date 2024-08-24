using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Diagnostics;

using System.IO;
using UnityEngine.SceneManagement;

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

    [Serializable]
    public class Options
    {
        public bool screenGlitch = true;
        public bool defaultControls = true;
    }


    public string optionPath = "Assets/settings.json";
    public Options options;
    public Toggle gToggle;
    public Toggle cToggle;

    public PlayerMovement player;

    Stopwatch sw=new Stopwatch();

    Stopwatch pauseSw=new Stopwatch();
    bool isPaused=false;
    public GameObject pauseMenu;
    public bool playerGounded
    {
        set { if (value) { if (!isPaused)sw.Start(); } else { sw.Stop(); } }
    }

    DateTime startTime;

    TimeSpan eTime;
    TimeSpan gTime;

    Stopwatch tempSw;
    Stopwatch tempPauseSw;
    TimeSpan tempETime;
    TimeSpan tempGTime;
    public GameObject lastCheckpoint;

    void Start()
    {
        Time.timeScale = 1;
        if (isPaused)
        {
            TogglePause();
        }

        startTime = DateTime.Now;

        if (player == null)
        {
            player = FindObjectOfType<PlayerMovement>();
        }

        var json = File.ReadAllText(optionPath);
        if (json != null)
        {
            options = JsonUtility.FromJson<Options>(json);
        }
        else
        { options = new Options(); }

        gToggle.isOn = options.screenGlitch;
        cToggle.isOn = options.defaultControls;
    }

    // Update is called once per frame
    void Update()
    {
        eTime = DateTime.Now - startTime;
        TimeSpan eMinusPausedTime = eTime - pauseSw.Elapsed;
        timer.text = eMinusPausedTime.ToString(@"mm\:ss\:ff");


        

        gTime = sw.Elapsed;
        gTimer.text = gTime.ToString(@"mm\:ss\:ff");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

    }

    public void TouchCheckpoint(GameObject checkPoint)
    {
        lastCheckpoint = checkPoint;
        tempSw = sw;
        tempSw.Stop();
        tempPauseSw = pauseSw;
        tempPauseSw.Stop();
        tempETime = eTime;
        tempGTime = gTime;
    }

    public void ReloadToCP()
    {

    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            pauseMenu.SetActive(true);
            pauseSw.Start();
            sw.Stop();
            Time.timeScale = 0;
        }
        else
        {
            pauseMenu.SetActive(false);
            pauseSw.Stop();
            sw.Start();
            Time.timeScale = 1;
        }
    }

    public void ToggleScreenGlitch(bool toggle)
    {
        options.screenGlitch = toggle;
        SaveOptionFile();
    }
    public void ToggleControls(bool toggle)
    {
        options.defaultControls = toggle;
        SaveOptionFile();
    }

    void SaveOptionFile()
    {
        print("Saved options");
        var save = JsonUtility.ToJson(options);
        File.WriteAllText(optionPath, save);
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
