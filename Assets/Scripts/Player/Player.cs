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
    [SerializeField] float jumpupValue;
    [SerializeField] float jumpSideValue;

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
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {

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

    void Jump()
    {
        if (shouldJump) 
        {
            shouldMove = false;
            Vector2 jumpDirection = new Vector2(0.5f, 0.5f);
            jumpDirection.Normalize();
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse); 

            shouldJump = false;
        }
    }

    public void OnMovement(InputAction.CallbackContext aCallbackContext)
    {
        myMovementVector = aCallbackContext.ReadValue<Vector2>();

        if (myMovementVector != Vector2.zero)
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
