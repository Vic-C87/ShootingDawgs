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
    [SerializeField] float distanceToGround;
    [SerializeField, Range(0,1)] float mySlowDownSpeed;
    [SerializeField] PlayerUI mySkillSelect;

    bool shouldMove;
    bool shouldJump;
    Vector2 myMovementVector;
    float timeStamp;
    Rigidbody2D rb;

    ESkills mySelectedSkill;
    
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
        if (myMovementVector.x > 0)
        {
            jumpSideValue = Mathf.Abs(jumpSideValue);
            //Set sprite direction (scale * -1)
        }
        if (myMovementVector.x < 0)
        {
            jumpSideValue = Mathf.Abs(jumpSideValue) * -1;
        }
    }

    private void FixedUpdate()
    {
        Movement();
        Jump();
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
        if (Physics2D.Raycast(leftSideRay, Vector2.down, distanceToGround) || Physics2D.Raycast(rightSideRay, Vector2.down, distanceToGround))
        {
            if (myMovementVector != Vector2.zero)
            {
                shouldMove = true;
            }
            return true;
        }

        return false;
    }

    void Jump()
    {
        if (shouldJump && IsOnGround()) 
        {
            shouldJump = false;
            Vector2 jumpDirection = new Vector2(jumpSideValue, jumpUpValue);
            jumpDirection.Normalize();
            rb.velocity = Vector2.zero;
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        }

        //when landed set vector2 = 0;
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
        }

        if (aCallbackContext.phase == InputActionPhase.Canceled)
        {
            mySkillSelect.gameObject.SetActive(false);
            Time.timeScale = 1f;
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
#endregion
}
