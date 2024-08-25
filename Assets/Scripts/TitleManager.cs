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
    public string optionPath; 


    public Toggle gToggle;
    public Toggle cToggle;

    public Options options;

    // Start is called before the first frame update
    void Start()
    {
        optionPath = Application.persistentDataPath + "/settings.json";
        string resultPath = Application.persistentDataPath + "/results.json";
        if (!File.Exists(resultPath))
        {
            File.Create(resultPath);
        }
        SaveOptionFile();

        var json = File.ReadAllText(optionPath);
        print(json.ToString());
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

    public void SaveOptionFile()
    {
        if (!File.Exists(optionPath))
        {
            File.Create(optionPath);
        }
        print("Saved options");
        string save = JsonUtility.ToJson(options);
        File.WriteAllText(optionPath, save);
        print(save + " path: " + optionPath);
    }
}
