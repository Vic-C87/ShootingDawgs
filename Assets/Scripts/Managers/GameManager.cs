using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject myLevelSelectScreen;
    [SerializeField] GameObject myMainScreen;


    int myUnlockedLevelIndex;

    int myDeathCount;
    int myCurrentLevel;

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
        myDeathCount = 0;
    }

    void Start()
    {
        myMainScreen.SetActive(true);
        myLevelSelectScreen.SetActive(false);
    }

    public void OnPressStart()
    {
        myMainScreen.SetActive(false);
        myLevelSelectScreen.SetActive(true);
    }

    public void OnSelectLevel(int aLevelNumber)
    {
        if (myUnlockedLevelIndex >= aLevelNumber)
        {
            SceneManager.LoadScene(aLevelNumber);
            myCurrentLevel = aLevelNumber;
        }
    }

    public int GetCurrentLevel()
    {
        return myCurrentLevel;
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

    public int GetHighestLevelUnlocked()
    {
        return myUnlockedLevelIndex;
    }

    public void IncreaseDeathCount()
    {
        myDeathCount++;
        FindObjectOfType<Canvas>().GetComponentInChildren<TextMeshProUGUI>().text = "Deaths: " +  myDeathCount;
    }

    public int GetDeathCount()
    {
        return myDeathCount;
    }
}
