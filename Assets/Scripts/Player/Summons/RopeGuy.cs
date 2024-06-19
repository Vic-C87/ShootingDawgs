using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGuy : MonoBehaviour
{
    [SerializeField] GameObject myRopePrefab;
    [SerializeField] Transform myRopeSpawnPoint;
    [SerializeField] GameObject myRopeBottom;

    [SerializeField] float myMaxLength;
    [SerializeField] float myRopeSpeed;
    float myLifeTime;
    float myTimeStamp;

    Vector3 myStartingLocalPosition;
    List<GameObject> myRopeList;

    PlayerMovement myPlayer;
    AudioSource myAudioSource;
    bool mySoundIsPlaying;

    void Awake()
    {
        myRopeList = new List<GameObject>();
        myAudioSource = GetComponent<AudioSource>();
        mySoundIsPlaying = false;
    }

    void Start()
    {
        myStartingLocalPosition = myRopeSpawnPoint.transform.localPosition;
    }

    void Update()
    {
        MoveRopeDown();
        if (Time.time - myTimeStamp > myLifeTime) 
        {
            DeSpawn();
        }
    }

    void MoveRopeDown()
    {
        if (myRopeSpawnPoint.position.y - myRopeBottom.transform.position.y < myMaxLength) 
        {
            if (!mySoundIsPlaying) 
            {
                mySoundIsPlaying = true;
                myAudioSource.Play();
            }
            myRopeBottom.transform.position += Vector3.down * myRopeSpeed * Time.deltaTime;
        }
        else
        {
            mySoundIsPlaying = false;
            myAudioSource.Stop();
        }
    }

    public void SpawnNewRopePiece()
    {
        GameObject rope = Instantiate<GameObject>(myRopePrefab, myRopeSpawnPoint.position, Quaternion.identity, myRopeBottom.transform);
        myRopeList.Add(rope);
    }

    public void SetPlayer(PlayerMovement aPlayer)
    {
        myPlayer = aPlayer;
    }

    public void SetLifeTime(float aLifeTime)
    {
        if (myRopeList.Count > 0) 
        {
            myRopeBottom.transform.localPosition = myStartingLocalPosition;
            for (int i = 0; i < myRopeList.Count; i++)
            {
                Destroy(myRopeList[i]);
            }
        }
        myLifeTime = aLifeTime;
        myTimeStamp = Time.time;
    }

    public void DeSpawn()
    {
        myPlayer.SpawnSmoke(transform.position, 1f);
        myPlayer.StopClimb();
        myPlayer.DeactivateRopeGuy(this);
        gameObject.SetActive(false);

    }

    public float GetRopeLength()
    {
        return myMaxLength;
    }
}
