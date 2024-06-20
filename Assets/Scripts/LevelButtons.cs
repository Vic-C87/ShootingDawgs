using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButtons : MonoBehaviour
{

    public void OnSelectLevel(int aLevelNumber)
    {
        GameManager.Instance.OnSelectLevel(aLevelNumber);
    }
}
