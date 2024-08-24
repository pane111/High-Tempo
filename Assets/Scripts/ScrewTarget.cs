using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrewTarget : MonoBehaviour
{
    public Transform targetPoint;
    public bool isEnemy;
    public bool breakOnHit;
    public Image EHB;
    public Image EFB;
    DroneScript drone;
    void Start()
    {
        drone = FindObjectOfType<DroneScript>();
        EHB = drone.EHB; EFB = drone.EFB;
        GameManager.Instance.onReset += EnableThis;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void DisableThis()
    {
        gameObject.SetActive(false);
    }
    void EnableThis()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            drone.lookTarget = targetPoint;
            drone.isIdle = false;
            drone.reticle.anim.ResetTrigger("Stop");
            drone.reticle.anim.SetTrigger("Start");

            if (isEnemy)
            {
                EFB.fillAmount = 1 / GetComponent<EnemyScript>().maxHealth;
                EHB.fillAmount = GetComponent<EnemyScript>().health / GetComponent<EnemyScript>().maxHealth;
                GetComponent<EnemyScript>().TakeDamage(1);
                if (GetComponent<EnemyScript>().health <=0)
                {
                    GetComponent<EnemyScript>().Die();
                    other.GetComponentInParent<AnimationEvents>().ScrewCol(true,false);
                    Invoke("DisableThis", 1.5f);
                    drone.reticle.anim.ResetTrigger("Start");
                    drone.reticle.anim.SetTrigger("Stop");
                    GameManager.Instance.defeatedEnemies.Add(gameObject);
                }
                else
                {
                    other.GetComponentInParent<AnimationEvents>().ScrewCol(false,false);
                }
            }
            else
            {
                other.GetComponentInParent<AnimationEvents>().ScrewCol(false,true);
            }

            if (breakOnHit)
            {
                other.transform.parent.position = transform.position;
                drone.reticle.anim.ResetTrigger("Start");
                drone.reticle.anim.SetTrigger("Stop");
                drone.isIdle = true;
                drone.lookTarget = null;
                print("Hit triggered");
                other.GetComponentInParent<PlayerMovement>().RestoreOneScrew();
                DisableThis();
            }
            
        }

        if (other.CompareTag("PlayerBullet"))
        {
            drone.isIdle = false;
            drone.lookTarget = targetPoint;
            drone.reticle.anim.ResetTrigger("Stop");
            drone.reticle.anim.SetTrigger("Start");
            Destroy(other.gameObject);
            if (isEnemy)
            {
                GetComponent<EnemyScript>().TakeDamage(0.15f);
            }
        }
    }
}
