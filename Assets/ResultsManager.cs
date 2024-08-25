using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System;

public class ResultsManager : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    [Serializable]
    public class Results
    {
        public string totalTime;
        public string groundTime;
    }

    Results results;
    void Start()
    {
        var json = File.ReadAllText(Application.persistentDataPath + "/results.json");
        if (json != null)
        {
            results = JsonUtility.FromJson<Results>(json);
        }
        else
        { results = new Results(); }

        resultText.text = "Total time: " + results.totalTime + "\nGrounded time: " + results.groundTime + "\nDon't worry, I didn't count the time you spent watching the cutscene";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Return()
    {
        SceneManager.LoadScene(0);
    }
}
