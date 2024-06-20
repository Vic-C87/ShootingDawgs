using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCanvasUI : MonoBehaviour
{
    [SerializeField] GameObject myPauseMenu;
    [SerializeField] TextMeshProUGUI myCurrentLevelText;

    void Start()
    {
        myPauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayPauseScreen()
    {
        myPauseMenu.SetActive(true);
    }

    public void UnPause()
    {
        LevelManager.Instance.OnPauseGame();
    }

    public void HidePauseMenu()
    {
        myPauseMenu.SetActive(false);
    }

    public void ReturnToLevelSelect()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    public void SetLevelText(int aCurrentLevel)
    {
        myCurrentLevelText.text = "Level: " + aCurrentLevel;
    }
}
