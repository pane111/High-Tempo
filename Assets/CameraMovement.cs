using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float maxDist;
    public float camSpeed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
          transform.position = Vector3.Lerp(transform.position, target.position - target.forward*maxDist, Time.deltaTime * camSpeed);
       
    }
}
