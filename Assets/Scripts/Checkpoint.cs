using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] GameObject myBatsPrefab;

    bool myBatsTriggered = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.activeSpawnPoint = this.gameObject;
            if (!myBatsTriggered)
            {
                myBatsTriggered = true;
                GameObject bats = Instantiate(myBatsPrefab, transform.position, Quaternion.identity);
                Destroy(bats, 1f);
            }
        }
    }
}
