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
    [SerializeField] Image myImageSelected;

    [SerializeField] Player myPlayer;
    ECursorLocation myCursorLocation;
    ESkills mySelectedSkill;


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
                mySelectedSkill = ESkills.None;
                break;
            case ECursorLocation.Top:
                mySpriteFrame = mySkillSpritePrefabs[1];
                mySelectedSkill = ESkills.Rope;
                break;
            case ECursorLocation.Right:
                mySpriteFrame = mySkillSpritePrefabs[2];
                mySelectedSkill = ESkills.Float;
                break;
            case ECursorLocation.Bottom:
                mySpriteFrame = mySkillSpritePrefabs[3];
                mySelectedSkill = ESkills.Anvil;
                break;
            case ECursorLocation.Left:
                mySpriteFrame = mySkillSpritePrefabs[4];
                mySelectedSkill = ESkills.PowerPunch;
                break;
            default:
                mySpriteFrame = mySkillSpritePrefabs[0];
                mySelectedSkill = ESkills.None;
                break;
        }
        myImageSelected.sprite = mySpriteFrame;
        myPlayer.SetSelectedSkill(mySelectedSkill);
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
        }
        else
        {
            myImageSelected.sprite = mySpriteFrame;
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
