using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pusher : MonoBehaviour
{
    [SerializeField] PlayerMovement myPlayer;

    private void OnDestroy()
    {
        myPlayer.SpawnSmoke(transform.position, 1f);
    }
}
