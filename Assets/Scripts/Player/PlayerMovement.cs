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
    bool myAnvilActive;
    bool myIsMoving;
    bool myIsFalling;
    bool myIsDead;

    int myRopeGuyCurrentSpawnIndex;

    float myJumpButtonPressedTimeStamp;
    float myJumpForce;
    float myXPositionWhenStartedClimb;
    float myPreviousYPosition;
    [SerializeField, Range(0,90)] float myJumpAngle;
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
    [SerializeField] int myPreLoadedRopeGuyAmount;

    [SerializeField] List<GameObject> mySummons;

    [SerializeField] GameObject mySummonsParent;

    Dictionary<ESummons, GameObject> myPreLoadedSummons;

    List<RopeGuy> myPreLoadedRopeGuys;

    Animator myAnimator;
    [SerializeField] EPlayerState myState;
    [SerializeField] EPlayerState myPreviousState;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myIsGrounded = false;
        myIsFacingRight = true;
        myPreLoadedSummons = new Dictionary<ESummons, GameObject>();
        myPreLoadedRopeGuys = new List<RopeGuy>();
        myRopeGuyCurrentSpawnIndex = 0;
        myAnimator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        myPreviousState = EPlayerState.None;
        myState = EPlayerState.Idle;
        myJumpingDirection.Normalize();
        mySkillSelectMenu.gameObject.SetActive(false);
        for (int i = 1; i < mySummons.Count; i++)
        {
            GameObject summon = Instantiate(mySummons[i], mySummonsParent.transform);
            summon.SetActive(false);
            myPreLoadedSummons.Add((ESummons)i, summon);
        }
        myPreLoadedSummons[ESummons.Anvil].transform.parent = transform;
        for (int i = 0; i < myPreLoadedRopeGuyAmount; i++)
        {
            GameObject summon = Instantiate(mySummons[0], mySummonsParent.transform);
            RopeGuy guy = summon.GetComponent<RopeGuy>();
            guy.SetPlayer(this);
            summon.SetActive(false);
            myPreLoadedRopeGuys.Add(guy);
        }
    }

    void Update()
    {
        CheckForGround();
        CheckFacingDirection();
        CheckAirTime();
        SetAnimator();
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

    void SetAnimator()
    {
        if (myPreviousState != myState)
        {
            ResetAnimatorBools();
            switch (myState)
            {
                case EPlayerState.Idle:
                    myAnimator.SetBool("isIdle", true);                    
                    break;
                case EPlayerState.Run:
                    myAnimator.SetBool("isWalking", true);
                    break;
                case EPlayerState.Jump:
                    myAnimator.SetBool("isJumping", true);
                    break;
                case EPlayerState.Fall:
                    myAnimator.SetBool("isFalling", true);
                    break;
                case EPlayerState.Land:
                    myAnimator.SetBool("isLanding", true); 
                    break;
                case EPlayerState.Die:
                    myAnimator.SetBool("isDead", true);
                    break;
                default:
                    break;
            }
            myPreviousState = myState;
        }
    }

    void ResetAnimatorBools()
    {
        myAnimator.SetBool("isIdle", false);
        myAnimator.SetBool("isWalking", false);
        myAnimator.SetBool("isJumping", false);
        myAnimator.SetBool("isFalling", false);
        myAnimator.SetBool("isBats", false);
        myAnimator.SetBool("isDead", false);
    }

    void CheckAirTime()
    {
        if (myHaveJumped)
        {
            if (myPreviousYPosition > 0f && transform.position.y < myPreviousYPosition)
            {
                SetNewState(EPlayerState.Fall);
                myIsFalling = true;
            }
            else
            {
                myPreviousYPosition = transform.position.y;
            }
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
                SetNewState(EPlayerState.Land);
                myRigidbody.velocity = Vector2.zero;
                myHaveJumped = false;
                myIsClimbing = false;
                myIsFalling = false;
            }
            if (myAnvilActive)
            {
                myPreLoadedSummons[ESummons.Anvil].SetActive(false);
                SpawnSmoke(transform.position + Vector3.down * myAnvilOffSet, 1f);
                myAnvilActive = false;
            }
            myPreviousYPosition = 0f;
        }
        else
        {
            myIsGrounded = false;
            if (!myIsClimbing)
            {
                SetNewState(EPlayerState.Jump);
                myHaveJumped = true;
            }
        }

        if (Mathf.Abs(transform.position.x - myXPositionWhenStartedClimb) > 1f) //Fixa!!! Logga y pos och längd och gör check för de här
        {
            StopClimb();
        }
    }

    public void SetNewState(EPlayerState aNewState)
    {
        myPreviousState = myState;
        myState = aNewState;
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

    public void SpawnSmoke(Vector3 aPosition, float aDuration)
    {

    }

    void ActivateRopeGuy(Vector2 aPosition)
    {
        myPreLoadedRopeGuys[myRopeGuyCurrentSpawnIndex].gameObject.SetActive(true);
        myPreLoadedRopeGuys[myRopeGuyCurrentSpawnIndex].transform.position = aPosition;
        SpawnSmoke(aPosition, 1f);
        myPreLoadedRopeGuys[myRopeGuyCurrentSpawnIndex].SetLifeTime(myRopeGuyLifeTime);

        myRopeGuyCurrentSpawnIndex++;
        if (myRopeGuyCurrentSpawnIndex >= myPreLoadedRopeGuys.Count)
        {
            myRopeGuyCurrentSpawnIndex = 0;
        }
    }

    void ActivateAnvil()
    {
        myPreLoadedSummons[ESummons.Anvil].SetActive(true);
        myPreLoadedSummons[ESummons.Anvil].transform.position = transform.position + Vector3.down * myAnvilOffSet;
        SpawnSmoke(transform.position + Vector3.down * myAnvilOffSet, 1f);
        myRigidbody.velocity = Vector2.zero;
        myRigidbody.AddForce(Vector2.down * myAnvilPullDownForce, ForceMode2D.Impulse);
        myAnvilActive = true;
    }

    void ActivatePowerPunch(float aDirection)
    {
        myPreLoadedSummons[ESummons.Puncher].SetActive(true);
        myPreLoadedSummons[ESummons.Puncher].transform.position = transform.position + (transform.right * myPuncherOffSet * aDirection);
        Vector2 direction = new Vector2(.5f, .5f);
        direction.Normalize();
        direction.x *= -aDirection;
        myRigidbody.AddForce(direction * myPowerPunchForce, ForceMode2D.Impulse);
    }

    public void SetSelectedSkill(ESkills aSelectedSkill)
    {
        mySelectedSkill = aSelectedSkill;
    }

#region Input

    public void OnMovement(InputAction.CallbackContext aCallbackContext)
    {
        myMovementVector = aCallbackContext.ReadValue<Vector2>();
        if (aCallbackContext.phase == InputActionPhase.Started)
        {
            myIsMoving = true;
            SetNewState(EPlayerState.Run);
        }

        if (aCallbackContext.phase == InputActionPhase.Canceled)
        {
            myIsMoving = false;
            SetNewState(EPlayerState.Idle);
        }
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
    {
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
                        ActivateRopeGuy(mousePosition);
                        break;
                    case ESkills.Float:
                        summon = Instantiate<GameObject>(mySummons[1], mousePosition, Quaternion.identity);//FIX!!!
                        Destroy(summon, 5f);//FIX!!!
                        break;
                    case ESkills.Anvil:
                        ActivateAnvil();
                        break;                  
                    case ESkills.PowerPunch:
                        float facingDirection = myIsFacingRight ? -1 : 1;
                        ActivatePowerPunch(facingDirection);
                        //Despawn time, flip puncher sprite
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
