using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] PlayerMovement player;

    public GameObject activeSpawnPoint;
    AudioSource myAudioSource;

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        myAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //myAudioSource.volume = 0.3f;
        myAudioSource.Play();
    }

    void heightCheck()
    {
        if (player.transform.position.y < -1f)
        {
            RespawnPlayer();
        }
    }

    private void Update()
    {
        heightCheck();
    }

    public void RespawnPlayer()
    {
        player.transform.position = activeSpawnPoint.transform.position;
    }
}
