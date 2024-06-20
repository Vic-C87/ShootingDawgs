using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButtons : MonoBehaviour
{
    Button[] myButtons;

    private void Awake()
    {
        myButtons = GetComponentsInChildren<Button>();
    }

    private void Start()
    {
        int highestLevelUnlocked = GameManager.Instance.GetHighestLevelUnlocked();

        for (int i = highestLevelUnlocked; i < myButtons.Length; i++) 
        {
            Image[] image = myButtons[i].GetComponentsInChildren<Image>();
            Color color = image[1].color;
            color.a = 0.1f;
            image[1].color = color;
            myButtons[i].interactable = false;
        }
    }

    public void OnSelectLevel(int aLevelNumber)
    {
        GameManager.Instance.OnSelectLevel(aLevelNumber);
    }
}
