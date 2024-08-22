using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.GraphicsBuffer;

public class DroneScript : MonoBehaviour
{
    public Transform followTarget;
    public float maxDist;
    public bool isIdle = true;

    public Transform lookTarget;
    public Transform chargeUI;

    public Transform shootPoint;
    public float chargeAmt;

    public UIRotate reticle;

    public GameObject bulletPrefab;

    public float shotDelay;
    public bool isShooting;

    public GameObject laserbeam;

    public GameObject enemyHPBar;
    public Image EHB;
    public Image EFB;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if (!isIdle && lookTarget != null)
        {
            transform.LookAt(lookTarget.position);
            transform.position = Vector3.Slerp(transform.position, lookTarget.position-transform.forward+Vector3.up, Time.deltaTime * 6);
            reticle.target = lookTarget;
            chargeUI.transform.position = Camera.main.WorldToScreenPoint(Vector3.zero);

            if (lookTarget.CompareTag("Enemy"))
            {
                Vector3 pos = Camera.main.WorldToScreenPoint(lookTarget.position);
                if (pos.y > 0 || pos.x > 0) {
                    enemyHPBar.SetActive(true);

                    enemyHPBar.transform.position = pos;
                }
                else
                {
                    enemyHPBar.SetActive(false);
                }
                
            }
            
        }
        else
        {
            enemyHPBar.SetActive(false);
            reticle.target = null;
        }

        if (lookTarget == null && !isIdle)
        {
            isIdle = true;
        }

        

        if (Input.GetMouseButtonDown(1)) 
        {
            isShooting = true;
            Shoot();
        
        }

        if (Input.GetMouseButtonUp(1))
        {
            isShooting = false;
            if (chargeAmt > 0.95f)
            {
                print("Shot laser");
                laserbeam.SetActive(true);
                Invoke("DisableBeam", 0.2f);
            }
        }

        if (Input.GetMouseButton(1))
        {
            chargeAmt += Time.deltaTime * 0.8f;
            if (isIdle)
            {
                transform.position = Vector3.Slerp(transform.position, shootPoint.position, Time.deltaTime * 15);
                transform.rotation = Quaternion.Euler(Vector3.zero);
            }


        }
        else
        {
            if ((followTarget.position - transform.position).magnitude > maxDist && isIdle)
            {
                transform.position = Vector3.Slerp(transform.position, followTarget.position, Time.deltaTime * 3);
            }
            chargeAmt -= Time.deltaTime * 3;
        }
        //Debug.Log(Input.GetMouseButton(1));

        chargeAmt = Mathf.Clamp(chargeAmt, 0, 1);
        chargeUI.GetComponent<Image>().fillAmount = chargeAmt;
        chargeUI.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    public void Shoot()
    {
        GameObject bullet = Instantiate (bulletPrefab,transform.position,Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 35,ForceMode.Impulse);
        Destroy(bullet, 1);

        if (isShooting ) { Invoke("Shoot", shotDelay); }
    }

    void DisableBeam()
    {
        laserbeam.SetActive(false);
    }
    
    
}
