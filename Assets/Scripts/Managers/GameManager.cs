using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] Sprite myCursor;

    [SerializeField] Image myCursorGameObject;


    private void Awake()
    {
        if (Instance != this && Instance !=null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        myCursorGameObject.sprite = myCursor;
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
}
