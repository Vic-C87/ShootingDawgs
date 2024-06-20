using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] PlayerMovement player;
    PlayerCanvasUI playerCanvas;

    public GameObject activeSpawnPoint;
    AudioSource myAudioSource;

    bool myGameIsPaused;
    float myTempTimeScale;

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
        playerCanvas = FindObjectOfType<PlayerCanvasUI>();
        myAudioSource.loop = true;
        myAudioSource.volume = 0.2f;
        myAudioSource.Play();
        playerCanvas.SetLevelText(GameManager.Instance.GetCurrentLevel());
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

    public void OnPauseGame()
    {
        myGameIsPaused = !myGameIsPaused;
        if (myGameIsPaused)
        {
            myTempTimeScale = Time.timeScale;
            playerCanvas.DisplayPauseScreen();
            Time.timeScale = 0f;
        }
        else
        {
            playerCanvas.HidePauseMenu();
            Time.timeScale = myTempTimeScale;
        }
    }
}
