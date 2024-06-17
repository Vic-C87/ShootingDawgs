using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    ESkills mySelectedSkill;
    Rigidbody2D myRigidbody;
    Vector2 myMovementVector;
    [SerializeField] Vector2 myJumpingDirection;
    
    bool myIsGrounded;
    bool myShouldJump;
    bool myHaveJumped;
    bool myIsFacingRight;
    bool mySkillsMenuIsOpen;
    bool myIsClimbing;

    float myJumpButtonPressedTimeStamp;
    float myJumpForce;
    float myXPositionWhenStartedClimb;

    [SerializeField] float myRopeGuyLifeTime;
    [SerializeField] float myHalfWidth;
    [SerializeField] float myHalfHeight;
    [SerializeField] float myGroundDistanceCheck;
    [SerializeField] float myLongJumpHoldTreshold;
    [SerializeField] float myPowerPunchForce;
    [SerializeField] float myAnvilPullDownForce;
    [SerializeField] float myAnvilOffSet;
    [SerializeField] float myPuncherOffSet;
    [SerializeField] float myPuncherLifeTime;

    [SerializeField] float mySpeed;
    [SerializeField] float myShortJumpForce;
    [SerializeField] float myLongJumpForce;
    [SerializeField, Range(0, 1)] float mySlowDownTimeValue; 
    [SerializeField] LayerMask myGroundLayerMask;
    [SerializeField] PlayerUI mySkillSelectMenu;

    [SerializeField] List<GameObject> mySummons;

    

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myIsGrounded = false;
        myIsFacingRight = true;
    }

    void Start()
    {
        myJumpingDirection.Normalize();
        mySkillSelectMenu.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckForGround();
        CheckFacingDirection();
    }

    private void FixedUpdate()
    {
        if (!myIsClimbing)
        {
            Movement();
            Jump();
        }
        else
        {
            Climb();
        }
    }

    void CheckForGround()
    {
        Vector2 leftSide = new Vector2(transform.position.x - myHalfWidth, transform.position.y - myHalfHeight);
        Vector2 rightSide = new Vector2(transform.position.x + myHalfWidth, transform.position.y - myHalfHeight);

        if (Physics2D.Raycast(leftSide, Vector2.down, myGroundDistanceCheck, myGroundLayerMask) || Physics2D.Raycast(rightSide, Vector2.down, myGroundDistanceCheck, myGroundLayerMask))
        {
            myIsGrounded = true;
            if (myHaveJumped)
            {
                myRigidbody.velocity = Vector2.zero;
                myHaveJumped = false;
                myIsClimbing = false;
            }
        }
        else
        {
            myIsGrounded = false;
            if (!myIsClimbing)
            {
                myHaveJumped = true;
            }
        }

        if (Mathf.Abs(transform.position.x - myXPositionWhenStartedClimb) > 1f)
        {
            StopClimb();
        }
    }

    public void StopClimb()
    {
        myRigidbody.gravityScale = 1f;
        myIsClimbing = false;
    }

    void CheckFacingDirection()
    {
        if (myMovementVector.x > 0 && !myIsFacingRight)
        {
            myIsFacingRight = true;
            Vector2 currentScale = transform.localScale;
            currentScale.x *= -1;
            transform.localScale = currentScale;
        }
        else if (myMovementVector.x < 0 && myIsFacingRight)
        {
            myIsFacingRight = false;
            Vector2 currentScale = transform.localScale;
            currentScale.x *= -1;
            transform.localScale = currentScale;
        }
    }

    void Climb()
    {
        transform.Translate(myMovementVector * mySpeed / 2 * Time.fixedDeltaTime);
    }

    void Movement()
    {
        if (myIsGrounded)
        {
            myMovementVector.y = 0f;

            transform.Translate(myMovementVector * mySpeed * Time.fixedDeltaTime);
        }
    }

    void Jump()
    {
        if (myShouldJump)
        {
            myShouldJump = false;
            if (myIsFacingRight)
            {
                myJumpingDirection.x = Mathf.Abs(myJumpingDirection.x);
            }
            else
            {
                myJumpingDirection.x = Mathf.Abs(myJumpingDirection.x) * -1;
            }
            myRigidbody.velocity = Vector2.zero;
            myRigidbody.AddForce(myJumpingDirection * myJumpForce, ForceMode2D.Impulse);
        }
    }

    void ActivateAnvil()
    {
        myRigidbody.velocity = Vector2.zero;
        myRigidbody.AddForce(Vector2.down * myAnvilPullDownForce, ForceMode2D.Impulse);
    }

    void ActivatePowerPunch(float aDirection)
    {
        Vector2 direction = new Vector2(.5f, .5f);
        direction.Normalize();
        direction.x *= -aDirection;
        myRigidbody.AddForce(direction * myPowerPunchForce, ForceMode2D.Impulse);
    }

    public void SetSelectedSkill(ESkills aSelectedSkill)
    {
        mySelectedSkill = aSelectedSkill;
    }

    public void TriggerCameraDeadZone()
    {

    }

