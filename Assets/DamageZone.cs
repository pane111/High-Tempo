using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UI;

public class DamageZone : MonoBehaviour
{
    // Start is called before the first frame update
    public float chargeTime;
    float eTime=0;
    public MeshRenderer visual;

    public Material warningMat;
    public Material damageMat;

    public Image circle;
    public Image circle2;

    public float sphereCastRadius;
    public float sphereCastDistance;

    public ParticleSystem hitParticle;
    void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
        Handles.DrawWireDisc(transform.position, transform.forward, sphereCastRadius);
        Handles.DrawWireDisc(transform.position-transform.forward*sphereCastDistance, transform.forward, sphereCastRadius);
        Debug.DrawLine(transform.position, transform.position-transform.forward * sphereCastDistance);
        
    }


    // Update is called once per frame
    void Update()
    {
        eTime += Time.deltaTime;
        

        if (eTime < chargeTime * 0.15f)
        {
            visual.enabled = true;
        }
        else
        {
            visual.enabled = false;
            
        }

        circle.fillAmount = eTime / chargeTime;
        circle2.fillAmount = eTime / chargeTime;

        if (eTime >= chargeTime)
        {
            eTime = 0;
            Collider[] cols = Physics.OverlapCapsule(transform.position, transform.position - transform.forward * sphereCastDistance, sphereCastRadius, 1 << 6);
            if (cols.Length > 0)
            {
                hitParticle.Play();
                cols[0].gameObject.GetComponent<PlayerMovement>().TakeForcedDamage();
            }
        }
        
    }
}
