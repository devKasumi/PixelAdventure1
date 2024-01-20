using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    [Header("MOMEMENT")]
    [SerializeField] float moveSpeed;
    [SerializeField] float h_Input;
    bool isFacingRight = true;

    [Header("JUMP")]
    [SerializeField] float jumpForce;
    [SerializeField] float timeJump;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask layerGround;
    [SerializeField] float fallingMultiple;
    [SerializeField] int extraJumpTimes = 1;

    [Header("Wall Jump")]
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Vector2 wallJumpForce;
    [SerializeField] float wallJumpSpeed;
    bool isWalled;
    bool isSliding;

    Vector2 velocityGravity;
    float timeCounter;
    bool isJumping;
    bool isGrounded;
    int remainingJumpTimes;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        velocityGravity =  new Vector2(0,-Physics2D.gravity.y);
    }

    // Update is called once per frame
    void Update()
    {
        h_Input = Input.GetAxis("Horizontal");  // move horizontally
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(.8f,.2f), 0, layerGround);
        isWalled = Physics2D.OverlapBox(wallCheck.position, new Vector2(.2f, .8f), 0, wallLayer);
        if ((h_Input < 0 && isFacingRight) || (h_Input > 0 && !isFacingRight))
            Flip(); 

        #region JUMP
        if (Input.GetKeyDown(KeyCode.Space)) // Returns true during the frame the user starts pressing down the key identified by the key KeyCode enum parameter.
        {
            if (isGrounded)
            {
                remainingJumpTimes = extraJumpTimes;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isJumping = true;
                timeCounter = 0;
            }
            else if (remainingJumpTimes > 0)
            {
                if (!isSliding)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce*.8f);
                    remainingJumpTimes--;
                }
                else
                {
                    rb.velocity = new Vector2(-h_Input * wallJumpForce.x, wallJumpForce.y);
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && !isGrounded) // Returns true during the frame the user releases the key identified by the key KeyCode enum parameter
        {
            isJumping = false;
            if (rb.velocity.y > 0)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .7f);
        }

        // Apply more gravity - player falling fast
        if (rb.velocity.y < 0)
        {
            rb.velocity -= velocityGravity * fallingMultiple * Time.deltaTime;
        }

        if (rb.velocity.y > 0 && isJumping)
        {
            timeCounter += Time.deltaTime;

            if (timeCounter >= timeJump)
            {
                isJumping = false;
            }

            float curMultiple = fallingMultiple;
            float t = timeCounter / timeJump;
            if (t > .5f)
            {
                curMultiple = fallingMultiple * (1 - t);
                rb.velocity += velocityGravity * curMultiple * Time.deltaTime;
            }
        }
        #endregion

        #region Wall Jump
        if (isWalled && !isGrounded && h_Input != 0)
        {
            isSliding = true;
            remainingJumpTimes = extraJumpTimes;
        }
        else isSliding = false;

        #endregion
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        float x = transform.localScale.x;
        x = -x;
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(h_Input*moveSpeed, rb.velocity.y);
        if (isSliding)
        {
            rb.velocity = new Vector2(0, Mathf.Clamp(rb.velocity.y, -wallJumpSpeed, float.MaxValue));
        }
    }

}
