using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] float smallJump;
    [SerializeField] float largeJump;
    [SerializeField] float jumpTimeThreshold;
    [SerializeField] float jumpForce;
    [SerializeField] float speed;
    [SerializeField] float jumpUpValue;
    [SerializeField] float jumpSideValue;
    [SerializeField, Range(0,1)] float mySlowDownSpeed;
    [SerializeField] PlayerUI mySkillSelect;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] bool isClimbing;
    bool isFacingRight;
    bool shouldMove;
    bool shouldJump;
    bool haveJumped;
    bool skillsMenuIsOpen;
    Vector2 myMovementVector;
    float timeStamp;
    float distanceToGround = 0.01f;
    Rigidbody2D rb;

    ESkills mySelectedSkill;

    Vector2 myClickedPosition;

    [SerializeField] GameObject[] mySummons;
    
    void Awake () 
    { 
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        mySkillSelect.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (myMovementVector.x > 0 && isFacingRight)
        {
            jumpSideValue = Mathf.Abs(jumpSideValue);
            Flip();
        }
        if (myMovementVector.x < 0 && !isFacingRight)
        {
            jumpSideValue = Mathf.Abs(jumpSideValue) * -1;
            Flip();
        }
        if (isClimbing)
        {
        }

    }

    private void FixedUpdate()
    {
        Movement();
        Jump();
    }
void Flip()
    {
        Vector2 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        isFacingRight = !isFacingRight;
    }

    void Movement()
    {
        if (IsOnGround() && shouldMove)
        {
            myMovementVector.y = 0f;
            transform.Translate(myMovementVector * speed * Time.fixedDeltaTime);
        }
    }

    bool IsOnGround()
    {
        Vector2 leftSideRay = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f);
        Vector2 rightSideRay = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f);
        if (Physics2D.Raycast(leftSideRay, Vector2.down, distanceToGround, groundLayer) || Physics2D.Raycast(rightSideRay, Vector2.down, distanceToGround, groundLayer))
        {
            if (myMovementVector != Vector2.zero)
            {
                shouldMove = true;
            }
            if (haveJumped)
            {
                haveJumped = false;
                rb.velocity = Vector2.zero;
            }
            return true;
        }
        return false;
    }

    void Jump()
    {
        if ((shouldJump && IsOnGround()) || (isClimbing && shouldJump)) 
        {
            shouldJump = false;
            isClimbing = false;
            rb.gravityScale = 1f;
            Vector2 jumpDirection = new Vector2(jumpSideValue, jumpUpValue);
            jumpDirection.Normalize();
            rb.velocity = Vector2.zero;
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        }
        else
        {
            jumpForce = 0;
        }
    }

    public void SetSelectedSkill(ESkills aSkill)
    {
        mySelectedSkill = aSkill;
    }

#region Input
    public void OnMovement(InputAction.CallbackContext aCallbackContext)
    {
        myMovementVector = aCallbackContext.ReadValue<Vector2>();

        if (aCallbackContext.phase == InputActionPhase.Started && IsOnGround())
        {
            shouldMove = true;
        }

        if (aCallbackContext.phase == InputActionPhase.Performed && IsOnGround())
        {
            shouldMove = true;
        }

        if (aCallbackContext.phase == InputActionPhase.Canceled)
        {
            myMovementVector = Vector2.zero;
            shouldMove = false;

        }
    }

    public void OnSkillSelect(InputAction.CallbackContext aCallbackContext) 
    {
        if (aCallbackContext.phase == InputActionPhase.Started)
        {
            mySkillSelect.gameObject.SetActive(true);
            Time.timeScale = mySlowDownSpeed;
            skillsMenuIsOpen = true;
        }

        if (aCallbackContext.phase == InputActionPhase.Canceled)
        {
            mySkillSelect.gameObject.SetActive(false);
            Time.timeScale = 1f;
            skillsMenuIsOpen = false;
        }
    }

    public void OnJump(InputAction.CallbackContext aCallbackContext)
    {
        if (aCallbackContext.phase == InputActionPhase.Started) 
        {
            timeStamp = Time.time;
            shouldMove = false;
        }

        if (aCallbackContext.phase == InputActionPhase.Canceled)
        {
            
            if (Time.time - timeStamp >= jumpTimeThreshold)
            {
                jumpForce = largeJump;
            }
            else
            {
                jumpForce = smallJump;
            }
            
            shouldJump = true;            
        }
    }

    public void OnSummon(InputAction.CallbackContext aCallbackContext)
    {
        if (!skillsMenuIsOpen)
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
                        Destroy(summon, 20f);//FIX!!!
                        break;
                    case ESkills.Anvil:
                        summon = Instantiate<GameObject>(mySummons[1], transform.position + Vector3.down, Quaternion.identity, transform);
                        ActivateAnvil();
                        Destroy(summon, 5f);//FIX!!!
                        break;
                    case ESkills.Float:
                        summon = Instantiate<GameObject>(mySummons[2], mousePosition, Quaternion.identity);
                        Destroy(summon, 5f);//FIX!!!
                        break;
                    case ESkills.PowerPunch:
                        float facingDirection = jumpSideValue > 0 ? -1 : 1;
                        summon = Instantiate<GameObject>(mySummons[3], transform.position + transform.right * facingDirection, Quaternion.identity, transform);
                        ActivatePowerPunch(facingDirection);
                        Destroy(summon, 5f);//FIX!!!
                        break;
                    default:
                        break;
                }
                

            }
        }
    }
#endregion

    void ActivateAnvil()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.down * 10, ForceMode2D.Impulse);
    }

    void ActivatePowerPunch(float aDirection)
    {
        Vector2 direction = new Vector2(.5f, .5f);
        direction.Normalize();
        direction.x *= -aDirection;
        rb.AddForce(direction * 10, ForceMode2D.Impulse);
    }

    void ActivateRope()
    {

    }

    void ActivateBats()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope") && !isClimbing)
        {
            rb.velocity = Vector2.zero;
            isClimbing = true;
            haveJumped = false;
            rb.gravityScale = 0f;
        }
    }
}
