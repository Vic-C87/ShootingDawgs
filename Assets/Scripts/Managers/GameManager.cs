using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] Sprite myCursor;
    [SerializeField] Image myCursorGameObject;
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

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        myCursorGameObject.sprite = myCursor;
        myLevelSelectScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCursorPosition();
    }

    void UpdateCursorPosition()
    {
        myCursorGameObject.transform.position = Input.mousePosition;
    }

    public void OnPressStart()
    {
        myMainScreen.SetActive(false);
        myLevelSelectScreen.SetActive(true);

    }

    public void OnSelectLevel(int aLevelNumber)
    {
        if (myUnlockedLevelIndex <= aLevelNumber)
        {
            SceneManager.LoadScene(aLevelNumber);
        }
    }

    public void OnPressQuit()
    {
        Application.Quit();
    }
}
