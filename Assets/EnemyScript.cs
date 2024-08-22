using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Rendering.Universal;

public class EnemyScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject hitEffect;
    public Transform center;
    public bool hyperarmor;
    Animator anim;
    Rigidbody rb;
    public Animator reticleAnim;
    DroneScript drone;
    public float maxHealth;
    public float health;

    public ParticleSystem finalHit;
    void Start()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        drone = FindObjectOfType<DroneScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword") || other.CompareTag("PlayerSwordStagger"))
        {
            Vector3 hit = other.ClosestPointOnBounds(center.position);
            Vector3 dir = center.position - hit;
            GameObject effect = Instantiate(hitEffect,center.position,Quaternion.identity);
            float angle = Vector3.Angle(dir, transform.forward);
            effect.GetComponent<ParticleSystem>().startRotation = angle;
            effect.GetComponent<ParticleSystem>().Play();

            reticleAnim.GetComponent<UIRotate>().target = center;
            reticleAnim.ResetTrigger("Stop");
            reticleAnim.SetTrigger("Start");
            drone.lookTarget = center;
            drone.isIdle = false;
            
            if (!hyperarmor && other.CompareTag("PlayerSword"))
            {
                TakeDamage(1);
                rb.AddForce(-transform.forward * 4, ForceMode.Impulse);
                anim.SetTrigger("Hit");
                //reticleAnim.ResetTrigger("Stop");
                //reticleAnim.SetTrigger("Start");
                
                Time.timeScale = 0.15f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                Invoke("ResetTime", 0.02f);
                
            }
            else if (!hyperarmor && other.CompareTag("PlayerSwordStagger"))
            {
                TakeDamage(2);
                anim.SetTrigger("StaggerHit");
                //reticleAnim.ResetTrigger("Start");
                //reticleAnim.SetTrigger("Stop");
                rb.velocity = Vector3.zero;
                rb.AddForce(transform.up*8,ForceMode.Impulse);
                
                Time.timeScale = 0.2f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                Invoke("ResetTime", 0.13f);
            }
        }

        if (other.CompareTag("Player"))
        {

            Vector3 hit = other.ClosestPointOnBounds(center.position);
            Vector3 dir = center.position - hit;
            GameObject effect = Instantiate(hitEffect, center.position, Quaternion.identity);
            float angle = Vector3.Angle(dir, transform.forward);
            effect.GetComponent<ParticleSystem>().startRotation = angle;
            effect.GetComponent<ParticleSystem>().Play();
            Time.timeScale = 0.15f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Invoke("ResetTime", 0.02f);

            anim.SetTrigger("Hit");

            rb.AddForce(-transform.forward * 4, ForceMode.Impulse);
            
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        
        GetComponent<ScrewTarget>().EHB.fillAmount = health / maxHealth;
        if (health<=1)
        {
            GetComponent<ScrewTarget>().EFB.fillAmount = 1 / maxHealth;
            GetComponent<ScrewTarget>().EFB.gameObject.SetActive(true);
            if (finalHit != null)
            {
                finalHit.Play();
            }
        }
        //if (health <= 0) { Die(); }
    }

    public void Die()
    {
        GetComponent<ScrewTarget>().EFB.fillAmount = 0;
        gameObject.tag = "Untagged";
        Time.timeScale = 0.15f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        Invoke("ResetTime", 0.04f);
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        anim.SetTrigger("Death");
        
    }

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
    }

    public void ResetTime()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
