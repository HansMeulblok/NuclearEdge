using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviourPun
{
    //Horizontal movement variables
    [Header("Horizontal variables")]
    public float accel;
    public float decel;
    public float airDecel;
    float tempDecel;
    public float maxSpeed;
    public float knockBackHorizontalStrenght;

    //Vertical movement variables
    [Header("Vertical variables")]
    public float jumpStrenght;
    public float jumpBufferLenght;
    public float maxFallSpeed;
    public float wallJumpHorizontal;
    public float wallJumpVertical;
    public float knockBackVerticalStrenght;

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
    public LayerMask sideMask;
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

    int countWallClingCollision;
    int countWallClingGrounded;

    //Crushing variables
    public ContactFilter2D crushFilter;
    int countCrushing;
    Collider2D[] crushResults;
    bool leftCrush;
    bool rightCrush;
    bool upCrush;
    bool downCrush;


    Vector3 lastSpeed;


    //Global variables
    Vector3 moveSpeed;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        // Disable script if player is not the local player.
        if (photonView != null && !photonView.IsMine) { enabled = false; }

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
                    moveSpeed.x -= accel * Time.fixedDeltaTime * 0.5f;
                }
                else
                {
                    moveSpeed.x -= (accel + decel) * Time.fixedDeltaTime;
                }
            }
            //Accelerate left
            else
            {
                if (!(onRightWallCling > 0))
                {
                    moveSpeed.x -= accel * Time.fixedDeltaTime;
                }
                else
                {
                    onRightWallCling -= Time.fixedDeltaTime;
                }
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
                    moveSpeed.x += accel * Time.fixedDeltaTime * 0.5f;
                }
                else
                {
                    moveSpeed.x += (accel + decel) * Time.fixedDeltaTime;
                }
            }
            //Accelerate right
            else
            {
                if (!(onLeftWallCling > 0))
                {
                    moveSpeed.x += accel * Time.fixedDeltaTime;
                }
                else
                {
                    onLeftWallCling -= Time.fixedDeltaTime;
                }
            }
        }
        //No horizontal input or both decelerate
        else
        {
            if (onLeftWallCling > 0)
            {
                onLeftWallCling = clingDuration;
            }
            if (onRightWallCling > 0)
            {
                onRightWallCling = clingDuration;
            }

            //The temporary instance of deceleration
            tempDecel = decel;

            //Have a different decelaretion while in the air
            if (!grounded)
            {
                tempDecel = airDecel;
            }
            //Decelerate
            if (moveSpeed.x >= tempDecel * Time.fixedDeltaTime)
            {
                moveSpeed.x -= tempDecel * Time.fixedDeltaTime;
            }
            if (moveSpeed.x <= -tempDecel * Time.fixedDeltaTime)
            {
                moveSpeed.x += tempDecel * Time.fixedDeltaTime;
            }
            //When current speed is lower then the decel amount set speed to 0
            if (moveSpeed.x > -tempDecel * Time.fixedDeltaTime && moveSpeed.x < tempDecel * Time.fixedDeltaTime)
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
                wallJumpBufferL -= Time.fixedDeltaTime;
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
                wallJumpBufferR -= Time.fixedDeltaTime;
            }
        }

        //After inputing jump
        if (jumpBuffer > 0)
        {
            //Lower jumpbuffer value overtime
            jumpBuffer -= Time.fixedDeltaTime;

            //When you are on the ground and want to jump
            if (grounded)
            {
                jumpBuffer = 0;
                moveSpeed.y = jumpStrenght;
            }
            //When you cling onto a wall do a walljump
            if (onLeftWallCling > 0 && canWallJump)
            {
                jumpBuffer = 0;
                moveSpeed.x = wallJumpHorizontal;
                moveSpeed.y = wallJumpVertical;
                wallJumpBufferL = wallJumpBuffer;
                onLeftWallCling = 0;
                onRightWallCling = 0;
            }
            //When you cling onto a wall do a walljump
            if (onRightWallCling > 0 && canWallJump)
            {
                jumpBuffer = 0;
                moveSpeed.x = -wallJumpHorizontal;
                moveSpeed.y = wallJumpVertical;
                wallJumpBufferR = wallJumpBuffer;
                onRightWallCling = 0;
                onLeftWallCling = 0;
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
                    moveSpeed.y -= tempGrav * upGravMult * Time.fixedDeltaTime;
                }
                else
                {
                    //Have a neutral gravity between falling and holding up
                    moveSpeed.y -= tempGrav * neutralGravMult * Time.fixedDeltaTime;
                }
            }
            //When falling
            if (moveSpeed.y <= 0)
            {
                //Have a higher gravity
                moveSpeed.y -= tempGrav * downGravMult * Time.fixedDeltaTime;
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
        if (Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.left, colisionDistance, sideMask))
        {   
            leftCol = true;
        }
        else
        {
            leftCol = false;
        }
        //Check right for collision
        if (Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.right, colisionDistance, sideMask))
        {
            rightCol = true;
        }
        else
        {
            rightCol = false;
        }

        //Check down for collision
        if (Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.down, colisionDistance, sludgeMask))
        {
            grounded = true;
        }
        else
        {
            if (rb.velocity.y < 0)
            {
                RaycastHit2D hit = Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.down, colisionDistance);
                Vector3 test = transform.position;
                test.y += hit.distance;
            }

            grounded = false;
        }



        //Check up for crushing
        if (Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x, 0.01f), 0, Vector2.up, colisionDistance + transform.localScale.y * 0.5f, sideMask))
        {
            upCrush = true;
        }
        else
        {
            upCrush = false;
        }
        //Check down for crushing
        if (Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x, 0.01f), 0, Vector2.down, colisionDistance + transform.localScale.y * 0.5f, sideMask))
        {
            downCrush = true;
        }
        else
        {
            downCrush = false;
        }
        //Check left for crushing
        if (Physics2D.BoxCast(transform.position, new Vector2(0.01f, transform.localScale.y), 0, Vector2.left, colisionDistance + transform.localScale.x * 0.5f, sideMask))
        {
            leftCrush = true;
        }
        else
        {
            leftCrush = false;
        }
        //Check right for crushing
        if (Physics2D.BoxCast(transform.position, new Vector2(0.01f, transform.localScale.y), 0, Vector2.right, colisionDistance + transform.localScale.x * 0.5f, sideMask))
        {
            rightCrush = true;
        }
        else
        {
            rightCrush = false;
        }

        //Check for crush with box overlap
        if (Physics2D.OverlapBox(transform.position, transform.localScale, 0, crushFilter, results:crushResults) > 1)
        {
            //bool crushed = false;
            for (int i = 0; i < crushResults.Length; i++)
            {
                WallChange wc = crushResults[i].GetComponent<WallChange>();
                if (wc != null)
                {
                    Debug.Log("Overlap crush");
                }
            }
        }

        //Check for crushing
        if ((downCrush && upCrush) || (leftCrush && rightCrush))
        {
            //Increase counter when two opiside collisions are detected
            countCrushing++;
            //Have a small buffer to avoid false posistives
            if (countCrushing >= 3)
            {
                Debug.Log("Boxcast crushed");
            }
        }
        else
        {
            //Reset counter when not getting crushed
            countCrushing = 0;
        }



        //Wall cling duration decrease
        if (onLeftWallCling > 0)
        {
            if (Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.left, colisionDistance, sideMask) && canWallCling)
            {
                countWallClingCollision = 0;
            }
            else
            {
                countWallClingCollision++;
                if (countWallClingCollision >= 5)
                {
                    onLeftWallCling = 0;
                }
            }

        }
        if (onRightWallCling > 0)
        {
            if (Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.right, colisionDistance, sideMask) && canWallCling)
            {
                countWallClingCollision = 0;
            }
            else
            {
                countWallClingCollision++;
                if (countWallClingCollision >= 5)
                {
                    onRightWallCling = 0;
                }
            }
        }

        //Wall cling detection if you are still on the wall
        if ((rightCol && lastSpeed.x > 0) || (rightCol && rightHold) && canWallCling)
        {
            onRightWallCling = clingDuration;
            lastSpeed.x = 0;
        }
        if ((leftCol && lastSpeed.x < 0) || (leftCol && leftHold) && canWallCling)
        {
            onLeftWallCling = clingDuration;
            lastSpeed.x = 0;
        }
        //Reset wall cling when on the ground
        if (grounded && (onLeftWallCling > 0 || onRightWallCling > 0))
        {
            countWallClingGrounded++;
            if (countWallClingGrounded > 3)
            {
                onLeftWallCling = 0;
                onRightWallCling = 0;
            }
        }
        else
        {
            countWallClingGrounded = 0;
        }

        if (Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.down, 0.05f, sludgeMask))
        {
            //check if falling platform is below the player and if it is parent it to it.s
            RaycastHit2D downHit = Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.down, 0.05f, sludgeMask);
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

        if (onLeftWallCling > 0 || onRightWallCling > 0)
        {
            if (rb.velocity.x > 0.1 || rb.velocity.x < -0.1)
            {
                onLeftWallCling = 0;
                onRightWallCling = 0;
            }
        }

        if (rb.velocity.x > 0.1 || rb.velocity.x < -0.1 || grounded)
        {
            lastSpeed = rb.velocity;
        }
    }

    public void UnParent()
    {
        //unparent the object
        transform.parent = null;
    }

    public void KnockBack()
    {
        moveSpeed = rb.velocity;
        moveSpeed.y = knockBackVerticalStrenght;

        if (knockBackHorizontalStrenght != 0)
        {
            if (moveSpeed.x > 0)
            {
                moveSpeed.x = -knockBackHorizontalStrenght;
            }
            else if (moveSpeed.x < 0)
            {
                moveSpeed.x = knockBackHorizontalStrenght;
            }
        }

        rb.velocity = moveSpeed;
    }
}
