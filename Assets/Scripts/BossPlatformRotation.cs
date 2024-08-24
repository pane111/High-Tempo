using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPlatformRotation : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerMovement pm;
    float rotateAmount;
    public bool isRotating;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        KeyCode btn = KeyCode.A;
        KeyCode btn2 = KeyCode.D;

        if (!GameManager.Instance.options.defaultControls)
        {
            btn = KeyCode.W; btn2 = KeyCode.S;
        }

        if (Input.GetKeyDown(btn) && pm.canMove && pm.isGrounded)
        {
            RotateLeft();
        }
        if (Input.GetKeyDown(btn2) && pm.canMove && pm.isGrounded)
        {
            RotateRight();
        }

        if (isRotating)
        {
            transform.Rotate(0, rotateAmount * Time.deltaTime * 5, 0);
        }
    }
    public void RotateLeft()
    {

        rotateAmount = -36;
        isRotating = true;
        pm.anim.SetTrigger("JumpLeft");
        pm.leftDodge.Play();
        
        Invoke("StopRotating", 0.2f);
        
    }
    public void RotateRight()
    {
        rotateAmount = 36;
        isRotating = true;
        pm.anim.SetTrigger("JumpRight");
        pm.rightDodge.Play();
        Invoke("StopRotating", 0.2f);
    }
    void StopRotating()
    {
        isRotating = false;
    }
}
