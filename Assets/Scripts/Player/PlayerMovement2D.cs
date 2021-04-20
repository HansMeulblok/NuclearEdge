using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    //Horizontal movement variables
    [Header("Horizontal variables")]
    public float accel;
    public float decel;
    public float airDecel;
    float tempDecel;
    public float maxSpeed;

    //Vertical movement variables
    [Header("Vertical variables")]
    public float jumpStrenght;
    public float jumpBufferLenght;
    public float wallJumpHorizontal;
    public float wallJumpVertical;

    [Header("Gravity variables")]
    public float gravity;
    public float gravZoneMult;
    public float upGravMult;
    public float downGravMult;
    public float wallGravMult;
    float tempGrav;

    [Header("Walljump variables")]
    public float clingDuration;
    public float maxDownSlideSpeed;
    public float wallJumpBuffer;
    float jumpBuffer;
    float onLeftWallCling;
    float onRightWallCling;
    float wallJumpBufferL;
    float wallJumpBufferR;

    //Collisions
    [Header("Collision")]
    public float colisionDistance;
    bool grounded;
    bool leftCol;
    bool rightCol;

    //Input
    bool upPressed;
    bool upHold;
    bool leftPressed;
    bool leftHold;
    bool rightPressed;
    bool rightHold;


    //Global variables
    Vector3 moveSpeed;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        //Get the rigidbody
        rb = GetComponent<Rigidbody2D>();
        //Reset movespeed on start
        moveSpeed = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        InputCheck();
    }

    // FixedUpdate is called at a fixed interval
    void FixedUpdate()
    {
        //Check for colisions
        CheckColision();
        //Change horizontal movement
        HorizontalMove();
        //Change vertical movement
        VerticalMove();
        //Pressed should always be in effect one fixedUpdate after keydown
        ResetPressed();
        
    }

    // This function checks the relevant inputs
    void InputCheck()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            upPressed = true;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            upHold = true;
        }
        else
        {
            upHold = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftPressed = true;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            leftHold = true;
        }
        else
        {
            leftHold = false;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightPressed = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rightHold = true;
        }
        else
        {
            rightHold = false;
        }

    }

    //To use keyDown with FixedUpdate it should reset only after one FixedUpdate
    void ResetPressed()
    {
        upPressed = false;
        leftPressed = false;
        rightPressed = false;
    }

    //This function handles horizontal movement
    void HorizontalMove()
    {
        //Horizontal movement
        //Get the current velocity to prevent clipping
        moveSpeed = rb.velocity;
        //Left (and not right input) for moving left
        if (leftHold && !rightHold)
        {
            //When currently moving the other way add the decel to decrease slip
            if (moveSpeed.x > 0)
            {
                if (wallJumpBufferL > 0)
                {
                    moveSpeed.x -= accel;
                }
                else
                {
                    moveSpeed.x -= accel + decel;
                }
            }
            //Accelerate left
            else if (!(onRightWallCling > 0))
            {
                moveSpeed.x -= accel;
            }
        }
        //Right (and not right input) for moving right
        else if (rightHold && !leftHold)
        {
            //When currently moving the other way add the decel to decrease slip
            if (moveSpeed.x < 0)
            {
                if (wallJumpBufferR > 0)
                {
                    moveSpeed.x += accel;
                }
                else
                {
                    moveSpeed.x += accel + decel;
                }
            }
            //Accelerate right
            else if (!(onLeftWallCling > 0))
            {
                moveSpeed.x += accel;
            }
        }
        //No horizontal input or both decelerate
        else
        {
            tempDecel = decel;
            if (!grounded)
            {
                tempDecel = airDecel;
            }
            //Decelerate
            if (moveSpeed.x >= tempDecel)
            {
                moveSpeed.x -= tempDecel;
            }
            if (moveSpeed.x <= -tempDecel)
            {
                moveSpeed.x += tempDecel;
            }
            //When current speed is lower then the decel amount set speed to 0
            if (moveSpeed.x > -tempDecel && moveSpeed.x < tempDecel)
            {
                moveSpeed.x = 0;
            }
        }

        //Keep speed within max speed bounds
        if (moveSpeed.x > maxSpeed)
        {
            moveSpeed.x = maxSpeed;
        }
        if (moveSpeed.x < -maxSpeed)
        {
            moveSpeed.x = -maxSpeed;
        }

        //Apply new speed
        rb.velocity = moveSpeed;
    }

    //This funciton handles vertical movement
    void VerticalMove()
    {
        //Get the current velocity to edit
        moveSpeed = rb.velocity;

        //When you try to jump create a buffer for better feel
        if (upPressed)
        {
            jumpBuffer = jumpBufferLenght;
        }

        //Decrease wallJump buffer
        if (wallJumpBufferL > 0)
        {
            //No buffer when on the ground
            if (grounded)
            {
                wallJumpBufferL = 0;
            }
            else
            {
                wallJumpBufferL -= Time.deltaTime;
            }
            
        }
        if (wallJumpBufferR > 0)
        {
            //No buffer when on the ground
            if (grounded)
            {
                wallJumpBufferR = 0;
            }
            else
            {
                wallJumpBufferR -= Time.deltaTime;
            }
        }

        //After inputing jump
        if (jumpBuffer > 0)
        {
            //Lower jumpbuffer value overtime
            jumpBuffer -= Time.deltaTime;

            //When you are on the ground and want to jump
            if (grounded)
            {
                jumpBuffer = 0;
                moveSpeed.y = jumpStrenght;
            }
            //When you cling onto a wall do a walljump
            if (onLeftWallCling > 0)
            {
                moveSpeed.y = wallJumpHorizontal;
                moveSpeed.x = wallJumpVertical;
                wallJumpBufferL = wallJumpBuffer;
                onLeftWallCling = 0;
            }
            //When you cling onto a wall do a walljump
            if (onRightWallCling > 0)
            {
                moveSpeed.y = wallJumpHorizontal;
                moveSpeed.x = -wallJumpVertical;
                wallJumpBufferR = wallJumpBuffer;
                onRightWallCling = 0;
            }
        }

        //When in the air apply gravity
        if (!grounded)
        {
            tempGrav = gravity;

            //Apply zone gravity multiplier
            tempGrav *= gravZoneMult;

            //When clinging on wall lower gravity
            if (onLeftWallCling > 0 || onRightWallCling > 0)
            {
                tempGrav *= 0.25f;
            }

            if (moveSpeed.y > 0)
            {
                //Create jump variance based on holding jump
                if (upHold)
                {
                    moveSpeed.y -= tempGrav * 1;
                }
                else
                {
                    moveSpeed.y -= tempGrav * 2;
                }
            }
            if (moveSpeed.y <= 0)
            {
                moveSpeed.y -= tempGrav * 2;
            }
        }

        //Have a maximum speed to slide down walls
        if (onLeftWallCling > 0 || onRightWallCling > 0)
        {
            if (moveSpeed.y < -maxDownSlideSpeed)
            {
                moveSpeed.y = -maxDownSlideSpeed;
            }
        }

        //Apply movement
        rb.velocity = moveSpeed;
    }

    //Check for collisions using boxcast
    void CheckColision()
    {
        //Check left for collision
        if (Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.left, colisionDistance))
        {
            leftCol = true;
        }
        else
        {
            leftCol = false;
        }
        //Check right for collision
        if (Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.right, colisionDistance))
        {
            rightCol = true;
        }
        else
        {
            rightCol = false;
        }
        //Check down for collision

        if (Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.down, colisionDistance))
        {
            grounded = true;
        }
        else
        {
            if (rb.velocity.y < 0)
            {
                RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.down, colisionDistance);
                Vector3 test = transform.position;
                test.y += hit.distance;

            }

            grounded = false;
        }

        //Wall cling duration decrease
        if (onLeftWallCling > 0)
        {
            onLeftWallCling -= Time.deltaTime;
        }
        if (onRightWallCling > 0)
        {
            onRightWallCling -= Time.deltaTime;
        }
        //Wall cling detection
        if (rightCol && !grounded && rightHold)
        {
            onRightWallCling = clingDuration;
        }
        if (leftCol && !grounded && leftHold)
        {
            onLeftWallCling = clingDuration;
        }
        //Reset wall cling when on the ground
        if (grounded)
        {
            onLeftWallCling = 0;
            onRightWallCling = 0;
        }
    }
}
