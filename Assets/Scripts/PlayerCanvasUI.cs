using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCanvasUI : MonoBehaviour
{
    [SerializeField] GameObject myPauseMenu;
    [SerializeField] TextMeshProUGUI myCurrentLevelText;
    [SerializeField] Image myCurrentSkillImage;
    [SerializeField] Sprite[] mySkillsSprites;
    Color myNoAlphaColor;
    Color myWithAlphaColor;

    [SerializeField] GameObject myGameFinishedScreen;
    [SerializeField] TextMeshProUGUI myLastText;

    void Start()
    {
        myNoAlphaColor = new Color(1f, 1f, 1f, 0f);
        myWithAlphaColor = new Color(1f, 1f, 1f, 1f);
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

    public void SetSkillImageUI(ESkills aSkill)
    {
        myCurrentSkillImage.color = myWithAlphaColor;
        myCurrentSkillImage.sprite = mySkillsSprites[(int)aSkill - 1];
    }

    public void ShowGameCompletedScreen()
    {
        Time.timeScale = 0f;
        myGameFinishedScreen.SetActive(true);
        myLastText.text = "Congratulations!\r\nYou died " + GameManager.Instance.GetDeathCount() + " times";
    }
}
