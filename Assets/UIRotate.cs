using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRotate : MonoBehaviour
{
    public float speed;
    RectTransform obj;
    public Transform target;
    public Animator anim;
    void Start()
    {
        obj = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.position);
        }
        else
        {
            transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        }
        obj.rotation *= Quaternion.Euler(0, 0, (Time.deltaTime * speed)%360);

    }
}
