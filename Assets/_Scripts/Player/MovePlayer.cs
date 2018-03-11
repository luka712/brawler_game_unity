using UnityEngine;

public enum JumpState { None, Jumping, DoubleJump }

public class MovePlayer : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float jumpStrength = 15f;
    public float doubleJumpStrength = 12f;

    // action
    public string jumpButton = "Jump_P1";
    public string runButton = "Run_P1";
    public string horizontalAxis = "Horizontal_P1";


    private new Transform transform;
    private Rigidbody2D rigidBody;
    private Animator animator;
    private Player player;

    private float initalXScale;

    /// <summary>
    /// Is player in jump.
    /// </summary>
    public JumpState JumpState { get; set; }

    // Use this for initialization
    private void Start()
    {
        transform = GetComponent<Transform>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        initalXScale = transform.localScale.x;
    }


    private void FixedUpdate()
    {
        LeftRightMovement();
        Jump();
    }

    private void LeftRightMovement()
    {
        if (!player.IsTeleporting)
        {
            animator.enabled = true;
            var direction = Input.GetAxis(horizontalAxis);
            var movingFast = Input.GetButton(runButton);
            animator.SetBool(ZugaiAnimations.Moving, direction != 0f);
            animator.SetBool(ZugaiAnimations.FastMoving, movingFast);
            if (direction > 0f)
            {
                direction = 1f;
                transform.localScale = transform.localScale.ChangeComponentX(initalXScale);
            }
            else if (direction < 0f)
            {
                direction = -1f;
                transform.localScale = transform.localScale.ChangeComponentX(-initalXScale);
            }

            var velocity = direction * movementSpeed * Time.deltaTime;
            if (movingFast)
            {
                velocity *= 2;
            }
            if (velocity != 0f)
            {
                transform.Translate(new Vector3(velocity, 0, 0));
            }
        }
        else
        {
            animator.enabled = false;
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown(jumpButton) && JumpState != JumpState.DoubleJump)
        {
            if (JumpState == JumpState.None)
            {
                JumpState = JumpState.Jumping;
                animator.SetBool(ZugaiAnimations.Jumping, true);
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);
                rigidBody.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
            }
            else if (JumpState == JumpState.Jumping)
            {
                JumpState = JumpState.DoubleJump;
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);
                rigidBody.AddForce(Vector2.up * doubleJumpStrength, ForceMode2D.Impulse);
            }
        }
    }

    internal void ResetJumpState()
    {
        JumpState = JumpState.None;
        animator.SetBool(ZugaiAnimations.Jumping, false);
        animator.SetTrigger(ZugaiAnimations.Landing);
    }

    internal void StopAnimating()
    {
        animator.SetBool(ZugaiAnimations.Moving, false);
        animator.SetBool(ZugaiAnimations.FastMoving, false);
    }
}
