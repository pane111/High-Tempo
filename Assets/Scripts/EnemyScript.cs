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

    public float maxPoise;
    public float poise;
    public bool isBoss;

    public ParticleSystem finalHit;
    public ParticleSystem teleport;

    public Transform player;
    public float playerFollowDistance;
    bool following;

    public float playerStoppingDistance;
    public float moveSpeed;

    public Transform rig;

    bool canMove;

    public float atkDelay;
    public float comboChance;

    public bool grounded;

    float eTime = 0;
    void Start()
    {
        eTime = atkDelay;
        health = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        drone = FindObjectOfType<DroneScript>();
        player = GameManager.Instance.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float curDistance = player.position.z - transform.position.z;
        if (canMove)
        {
            

            if (Mathf.Abs(curDistance) <= playerFollowDistance)
            {
                following = true;
            }
            if (curDistance < 0)
            {
                if (rig != null)
                    rig.rotation = Quaternion.Euler(0, 180, 0);
                
            }
            else
            {
                if (rig != null)
                    rig.rotation = Quaternion.Euler(0, 0, 0);
            }

            
            if (following && transform.position.z < player.position.z && health > 0)
            {
                transform.position = new Vector3(transform.position.x,transform.position.y, player.position.z+4);
                teleport.Play();
            }

            if (Mathf.Abs(curDistance) > playerStoppingDistance && following && grounded)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, moveSpeed * (curDistance / Mathf.Abs(curDistance)));
                anim.SetBool("Moving", true);
                eTime = atkDelay;

            }
            else
            {
               
                anim.SetBool("Moving", false);
            }
        }
        

        if (Mathf.Abs(curDistance) <= playerStoppingDistance && following && grounded)
        {

            if (player.position.y > transform.position.y+2 && Mathf.Abs(curDistance) < 0.8f) 
            {
                anim.SetTrigger("JumpAttack");
                
            }
            //can attack
            eTime += Time.deltaTime;
            if (eTime >= atkDelay)
            {
                eTime = 0;
                anim.SetTrigger("Attack");
                
            }
        }
    }

    void Combo()
    {
        float randomChance = Random.Range(0, 100);

        if (randomChance <= comboChance)
        {
            anim.SetTrigger("Attack");
        }
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
            
            if (other.CompareTag("PlayerSword"))
            {
                TakeDamage(1);
                

                if ((isBoss && poise <= 0) || !isBoss)
                {
                    SetMass(1);
                    rb.AddForce(-transform.forward * 1.5f, ForceMode.Impulse);
                    anim.SetTrigger("Hit");
                }
                
                //reticleAnim.ResetTrigger("Stop");
                //reticleAnim.SetTrigger("Start");
                
                Time.timeScale = 0.15f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                Invoke("ResetTime", 0.02f);
                
            }
            else if (other.CompareTag("PlayerSwordStagger"))
            {
                TakeDamage(2);

                if ((isBoss && poise <= 0) || !isBoss)
                {
                    grounded = false;
                    anim.SetBool("Grounded", grounded);
                    anim.SetTrigger("StaggerHit");

                    //reticleAnim.ResetTrigger("Start");
                    //reticleAnim.SetTrigger("Stop");
                    rb.velocity = Vector3.zero;
                    SetMass(1);
                    rb.AddForce(transform.up * 8, ForceMode.Impulse);
                }
                    
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
        if (isBoss)
        {
            poise-=amount;
        }
        
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

    void SetMass(int m)
    {
        rb.mass = m;
    }
    void ResetJumpTrigger()
    {
        anim.ResetTrigger("JumpAttack");
    }

    public void StopMovement()
    {
        canMove = false;
    }
    public void EnableMovement()
    {
        canMove=true;
    }
    public void EnemyDash(float amount)
    {
        SetMass(1);
        rb.AddForce(rig.forward * amount, ForceMode.Impulse); 
    }
    void JumpUp()
    {
        SetMass(1);
        rb.AddForce(transform.up * 8.5f, ForceMode.Impulse);
    }
    

    public void Die()
    {
        rig = null;
        GetComponent<ScrewTarget>().EFB.fillAmount = 0;
        gameObject.tag = "Untagged";
        Time.timeScale = 0.15f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        Invoke("ResetTime", 0.04f);
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        anim.SetTrigger("Death");
        
    }

    public void RePoise()
    {
        poise = maxPoise;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            grounded = true;
            anim.SetBool("Grounded", grounded);
        }
    }

    private void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.CompareTag("Terrain"))
        {
            grounded = false;
            anim.SetBool("Grounded", grounded);
        }
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
