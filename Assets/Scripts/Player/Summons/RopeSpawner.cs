using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSpawner : MonoBehaviour
{
    [SerializeField] RopeGuy myParent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
        {
            Debug.Log("HIT");
            myParent.SpawnNewRopePiece();
        }
    }
}
