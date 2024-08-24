using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerMovement : MonoBehaviour
{
    public Animator anim;
    public Transform rig;
    public float initMoveSpeed;
    public Rigidbody rb;
    public float jumpForce;
    public float groundCheckRange;
    public Transform center;
    public bool isGrounded = false;

    public float maxScrews=3;
    public float screws;
    public Image dashMeter;

    public bool canMove=true;

    public ParticleSystem leftDodge;
    public ParticleSystem rightDodge;
    public ParticleSystem screwEffect;

    public int maxHealth;
    public int curHealth;
    public Image healthBar;

    public ParticleSystem dmgEffect;


    public bool invincible=false;
    void Start()
    {
        curHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        

    }
    

    // Update is called once per frame
    void Update()
    {
        
       

        if (Physics.Raycast(center.position, Vector3.down, groundCheckRange))
        {
            anim.SetBool("Grounded", true);
            //screws = maxScrews;
            //dashMeter.fillAmount =1;
            isGrounded = true;
            GameManager.Instance.playerGounded = true;
        }
        else
        {
            anim.SetBool("Grounded", false);
            isGrounded = false;
            GameManager.Instance.playerGounded = false;
        }
        if (transform.position.y < 0)
        {
            curHealth=0;
            healthBar.fillAmount = 0;
            anim.SetTrigger("Death");
        }

        Quaternion forwardQ = Quaternion.Euler(0, 0, 0);
        Quaternion backwardQ = Quaternion.Euler(0, 180, 0);

        if (isGrounded && canMove)
        {
            string _axis = "Vertical";
            if (!GameManager.Instance.options.defaultControls)
            {
                _axis = "Horizontal"; 
            }

            if (Input.GetAxisRaw(_axis) > 0)
            {
                anim.SetBool("Moving", true);
                rig.rotation = Quaternion.Lerp(rig.rotation, forwardQ, Time.deltaTime * 15);
                rb.velocity = new Vector3(0, rb.velocity.y, initMoveSpeed);
            }
            else if (Input.GetAxisRaw(_axis) < 0)
            {
                anim.SetBool("Moving", true);
                rig.rotation = Quaternion.Lerp(rig.rotation, backwardQ, Time.deltaTime * 15);
                rb.velocity = new Vector3(0, rb.velocity.y, -initMoveSpeed);
            }
            else
            {
                rig.rotation = Quaternion.Lerp(rig.rotation, forwardQ, Time.deltaTime * 15);
                anim.SetBool("Moving", false);
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                anim.SetTrigger("Dodge");
            }
            


            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            }


        }


        if (Input.GetMouseButtonDown(0) && !GameManager.Instance.isPaused)
        {
            anim.SetTrigger("Attack");
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            TakeForcedDamage();
            TakeForcedDamage();
            TakeForcedDamage();
        }
        

        if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && canMove)
        {
            if (screws > 0)
            {
                screws = screws - 1;
                dashMeter.fillAmount = screws / maxScrews;
                anim.SetTrigger("Screw");
            }
            
        }
    }

    public void RefillScrews()
    {
        //print("Refilled all screws");
        screws = maxScrews;
        dashMeter.fillAmount = 1;
    }
    public void RestoreOneScrew()
    {
        //print("Restored screw");
        screws++;
        if (screws > maxScrews) { screws=maxScrews;}
        dashMeter.fillAmount = screws / maxScrews;
    }
    public void TakeForcedDamage()
    {
        invincible = true;

        dmgEffect.Play();
        curHealth--;
        healthBar.fillAmount = (float)curHealth / (float)maxHealth;
        if (curHealth <= 0)
        {
            rb.useGravity = false;
            //rb.isKinematic = true;
            //GetComponent<Collider>().enabled = false;
            anim.SetTrigger("Death");
        }
        else
        {
            anim.SetTrigger("Damage");
        }
    }

    public void TakeDamage()
    {
        if (!invincible)
        {
            invincible = true;
            
            dmgEffect.Play();
            curHealth--;
            healthBar.fillAmount = (float)curHealth / (float)maxHealth;
            if (curHealth <= 0)
            {
                rb.useGravity = false;
                //rb.isKinematic = true;
                //GetComponent<Collider>().enabled = false;
                anim.SetTrigger("Death");
            }
            else
            {
                anim.SetTrigger("Damage");
            }
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //print("Collided with trigger " + other.name);
        if (other.CompareTag("EnemyAttack"))
        {
            TakeDamage();
        }
        if (other.CompareTag("Checkpoint"))
        {
            if (GameManager.Instance.lastCheckpoint != other.gameObject)
            {
                other.GetComponentInChildren<ParticleSystem>().Play();
                GameManager.Instance.TouchCheckpoint(other.gameObject);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            RefillScrews();
        }
    }

}
