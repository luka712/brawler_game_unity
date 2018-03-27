using UnityEngine;

public enum JumpState { None, Jumping, DoubleJump }

public class MovePlayer : MonoBehaviour
{

    public float _doubleJumpStrength = 12f;
    private float _rotationCorrectionSpeed = 250f;

    // action
    public string _runButton = "Run_P1";



    private new Transform transform;
    private Rigidbody2D rigidBody;
    private Animator animator;
    private Player player;

    private float initalXScale;

    /// <summary>
    /// Is player in jump.
    /// </summary>
    public JumpState JumpState { get; set; }

    public float GravityScale
    {
        get { return rigidBody.gravityScale; }
        set { rigidBody.gravityScale = value; }
    }

    public float Rotation
    {
        get { return transform.eulerAngles.z; }
        set
        {
            transform.rotation = Quaternion.Euler(
                new Vector3(transform.rotation.x, transform.rotation.y, value));
        }
    }

    public bool HasParent => this.gameObject.transform.parent != null;

    // Use this for initialization
    private void Start()
    {
        transform = GetComponent<Transform>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        initalXScale = transform.localScale.x;
    }

    private void Update()
    {
        CorrectRotation();
    }

    private void FixedUpdate()
    {
        LeftRightMovement();
        Jump();
    }

    private void LeftRightMovement()
    {
        //if (player.IsTeleporting)
        //{
        //    animator.enabled = true;
        //    var direction = 0f; // Input.GetAxis(_horizontalAxis);
        //    var movingFast = Input.GetButton(_runButton);
        //    animator.SetBool(Animations.Moving, direction != 0f);
        //    animator.SetBool(Animations.FastMoving, movingFast);
        //    if (direction > 0f)
        //    {
        //        direction = 1f;
        //        transform.localScale = transform.localScale.ChangeComponentX(initalXScale);
        //    }
        //    else if (direction < 0f)
        //    {
        //        direction = -1f;
        //        transform.localScale = transform.localScale.ChangeComponentX(-initalXScale);
        //    }

        //    var velocity = direction * Time.deltaTime;

        //    // flip velocity if rotated 180
        //    if(Rotation > 180)
        //    {
        //        velocity *= -1;
        //    }

        //    if (movingFast)
        //    {
        //        velocity *= 2;
        //    }
        //    if (velocity != 0f)
        //    {
        //        transform.Translate(new Vector3(velocity, 0, 0));
        //    }
        //}
        //else
        //{
        //    animator.enabled = false;
        //}
    }

    private void Jump()
    {
        //if (Input.GetButtonDown(_jumpButton) && JumpState != JumpState.DoubleJump)
        //{
        //    if (JumpState == JumpState.None)
        //    {
        //        JumpState = JumpState.Jumping;
        //     
        //    }
        //    else if (JumpState == JumpState.Jumping)
        //    {
        //        JumpState = JumpState.DoubleJump;
        //        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);
        //        rigidBody.AddForce(Vector2.up * _doubleJumpStrength, ForceMode2D.Impulse);
        //    }
        //}
    }

    private void CorrectRotation()
    {
        var rotation = this.transform.rotation.eulerAngles;
        if (!HasParent && rotation.z != 0)
        {
            // degrees
            if (rotation.z > 180)
            {
                rotation.z += _rotationCorrectionSpeed * Time.deltaTime;
                if (rotation.z > 360)
                {
                    rotation.z = 0;
                }
            }
            else
            {
                rotation.z -= _rotationCorrectionSpeed * Time.deltaTime;
                if (rotation.z < 0)
                {
                    rotation.z = 0;
                }
            }

            transform.rotation = Quaternion.Euler(rotation);
        }
    }

    internal void ResetJumpState()
    {
        JumpState = JumpState.None;
        animator.SetBool(Animations.Jumping, false);
        animator.SetTrigger(Animations.Landing);
    }

    internal void StopAnimating()
    {
        animator.SetBool(Animations.Moving, false);
        animator.SetBool(Animations.FastMoving, false);
    }

    internal void SetParent(Transform transform)
    {
        this.gameObject.transform.SetParent(transform);
    }
}
