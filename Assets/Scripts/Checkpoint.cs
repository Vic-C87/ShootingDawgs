using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] int id;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.activeSpawnPoint = this.gameObject;
            if (id == 7)
            {
                //other.GetComponent<PlayerMovement>().TriggerCameraDeadZone();
            }
        }
    }
}
