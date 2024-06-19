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
    bool myIsBats;

    int myRopeGuyCurrentSpawnIndex;

    float myJumpButtonPressedTimeStamp;
    float myJumpForce;
    float myXPositionWhenStartedClimb;
    float myPreviousYPosition;
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
    [SerializeField, Range(0,1)] float myGravityWhenBats;

    [SerializeField] float mySpeed;
    [SerializeField] float myShortJumpForce;
    [SerializeField] float myLongJumpForce;
    [SerializeField, Range(0, 1)] float mySlowDownTimeValue; 
    [SerializeField] LayerMask myGroundLayerMask;
    [SerializeField] PlayerUI mySkillSelectMenu;
    [SerializeField] int myPreLoadedRopeGuyAmount;

    [SerializeField] List<GameObject> mySummons;

    [SerializeField] GameObject mySummonsParent;

    GameObject myPreLoadedAnvil;

    List<RopeGuy> myPreLoadedRopeGuys;

    Animator myAnimator;
    EPlayerState myState;
    EPlayerState myPreviousState;

    float myCurrentRopeGuyYPosition;
    float myCurrentRopeLength;

    [SerializeField] GameObject mySmoke;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myIsGrounded = false;
        myIsFacingRight = true;
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
        GameObject anvil = Instantiate(mySummons[2], transform);
        anvil.SetActive(false);
        myPreLoadedAnvil = anvil;
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

    void FixedUpdate()
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
            switch (myState)
            {
                case EPlayerState.Idle:
                    myAnimator.SetBool("isBats", false);
                    myAnimator.SetBool("isDead", false);
                    myAnimator.SetBool("isLanding", false);
                    myAnimator.SetBool("isWalking", false);
                    myAnimator.SetBool("isIdle", true);                    
                    break;
                case EPlayerState.Run:
                    myAnimator.SetBool("isIdle", false);
                    myAnimator.SetBool("isWalking", true);
                    break;
                case EPlayerState.PrepareJump:
                    myAnimator.SetBool("isIdle", false);
                    myAnimator.SetBool("isWalking", false);
                    myAnimator.SetBool("isPreparingJump", true);
                    break;
                case EPlayerState.Jump:
                    myAnimator.SetBool("isPreparingJump", false);
                    myAnimator.SetBool("isJumping", true);
                    break;
                case EPlayerState.Fall:
                    myAnimator.SetBool("isBats", false);
                    myAnimator.SetBool("isJumping", false);
                    myAnimator.SetBool("isFalling", true);
                    break;
                case EPlayerState.Land:
                    myAnimator.SetBool("isFalling", false);
                    myAnimator.SetBool("isBats", false);
                    myAnimator.SetBool("isLanding", true); 
                    break;
                case EPlayerState.Bats:
                    myAnimator.SetBool("isJumping", false);
                    myAnimator.SetBool("isFalling", false);
                    myAnimator.SetBool("isBats", true);
                    break;
                case EPlayerState.Die:
                    myAnimator.SetBool("isLanding", false);
                    myAnimator.SetBool("isDead", true);
                    break;
                default:
                    break;
            }
            myPreviousState = myState;
        }
    }

    void CheckAirTime()
    {
        if (myHaveJumped && !myIsBats)
        {
            if (myPreviousYPosition > 0f && transform.position.y < myPreviousYPosition)
            {
                SetNewState(EPlayerState.Fall);
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
            }
            if (myIsBats) 
            {
                SetNewState(EPlayerState.Land);
                myAnimator.SetBool("isBats", false);
                myIsBats = false;
                myRigidbody.gravityScale = 1f;
            }
            if (myAnvilActive)
            {
                SetNewState(EPlayerState.Land);
                myPreLoadedAnvil.SetActive(false);
                SpawnSmoke(transform.position + Vector3.down * myAnvilOffSet, 1f);
                myAnvilActive = false;
            }
            myPreviousYPosition = 0f;
        }
        else
        {
            myIsGrounded = false;
            if (!myIsClimbing && !myIsBats)
            {
                myHaveJumped = true;
            }
        }

        if (Mathf.Abs(transform.position.x - myXPositionWhenStartedClimb) > 1f && !myIsBats)
        {
            StopClimb();
        }
    }

    public void SetNewState(EPlayerState aNewState)//Is 'public' needed?
    {
        myPreviousState = myState;
        myState = aNewState;
    }

    public void StopClimb()
    {
        if (!myIsBats)
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
        Vector2 tempYPosition = transform.position;
        if (transform.position.y >= myCurrentRopeGuyYPosition)//  && 
        {
            tempYPosition.y -= .1f;
            transform.position = tempYPosition;
        }
        else if (transform.position.y <= myCurrentRopeGuyYPosition - myCurrentRopeLength)
        {
            tempYPosition.y += .1f;
            transform.position = tempYPosition;
        }
        transform.Translate(myMovementVector * mySpeed / 2 * Time.fixedDeltaTime);   
    }

    void Movement()
    {
        if (myIsGrounded || myIsBats)
        {
            myMovementVector.y = 0f;

            transform.Translate(myMovementVector * mySpeed * Time.fixedDeltaTime);
        }
    }

    void Jump()
    {
        if (myShouldJump)
        {
            SetNewState(EPlayerState.Jump);
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
        GameObject smoke = Instantiate(mySmoke, aPosition, Quaternion.identity);
        Destroy(smoke, aDuration);
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
        myPreLoadedAnvil.SetActive(true);
        myPreLoadedAnvil.transform.position = transform.position + Vector3.down * myAnvilOffSet;
        SpawnSmoke(transform.position + Vector3.down * myAnvilOffSet, 1f);
        myRigidbody.velocity = Vector2.zero;
        myRigidbody.AddForce(Vector2.down * myAnvilPullDownForce, ForceMode2D.Impulse);
        myAnvilActive = true;
    }

    void ActivatePowerPunch(float aDirection)
    {
        GameObject puncher = Instantiate(mySummons[3], transform.position + (transform.right * myPuncherOffSet * aDirection), transform.rotation);
        SpawnSmoke(transform.position + (transform.right * myPuncherOffSet * aDirection), 1f);
        Vector3 tempScale = puncher.transform.localScale;
        tempScale.x *= -aDirection;
        puncher.transform.localScale = tempScale;
        Vector2 direction = new Vector2(.5f, .5f);
        direction.Normalize();
        direction.x *= -aDirection;
        myRigidbody.AddForce(direction * myPowerPunchForce, ForceMode2D.Impulse);
        Destroy(puncher, 1f);

    }

    void ActivateBats()
    {
        myIsBats = true;
        myRigidbody.gravityScale = myGravityWhenBats;
        myRigidbody.velocity = Vector2.zero;
        SetNewState(EPlayerState.Bats);
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
            SetNewState(EPlayerState.Run);
        }

        if (aCallbackContext.phase == InputActionPhase.Canceled)
        {
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
            SetNewState(EPlayerState.PrepareJump);
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
                switch (mySelectedSkill)
                {
                    case ESkills.None:
                        break;
                    case ESkills.Rope:
                        ActivateRopeGuy(mousePosition);
                        break;
                    case ESkills.Float:
                        ActivateBats();
                        break;
                    case ESkills.Anvil:
                        ActivateAnvil();
                        break;                  
                    case ESkills.PowerPunch:
                        float facingDirection = myIsFacingRight ? -1 : 1;
                        ActivatePowerPunch(facingDirection);
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
            myIsBats = false;
            myRigidbody.gravityScale = 0f;
            myRigidbody.velocity = Vector2.zero;
            myXPositionWhenStartedClimb = collision.GetComponent<Transform>().position.x;
            RopeGuy guy = collision.GetComponentInParent<Transform>().GetComponentInParent<RopeGuy>();
            myCurrentRopeGuyYPosition = guy.transform.position.y;
            myCurrentRopeLength = guy.GetRopeLength();
        }
    }
}
