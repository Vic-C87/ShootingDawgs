using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] GameObject myBatsPrefab;

    AudioSource myAudioSource;

    bool myBatsTriggered = false;

    private void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(LevelManager.Instance.activeSpawnPoint != this.gameObject) 
            {
                LevelManager.Instance.activeSpawnPoint = this.gameObject;
                if (myAudioSource.clip != null)
                    myAudioSource.Play();
            }

            if (!myBatsTriggered)
            {
                myBatsTriggered = true;
                GameObject bats = Instantiate(myBatsPrefab, transform.position, Quaternion.identity);
                Destroy(bats, 1f);
            }
        }
    }
}
