using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject myLevelSelectScreen;
    [SerializeField] GameObject myMainScreen;


    int myUnlockedLevelIndex;

    private void Awake()
    {
        if (Instance != this && Instance !=null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        myUnlockedLevelIndex = 1;
    }

    void Start()
    {
        myMainScreen.SetActive(true);
        myLevelSelectScreen.SetActive(false);
    }

    public void OnPressStart()
    {
        Debug.Log("Start!");
        myMainScreen.SetActive(false);
        myLevelSelectScreen.SetActive(true);
    }

    public void OnSelectLevel(int aLevelNumber)
    {
        if (myUnlockedLevelIndex >= aLevelNumber)
        {
            SceneManager.LoadScene(aLevelNumber);
        }
    }

    public void ReturnToLevelSelect(int aLevelToUnlock)
    {
        myUnlockedLevelIndex = aLevelToUnlock;
        SceneManager.LoadScene(0);
        
    }

    public void OnPressQuit()
    {
        Application.Quit();
    }
}