#region Input

    public void OnMovement(InputAction.CallbackContext aCallbackContext)
    {
        myMovementVector = aCallbackContext.ReadValue<Vector2>();
    }

    public void OnOpenSkillMenu(InputAction.CallbackContext aCallbackContext)
    {
        if (aCallbackContext.phase == InputActionPhase.Started)
        {
            mySkillSelectMenu.gameObject.SetActive(true);
            Time.timeScale = mySlowDownTimeValue;
            mySkillsMenuIsOpen = true;
        }

        if (aCallbackContext.phase == InputActionPhase.Canceled)
        {
            mySkillSelectMenu.gameObject.SetActive(false);
            Time.timeScale = 1;
            mySkillsMenuIsOpen = false;
        }
    }

    public void OnJump(InputAction.CallbackContext aCallbackContext)
    {
        if (aCallbackContext.phase == InputActionPhase.Started)
        {
            myJumpButtonPressedTimeStamp = Time.time;
        }

        if (aCallbackContext.phase == InputActionPhase.Canceled && myIsGrounded)
        {
            if (Time.time - myJumpButtonPressedTimeStamp > myLongJumpHoldTreshold)
            {
                myJumpForce = myLongJumpForce;
            }
            else
            {
                myJumpForce = myShortJumpForce;
            }

            myShouldJump = true;
        }
    }

    public void OnSummon(InputAction.CallbackContext aCallbackContext)
    { //Poola GameObjects: Ta bort Anvil n'r landar: lägg in smoke
        if (!mySkillsMenuIsOpen)
        {
            if (aCallbackContext.phase == InputActionPhase.Canceled)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameObject summon;
                switch (mySelectedSkill)
                {
                    case ESkills.None:
                        break;
                    case ESkills.Rope:
                        summon = Instantiate<GameObject>(mySummons[0], mousePosition, Quaternion.identity);
                        summon.GetComponent<RopeGuy>().SetPlayer(this);
                        Destroy(summon, myRopeGuyLifeTime);
                        break;
                    case ESkills.Float:
                        summon = Instantiate<GameObject>(mySummons[1], mousePosition, Quaternion.identity);
                        Destroy(summon, 5f);//FIX!!!
                        break;
                    case ESkills.Anvil:
                        summon = Instantiate<GameObject>(mySummons[2], transform.position + (Vector3.down * myAnvilOffSet), Quaternion.identity, transform);
                        ActivateAnvil();
                        Destroy(summon, 5f);//FIX!!!
                        break;                  
                    case ESkills.PowerPunch:
                        float facingDirection = myIsFacingRight ? -1 : 1;
                        summon = Instantiate<GameObject>(mySummons[3], transform.position + (transform.right * myPuncherOffSet * facingDirection), Quaternion.identity);
                        ActivatePowerPunch(facingDirection);
                        Destroy(summon, myPuncherLifeTime);//FIX!!!
                        break;
                    default:
                        break;
                }


            }
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!myIsClimbing && collision.CompareTag("Rope")) 
        {
            myIsClimbing = true;
            myRigidbody.gravityScale = 0f;
            myRigidbody.velocity = Vector2.zero;
            myXPositionWhenStartedClimb = collision.GetComponent<Transform>().position.x;
        }
    }
}
