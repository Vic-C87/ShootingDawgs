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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveRopeDown();
    }

    void MoveRopeDown()
    {
        if (myRopeSpawnPoint.position.y - myRopeBottom.transform.position.y < myMaxLength) 
        {
            myRopeBottom.transform.position += Vector3.down * myRopeSpeed * Time.deltaTime;
        }
        //if rope is not longer then myLenght, move rope down
    }

    public void SpawnNewRopePiece()
    {
        GameObject ropePiece = Instantiate<GameObject>(myRopePrefab, myRopeSpawnPoint.position, Quaternion.identity, myRopeBottom.transform);
    }
}
