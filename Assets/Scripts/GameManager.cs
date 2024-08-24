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

    public TextMeshProUGUI checkpointTimer;

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
    public bool isPaused=false;
    public GameObject pauseMenu;
    public bool playerGounded
    {
        set { if (value) { if (!isPaused) { sw.Start();  } } else { sw.Stop();  } }
    }

    DateTime startTime;

    TimeSpan eTime;
    TimeSpan gTime;

    Stopwatch tempSw;
    Stopwatch tempPauseSw;
    Stopwatch lostTime = new Stopwatch();
    Stopwatch eWatch = new Stopwatch();
    
    TimeSpan tempETime;
    TimeSpan tempGTime;
    public GameObject lastCheckpoint;

    public Animator reticleAnim;

    public List<GameObject> defeatedEnemies = new List<GameObject>();

    public Action onReset;
    DroneScript drone;

    void Start()
    {
        drone = FindObjectOfType<DroneScript>();
        lostTime.Stop();
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
        eWatch.Start();
    }

    public bool isLostTimeRunning;
    // Update is called once per frame
    void Update()
    {

        isLostTimeRunning = lostTime.IsRunning;
        eTime = DateTime.Now - startTime;
        if (!lostTime.IsRunning)
        {
            
            TimeSpan eMinusPausedTime = tempETime + eWatch.Elapsed - pauseSw.Elapsed;
            timer.text = eMinusPausedTime.ToString(@"mm\:ss\:ff");
            gTime = tempGTime + sw.Elapsed;
            
            
            gTimer.text = gTime.ToString(@"mm\:ss\:ff");
        }
        else
        {
            
            TimeSpan eMinusPausedTime = tempETime + eWatch.Elapsed - pauseSw.Elapsed;
            timer.text = eMinusPausedTime.ToString(@"mm\:ss\:ff");
            
            gTime = sw.Elapsed;
            gTimer.text = gTime.ToString(@"mm\:ss\:ff");
        }


        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

    }

    public void TouchCheckpoint(GameObject checkPoint)
    {
        eWatch.Restart();

        lostTime.Reset();
        lostTime.Start();
        lastCheckpoint = checkPoint;
        tempSw = sw;
        tempSw.Stop();
        tempPauseSw = pauseSw;
        tempPauseSw.Stop();
        tempETime = eTime;
        tempGTime = gTime;
        print(tempGTime.ToString());
        for (int i = defeatedEnemies.Count-1; i > 0; i--) 
        {
            if (defeatedEnemies[i] != null)
            Destroy(defeatedEnemies[i]);
        }
        defeatedEnemies.Clear();
        checkpointTimer.text = timer.text + "\n" + gTimer.text;
    }

    public void ReloadToCP()
    {
        eWatch.Restart();
        lostTime.Stop();
        drone.lookTarget = null;
        drone.isIdle = true;
        if (onReset != null)
            onReset();
        
        sw.Reset();
        pauseSw = tempPauseSw;
        
        eTime = tempETime;
        //gTime = tempGTime;
        for (int i = 0; i < defeatedEnemies.Count; i++)
        {
            if (defeatedEnemies[i] != null)
                defeatedEnemies[i].SetActive(true);
            if (defeatedEnemies[i] != null)
                defeatedEnemies[i].GetComponent<EnemyScript>().ResetEnemy();
        }
        defeatedEnemies.Clear();
        lostTime.Reset();
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
