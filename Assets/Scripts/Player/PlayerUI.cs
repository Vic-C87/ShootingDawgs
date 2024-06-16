using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Sprite[] mySkillSpritePrefabs;
    [SerializeField] float myMaxDistance;
    [SerializeField] Sprite mySpriteFrame;
    [SerializeField] Image myImage;
    ECursorLocation myCursorLocation;


    void Awake()
    {
        myCursorLocation = ECursorLocation.None;
    }

    // Start is called before the first frame update
    void Start()
    {
        myImage.sprite = mySpriteFrame;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCursorPosition();

        if (Input.GetMouseButtonDown(0))
        {
            SetSkill();
        }
    }

    void SetSkill()
    {
        switch (myCursorLocation)
        {
            case ECursorLocation.None:
                mySpriteFrame = mySkillSpritePrefabs[0];
                break;
            case ECursorLocation.Top:
                mySpriteFrame = mySkillSpritePrefabs[1];
                break;
            case ECursorLocation.Right:
                mySpriteFrame = mySkillSpritePrefabs[2];
                break;
            case ECursorLocation.Bottom:
                mySpriteFrame = mySkillSpritePrefabs[3];
                break;
            case ECursorLocation.Left:
                mySpriteFrame = mySkillSpritePrefabs[4];
                break;
            default:
                mySpriteFrame = mySkillSpritePrefabs[0];
                break;
        }
    }

    void CheckCursorPosition()
    {
        Vector2 viewPortPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        viewPortPosition.x -= .5f;
        viewPortPosition.y -= .5f;

        float x = Mathf.Abs(viewPortPosition.x);
        float y = Mathf.Abs(viewPortPosition.y);

        float diagonal = Mathf.Sqrt(x*x + y*y);

        if (diagonal <= myMaxDistance)
        {
            if (y >= x)
            {
                if (viewPortPosition.y > 0)
                {
                    myImage.sprite = mySkillSpritePrefabs[1];
                    myCursorLocation = ECursorLocation.Top;
                }
                else
                {
                    myImage.sprite = mySkillSpritePrefabs[3];
                    myCursorLocation = ECursorLocation.Bottom;
                }
            }
            else
            {
                if (viewPortPosition.x > 0)
                {
                    myImage.sprite = mySkillSpritePrefabs[2];
                    myCursorLocation = ECursorLocation.Right;
                }
                else
                {
                    myImage.sprite = mySkillSpritePrefabs[4];
                    myCursorLocation = ECursorLocation.Left;
                }
            }


            //if (viewPortPosition.y > 0 && y >= x)
            //{
            //    myImage.sprite = mySkillSpritePrefabs[1];
            //    if (Input.GetMouseButtonDown(0)) 
            //    {
            //        mySpriteFrame = mySkillSpritePrefabs[1];
            //    }
            //}
            //else if (viewPortPosition.x > 0 && x > y)
            //{
            //    myImage.sprite = mySkillSpritePrefabs[2];
            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        mySpriteFrame = mySkillSpritePrefabs[2];
            //    }
            //}
            //else if (viewPortPosition.y < 0 && y >= x)
            //{
            //    myImage.sprite = mySkillSpritePrefabs[3];
            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        mySpriteFrame = mySkillSpritePrefabs[3];
            //    }
            //}     
            //else
            //{
            //    myImage.sprite = mySkillSpritePrefabs[4];
            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        mySpriteFrame = mySkillSpritePrefabs[4];
            //    }
            //}

        }
        else
        {
            myImage.sprite = mySpriteFrame;
        }
    }


    enum ECursorLocation
    {
        None,
        Top,
        Right,
        Bottom,
        Left
    }
}
