using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject spawnOnDeath;
    

    private void OnDestroy()
    {
        Instantiate(spawnOnDeath,transform.position,Quaternion.identity);
    }
}
