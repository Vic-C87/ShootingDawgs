using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] GameObject myBatsPrefab;

    AudioSource myAudioSource;

    bool myBatsTriggered = false;

    [SerializeField] bool myIsEndPoint;
    [SerializeField] int myNextLevelNumber;

    float myTimeStamp;
    bool myShouldExit;
    [SerializeField] bool myIsLastLevel;

    private void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (myShouldExit && Time.time - myTimeStamp > 1f)
        {
            ReturnToMenu();
        }
    }

    void ReturnToMenu()
    {
        GameManager.Instance.ReturnToLevelSelect(myNextLevelNumber);
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

            if (myIsEndPoint && !myIsLastLevel) 
            {
                myShouldExit = true;
                myTimeStamp = Time.time;
            }
            else if (myIsLastLevel)
            {
                FindObjectOfType<PlayerCanvasUI>().ShowGameCompletedScreen();
            }
        }
    }
}
