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
    public float maxFallSpeed;
    public float wallJumpHorizontal;
    public float wallJumpVertical;

    [Header("Gravity variables")]
    public float gravity;
    public float gravZoneMult;
    public float upGravMult;
    public float neutralGravMult;
    public float downGravMult;
    public float upWallGravMult;
    public float downWallGravMult;
    float tempGrav;

    [Header("Walljump variables")]
    public float clingDuration;
    public float maxDownSlideSpeed;
    public float wallJumpBuffer;
    public bool canWallCling = true;
    public bool canWallJump = true;
    float jumpBuffer;
    float onLeftWallCling;
    float onRightWallCling;
    float wallJumpBufferL;
    float wallJumpBufferR;

    //Collisions
    [Header("Collision")]
    public float colisionDistance;
    public LayerMask sludgeMask;
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
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            upPressed = true;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            upHold = true;
        }
        else
        {
            upHold = false;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            leftPressed = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            leftHold = true;
        }
        else
        {
            leftHold = false;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            rightPressed = true;
        }
        if (Input.GetKey(KeyCode.D))
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
                //Prevent moving back to a wall too quickly
                if (wallJumpBufferL > 0)
                {
                    moveSpeed.x -= accel * Time.deltaTime;
                }
                else
                {
                    moveSpeed.x -= (accel + decel) * Time.deltaTime;
                }
            }
            //Accelerate left
            else if (!(onRightWallCling > 0))
            {
                moveSpeed.x -= accel * Time.deltaTime;
            }
        }
        //Right (and not right input) for moving right
        else if (rightHold && !leftHold)
        {
            //When currently moving the other way add the decel to decrease slip
            if (moveSpeed.x < 0)
            {
                //Prevent moving back to a wall too quickly
                if (wallJumpBufferR > 0)
                {
                    moveSpeed.x += accel * Time.deltaTime;
                }
                else
                {
                    moveSpeed.x += (accel + decel) * Time.deltaTime;
                }
            }
            //Accelerate right
            else if (!(onLeftWallCling > 0))
            {
                moveSpeed.x += accel * Time.deltaTime;
            }
        }
        //No horizontal input or both decelerate
        else
        {
            //The temporary instance of deceleration
            tempDecel = decel;

            //Have a different decelaretion while in the air
            if (!grounded)
            {
                tempDecel = airDecel;
            }
            //Decelerate
            if (moveSpeed.x >= tempDecel * Time.deltaTime)
            {
                moveSpeed.x -= tempDecel * Time.deltaTime;
            }
            if (moveSpeed.x <= -tempDecel * Time.deltaTime)
            {
                moveSpeed.x += tempDecel * Time.deltaTime;
            }
            //When current speed is lower then the decel amount set speed to 0
            if (moveSpeed.x > -tempDecel * Time.deltaTime && moveSpeed.x < tempDecel * Time.deltaTime)
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
            if (onLeftWallCling > 0 && canWallJump)
            {
                moveSpeed.y = wallJumpHorizontal;
                moveSpeed.x = wallJumpVertical;
                wallJumpBufferL = wallJumpBuffer;
                onLeftWallCling = 0;
            }
            //When you cling onto a wall do a walljump
            if (onRightWallCling > 0 && canWallJump)
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
                if (moveSpeed.y > 0)
                {
                    tempGrav *= upWallGravMult;
                }
                else
                {
                    tempGrav *= downWallGravMult;
                }
            }

            //When moving up
            if (moveSpeed.y > 0)
            {
                //Create jump variance based on holding jump
                if (upHold)
                {
                    //Have a lower gravity when holding up to create short and long hops
                    moveSpeed.y -= tempGrav * upGravMult * Time.deltaTime;
                }
                else
                {
                    //Have a neutral gravity between falling and holding up
                    moveSpeed.y -= tempGrav * neutralGravMult * Time.deltaTime;
                }
            }
            //When falling
            if (moveSpeed.y <= 0)
            {
                //Have a higher gravity
                moveSpeed.y -= tempGrav * downGravMult * Time.deltaTime;
            }
        }

        //Have a maximum speed to slide down walls
        if (onLeftWallCling > 0 || onRightWallCling > 0)
        {
            //Have a maximum speed at which you can slide down a wall
            if (moveSpeed.y < -maxDownSlideSpeed)
            {
                moveSpeed.y = -maxDownSlideSpeed;
            }
        }

        //Have a maximum fallspeed
        if (moveSpeed.y < -maxFallSpeed)
        {
            moveSpeed.y = -maxFallSpeed;
        }

        //Apply movement
        rb.velocity = moveSpeed;
    }

    //Check for collisions using boxcast
    void CheckColision()
    {
        //Check left for collision
        if (Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.left, colisionDistance, sludgeMask))
        {
            leftCol = true;
        }
        else
        {
            leftCol = false;
        }
        //Check right for collision
        if (Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.right, colisionDistance, sludgeMask))
        {
            rightCol = true;
        }
        else
        {
            rightCol = false;
        }

        //Check down for collision
        if (Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.down, colisionDistance, sludgeMask))
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
            if (Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.left, colisionDistance) && canWallCling)
            {
                onLeftWallCling -= Time.deltaTime;
            }
            else
            {
                onLeftWallCling = 0;
            }

        }
        if (onRightWallCling > 0)
        {
            if (Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.right, colisionDistance) && canWallCling)
            {
                onRightWallCling -= Time.deltaTime;
            }
            else
            {
                onRightWallCling = 0;
            }
        }

        //Wall cling detection if you are still on the wall
        if (rightCol && !grounded && rightHold && canWallCling)
        {
            onRightWallCling = clingDuration;
        }
        if (leftCol && !grounded && leftHold && canWallCling)
        {
            onLeftWallCling = clingDuration;
        }
        //Reset wall cling when on the ground
        if (grounded)
        {
            onLeftWallCling = 0;
            onRightWallCling = 0;
        }

        if (Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.down, 0.05f, sludgeMask))
        {
          //check if falling platform is below the player and if it is parent it to it.s
            RaycastHit2D downHit = Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.down, 0.05f);
            if (downHit.transform.tag == "Falling Platform")
            {
                transform.parent = downHit.transform;
            }
            else
            {
                transform.parent = null;
            }

        }
        else
        {
          transform.parent = null;
        }

    }

    public void UnParent()
    {
        //unparent the object
        transform.parent = null;
    }
}
