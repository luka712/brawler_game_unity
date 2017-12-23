using UnityEngine;

public enum JumpState { None, Jumping, DoubleJump }

public class MovePlayer : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 5f;
    [SerializeField]
    private float jumpStrength = 500f;
    [SerializeField]
    private float doubleJumpStrength = 250f;

    private new Transform transform;
    private Rigidbody2D rigidBody;
    private Animator animator;

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
        initalXScale = transform.localScale.x;
    }


    private void FixedUpdate()
    {
        LeftRightMovement();
        Jump();
    }

    private void LeftRightMovement()
    {
        var direction = Input.GetAxis("Horizontal");
        animator.SetBool("moving", direction != 0f);
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
        if (velocity != 0f)
        {
            transform.Translate(new Vector3(velocity, 0, 0));
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && JumpState != JumpState.DoubleJump)
        {
            if (JumpState == JumpState.None)
            {
                JumpState = JumpState.Jumping;
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);
                rigidBody.AddForce(Vector2.up * jumpStrength, ForceMode2D.Force);
            }
            else if (JumpState == JumpState.Jumping)
            {
                JumpState = JumpState.DoubleJump;
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);
                rigidBody.AddForce(Vector2.up * doubleJumpStrength, ForceMode2D.Force);
            }
        }
    }

    public void ResetJumpState()
    {
        JumpState = JumpState.None;
    }
}
