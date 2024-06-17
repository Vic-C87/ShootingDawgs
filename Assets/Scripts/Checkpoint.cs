using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.activeSpawnPoint = this.gameObject;
<<<<<<< HEAD
            if (id == 7)
            {
                //other.GetComponent<PlayerMovement>().TriggerCameraDeadZone();
            }
=======
>>>>>>> 3101eff75bc9d85bb5060b85d64b2c3a383104a1
        }
    }
}
