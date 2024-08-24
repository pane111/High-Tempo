using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;


public class TitleManager : MonoBehaviour
{


    [Serializable]
    public class Options
    {
        public bool screenGlitch = true;
        public bool defaultControls = true;
    }
    public string optionPath = "Assets/settings.json";


    public Toggle gToggle;
    public Toggle cToggle;

    Options options;

    // Start is called before the first frame update
    void Start()
    {
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
        
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void ExitGame()
    {
        Application.Quit();
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
}
