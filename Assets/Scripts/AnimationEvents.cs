using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using URPGlitch.Runtime.DigitalGlitch;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using TMPro;

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

    public Volume vm;
    DigitalGlitchVolume dgv;

    float intersity;
    float curInter = 0;
    bool isInterpolating = false;

    bool isDashing=false;

    public Image blackScreen;

    public bool canAttack;

    
    void Start()
    {
        blackScreen.gameObject.SetActive(true);
        anim = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        vm.profile.TryGet<DigitalGlitchVolume>(out dgv);
        InterpolateBlackscreen(0);
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

        if (isInterpolating)
        {
            if (curInter < intersity)
            {
                curInter += Time.deltaTime * 0.65f;
                MyFloatParameter fP = new MyFloatParameter(curInter, false);
                dgv.intensity.SetValue(fP);
            }
            else
            {
                isInterpolating = false;
            }
            
        }

        if (isDashing)
        {
          
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 100, Time.deltaTime * 5);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 80, Time.deltaTime * 5);
        }
    }

    void Iframes()
    {
        pm.invincible = true;
    }
    void StopIframes()
    {
        pm.invincible = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Collider>().enabled = true;
    }

    public void DashCam()
    {
        isDashing = true;
        Invoke("StopDashCam", 0.2f);
        
    }
    public void StopDashCam()
    {
        isDashing = false;
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

    void JumpUp(float force)
    {
        pm.rb.AddForce(transform.up * force, ForceMode.Impulse);
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

    public sealed class MyFloatParameter : VolumeParameter<float>
    {
        public MyFloatParameter(float value, bool overrideState = false)
            : base(value, overrideState) { }

        public sealed override void Interp(float from, float to, float t)
        {
            m_Value = from + (to - from) * t;
        }
    }

    void Reload()
    {
        if (GameManager.Instance.lastCheckpoint == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {

            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().position = (GameManager.Instance.lastCheckpoint.transform.position);
            Debug.Log(transform.position);
            pm.invincible = true;
            Invoke("StopIframes", 1);
            Heal();
            InterpolateBlackscreen(0);
            TriggerGlitch(0);
            EnablePm();
            StartMovement();
            StopDash();
            GameManager.Instance.ReloadToCP();
        }
        
    }


    void TriggerGlitch(float intensity)
    {
        if (dgv!=null && GameManager.Instance.options.screenGlitch)
        {
            MyFloatParameter fP = new MyFloatParameter(intensity, false);
            dgv.intensity.SetValue(fP);
        }
    }
    void InterpolateGlitch(float intensity)
    {
        if (dgv!= null && GameManager.Instance.options.screenGlitch)
        {
            intersity = intensity;
            isInterpolating = true;
        }
    }

    void InterpolateBlackscreen(float v)
    {
        Color newColor = new Color(0, 0, 0, v);
        DOVirtual.Color(blackScreen.color, newColor, 1.4f, (value) =>
        {
            blackScreen.color = value;
        });
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
                //transform.position = new Vector3(transform.position.x, 8, transform.position.z);
                ds.lookTarget = null;
                anim.SetTrigger("Finisher");
                Heal();
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
    public void Heal()
    {
        pm.curHealth = pm.maxHealth;
        pm.healthBar.fillAmount = 1;
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
