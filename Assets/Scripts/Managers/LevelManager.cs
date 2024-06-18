using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] PlayerMovement player;

    public GameObject activeSpawnPoint;

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
    }

    public static LevelManager Instance;

    void heightCheck()
    {
        if (player.transform.position.y < 0f)
        {
            player.SetNewState(EPlayerState.Die);
            player.transform.position = activeSpawnPoint.transform.position;
        }
    }

    private void FixedUpdate()
    {
        heightCheck();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
