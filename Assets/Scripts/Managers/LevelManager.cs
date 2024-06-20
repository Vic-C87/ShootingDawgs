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
        myAudioSource.loop = true;
        myAudioSource.volume = 0.2f;
        myAudioSource.Play();
    }

    private void Update()
    {
        heightCheck();
    }

    void heightCheck()
    {
        if (player.transform.position.y < -1f)
        {
            RespawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        player.transform.position = activeSpawnPoint.transform.position;
        GameManager.Instance.IncreaseDeathCount();
    }
}
