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

    bool isFacingRight = true;
    bool shouldMove;
    bool shouldJump;
    Vector2 myMovementVector;
    float timeStamp;
    Rigidbody2D rb;
    
    void Awake () 
    { 
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        jumpSideValue *= -1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (Input.GetAxisRaw("Horizontal") > 0.5)
        {
            isFacingRight = true;
            Debug.Log("I'm facing right");
        }
        if (Input.GetAxisRaw("Horizontal") < -0.5)
        {
            isFacingRight = false;
            Debug.Log("I'm not");
        }

        Movement();
        Jump();
    }

    void Movement()
    {
        if (shouldMove)
        {
            myMovementVector.y = 0f;
            transform.Translate(myMovementVector * speed * Time.fixedDeltaTime);
        }
    }

    bool IsOnGround()
    {
        Vector2 leftSideRay = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f);
        Vector2 rightSideRay = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f);

        return Physics2D.Raycast(leftSideRay, Vector2.down, distanceToGround) || Physics2D.Raycast(rightSideRay, Vector2.down, distanceToGround);
    }

    void Jump()
    {
        if (shouldJump && IsOnGround()) 
        {
            shouldMove = false;

            if(!isFacingRight)
            {
                jumpSideValue *= -1;
            }
            else
            {
                jumpSideValue *= -1;
            }

            Vector2 jumpDirection = new Vector2(jumpSideValue, jumpUpValue);
            jumpDirection.Normalize();
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse); 

            shouldJump = false;
        }

        //when landed set vector2 = 0;
    }

    public void OnMovement(InputAction.CallbackContext aCallbackContext)
    {
        myMovementVector = aCallbackContext.ReadValue<Vector2>();

        if (myMovementVector != Vector2.zero && IsOnGround())
        {
            shouldMove = true;
        }
        else shouldMove = false;

        Debug.Log(myMovementVector);
    }

    public void OnMenuPress(InputAction.CallbackContext aCallbackContext) 
    {

    }

    public void onJump(InputAction.CallbackContext aCallbackContext)
    {
        if (aCallbackContext.phase == InputActionPhase.Started) 
        {
            timeStamp = Time.time;
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
}
