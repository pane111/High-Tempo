using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimEvents : MonoBehaviour
{
    public List<GameObject> projectiles = new List<GameObject>();
    public Transform shotPoint;
    void Start()
    {
        GameManager.Instance.onReset += DestroyThis;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = -GameManager.Instance.player.transform.forward;
    }
    void SpawnProjectile(int n)
    {
        GameObject p = Instantiate(projectiles[n], shotPoint.position,Quaternion.identity);

        Destroy(p, 5);
    }

    void DestroyThis()
    {
        Destroy(gameObject);
    }

    void TriggerDialogue(string dialogue)
    {
        GameManager.Instance.DisplayDialogue(dialogue);
    }
}
