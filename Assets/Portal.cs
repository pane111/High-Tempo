using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    public bool startCutscene;
    public GameObject cutsceneCam;
    public GameObject areaToEnable;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.whitescreen.color = Color.white;
            GameManager.Instance.InterpolateWhitescreen(0.5f);
            other.transform.position = target.position;
            areaToEnable.SetActive(true);
            if (startCutscene)
            {
                cutsceneCam.gameObject.SetActive(true);
            }
        }
    }
}
