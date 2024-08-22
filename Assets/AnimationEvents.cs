using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public GameObject weapon;
    public Animator anim;
    PlayerMovement pm;
    public DroneScript ds;
    public Transform center;
    public Transform playerPivot;
    public GameObject finisherEffect;
    public GameObject screwEffect;

    public bool canAttack;
    void Start()
    {
        anim = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canAttack)
        {
            if (Input.GetMouseButtonDown(0))
            {
                anim.SetTrigger("Attack");
            }
        }
    }

    public void SetTag(string t)
    {
        weapon.tag = t;
    }
    public void ResetAnimTrigger(string t)
    {
        anim.ResetTrigger(t);
    }
    void EnableAttack()
    {
        canAttack = true;
    }
    private void DisableAttack()
    {
        canAttack = false;
    }
    public void Dash(float force)
    {
        
        pm.enabled = false;
        if (ds.lookTarget!=null)
        {

            Vector3 dir = ds.lookTarget.transform.position - center.position;
            dir.x = 0;
            playerPivot.forward = dir.normalized;
            //transform.forward = dir.normalized;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().AddForce(dir.normalized * force, ForceMode.VelocityChange);
        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.VelocityChange);
        }
        
    }
    
    public void StopDash()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = true;
        playerPivot.forward = transform.forward;
        pm.enabled = true;
        
    }
    public void StopDash2()
    {
        GetComponent<Rigidbody>().useGravity = true;
        playerPivot.forward = transform.forward;
        pm.enabled = true;
    }
    public void ResetVelocity()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = true;
    }
    public void ScrewCol(bool kill, bool obj)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = true;
        if (!kill)
        {
            anim.SetTrigger("HitScrew");
            if (obj)
            {
                GetComponent<Rigidbody>().AddForce(Vector3.up * 8 - Vector3.forward * 2, ForceMode.VelocityChange);
            }
            else
            {
                GetComponent<Rigidbody>().AddForce(Vector3.up * 6 - Vector3.forward * 6, ForceMode.VelocityChange);
            }
            playerPivot.forward = transform.forward;
            anim.ResetTrigger("Screw");
            Invoke("EnablePm", 0.3f);
        }
        else
        {
            if (ds.lookTarget!=null)
            {
                pm.RefillScrews();
                
                transform.position = ds.lookTarget.position - transform.forward;
                
                anim.SetTrigger("Finisher");
            }
            else
            {
                anim.SetTrigger("HitScrew");

                GetComponent<Rigidbody>().AddForce(Vector3.up * 6 - Vector3.forward * 6, ForceMode.VelocityChange);
                playerPivot.forward = transform.forward;
                anim.ResetTrigger("Screw");
                Invoke("EnablePm", 0.3f);
            }
            

        }
        
        

    }
    void StopMovement()
    {
        pm.canMove = false;
    }
    void StartMovement()
    {
        pm.canMove = true;
    }

    void DisablePM()
    {
        pm.enabled = false;
    }
    void EnablePm()
    {
        pm.enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
    }
    public void SpawnFinisherEffect()
    {
        Instantiate(finisherEffect, transform);
    }

    public void ChangeTimeScale(float amt)
    {
        Time.timeScale = amt;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
    void SpawnScrewEffect()
    {
        pm.screwEffect.Play();
    }
    
}
