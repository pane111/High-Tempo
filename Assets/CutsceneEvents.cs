using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneEvents : MonoBehaviour
{
    
    public GameObject bossToSpawn;
    public bool finish;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void DisablePlayer()
    {
        GameManager.Instance.player.gameObject.SetActive(false);
    }

    void Dialogue(string message)
    {
        GameManager.Instance.DisplayDialogue(message);
    }

    void EndCutscene()
    {
        if (finish)
        {
            SceneManager.LoadScene("ResultScreen");
        }

        GameManager.Instance.player.gameObject.SetActive(true);
        if (bossToSpawn != null)
        {
            GameObject boss = Instantiate(bossToSpawn, transform.position, Quaternion.identity);
            boss.transform.parent = transform.parent;
        }

            gameObject.SetActive(false); }

    void White(float dur)
    {
        GameManager.Instance.InterpolateWhitescreen(dur);
    }
}
