using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    //Horizontal movement variables
    [Header("Horizontal variables")]
    public float accel;
    public float decel;
    public float maxSpeed;

    //Vertical movement variables
    [Header("Vertical variables")]
    public float jumpStrenght;
    public float jumpBufferLenght;
    public float wallJumpHorizontal;
    public float wallJumpVertical;
    public float upGrav;
    public float downGrav;
    public float clingDuration;

    float jumpBuffer;
    float onLeftWallCling;
    float onRightWallCling;

    //Collisions
    [Header("Collision")]
    public float colisionDistance;
    bool grounded;
    bool leftCol;
    bool rightCol;

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
        //Check for colisions
        CheckColision();
        //Change horizontal movement
        HorizontalMove();
        //Change vertical movement
        VerticalMove();

        Debug.Log(grounded);
    }

    //This fuction handles horizontal movement
    void HorizontalMove()
    {
        //Horizontal movement
        //Get the current velocity to prevent clipping
        moveSpeed = rb.velocity;
        //Left (and not right input) for moving left
        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            //When currently moving the other way add the decel to decrease slip
            if (moveSpeed.x > 0)
            {
                moveSpeed.x -= accel + decel;
            }
            //Accelerate left
            else if (!(onRightWallCling > 0))
            {
                moveSpeed.x -= accel;
            }
        }
        //Right (and not right input) for moving right
        else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            //When currently moving the other way add the decel to decrease slip
            if (moveSpeed.x < 0)
            {
                moveSpeed.x += accel + decel;
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
            //Decelerate
            if (moveSpeed.x >= decel)
            {
                moveSpeed.x -= decel;
            }
            if (moveSpeed.x <= -decel)
            {
                moveSpeed.x += decel;
            }
            //When current speed is lower then the decel amount set speed to 0
            if (moveSpeed.x > -decel && moveSpeed.x < decel)
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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            jumpBuffer = jumpBufferLenght;
        }

        //
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
                onLeftWallCling = 0;
            }
            //When you cling onto a wall do a walljump
            if (onRightWallCling > 0)
            {
                moveSpeed.y = wallJumpHorizontal;
                moveSpeed.x = -wallJumpVertical;
                onRightWallCling = 0;
            }
        }

        //When in the air apply gravity
        if (!grounded)
        {
            if (moveSpeed.y > 0)
            {
                //Create jump variance based on holding jump
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    moveSpeed.y -= upGrav;
                }
                else
                {
                    moveSpeed.y -= downGrav;
                }
            }
            if (moveSpeed.y <= 0)
            {
                moveSpeed.y -= downGrav;
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
        if (rightCol && !grounded && Input.GetKey(KeyCode.RightArrow))
        {
            onRightWallCling = clingDuration;
        }
        if (leftCol && !grounded && Input.GetKey(KeyCode.LeftArrow))
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
