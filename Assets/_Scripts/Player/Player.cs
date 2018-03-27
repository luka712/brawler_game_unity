using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Player : MonoBehaviour, ITeleportObjectInterface
{
    #region Editor Fields

    public string _horizontalAxis = "Horizontal_P1";
    public string _jumpButton = "Jump_P1";
    public float _movementSpeed = 12f;
    public float _jumpStrength = 15f;

    #endregion

    #region Fields

    private float currentFramePosY;
    private float previousFramePosY;
    private DividedSprite[] dividedSprites;
    private Vector3 positionToTeleportTo;
    private ICommand pushTeleportStateCommand = new PushTeleportStateCommand();
    private ICommand popTeleportStateCommand = new PopTeleportStateCommand();

    protected BoxCollider2D playerGroundCollider;
    protected BoxCollider2D playerCollisionsCollider;

    #endregion

    protected Rigidbody2D rigidBody;
    protected SpriteRenderer spriteRenderer;



    protected Animator animator;
    protected int health;

    // Spawn
    private int spawnCounter;
    private bool goingUp;
    private float spawnAnimationSpeed = 1f;
    private bool gameStart = true;

    // team
    [SerializeField]
    protected int group = 1;

    // events
    public event Action<Player> OnDeath;

    #region Properties

    public Stack<IPlayerState> State { get; set; } = new Stack<IPlayerState>();

    public bool Spawning { get; private set; }

    public PlayerMovesLookup MoveLookup { get; private set; }

    public Vector3 Scale
    {
        get { return transform.localScale; }
        set { transform.localScale = value; }
    }

    public Vector3 Position
    {
        get { return transform.position; }
        private set { transform.position = value; }
    }

    public Vector2 Velocity
    {
        get { return rigidBody.velocity; }
        private set { rigidBody.velocity = value; }
    }

    public TeleportState TeleportState { get; set; }

    public bool IsOnGround { get; private set; }

    public ISpriteDivider SpriteDivider { get; private set; }

    #endregion

    protected virtual void Start()
    {
        MoveLookup = new PlayerMovesLookup
        {
            HorizontalAxis = _horizontalAxis,
            JumpButton = _jumpButton
        };
        SpriteDivider = GetComponent<ISpriteDivider>();
        dividedSprites = SpriteDivider.DividedSprites;

        var colliders = GetComponentsInChildren<BoxCollider2D>();
        playerGroundCollider = colliders
            .FirstOrDefault(x => x.gameObject.CompareTag(Tags.PlayerGroundCollider));
        playerCollisionsCollider = colliders
            .FirstOrDefault(x => x.gameObject.CompareTag(Tags.Player));


        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Health = 100;
    }

    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health > 100)
            {
                health = 100;
            }
            else if (health <= 0)
            {
                health = 0;
                playerGroundCollider.gameObject.SetActive(false);
                if (OnDeath != null)
                {
                    spriteRenderer.color = spriteRenderer.color.SetAlpha(0f);
                    OnDeath(this);
                }
            }
        }
    }

    /// <summary>
    /// Gets player team.
    /// </summary>
    public int Group { get { return group; } }

    protected virtual void Update()
    {

        //if (spawnCounter > 0)
        //{
        //    var alpha = spriteRenderer.color.a;
        //    if (goingUp)
        //    {
        //        alpha += spawnAnimationSpeed * Time.deltaTime;
        //        if (alpha >= 1f)
        //        {
        //            goingUp = false;
        //            spawnCounter--;
        //            if (spawnCounter <= 0)
        //            {
        //                Spawning = false;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        alpha -= spawnAnimationSpeed * Time.deltaTime;
        //        if (alpha <= 0f)
        //        {
        //            goingUp = true;
        //        }
        //    }
        //    spriteRenderer.color = spriteRenderer.color.SetAlpha(alpha);
        //}

        // update
        UpdateDividedSprites();

    }



    public void AddDamage(int damage, Vector2 feedback)
    {
        Health -= damage;
        rigidBody.AddForce(feedback);
    }

    public void AddHealth(int health)
    {
        Health += health;
    }

    public void SetPosition(Vector3 position)
    {
        this.gameObject.transform.position = position;
    }

    public void Spawn(Vector2 position)
    {
        this.transform.position = position.ToVector3();
        Health = 100;
        if (gameStart)
        {
            gameStart = false;
        }
        else
        {
            Spawning = true;
            if (playerGroundCollider != null)
            {
                playerGroundCollider.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("playerGroundCollider is null. Player 114");
            }
            spriteRenderer.color = spriteRenderer.color.SetAlpha(0f);
            spawnCounter = 3;
        }
    }


    #region  Public player methods.

    public bool CheckIfPlayerIsGrounded()
        => Velocity.y == 0 && IsOnGround;

    public void LookRight()
        => ChangeLocalScaleX(Mathf.Abs(Scale.x));


    public void LookLeft()
        => ChangeLocalScaleX(Mathf.Abs(Scale.x) * -1);

    public virtual void MoveRight()
        => transform.Translate(new Vector3(_movementSpeed * Time.deltaTime, 0, 0));

    public virtual void MoveLeft()
        => transform.Translate(new Vector3(-_movementSpeed * Time.deltaTime, 0, 0));


    public virtual void PlayMoveAnimation(bool play = true, string animationName = Animations.Moving)
        => animator.SetBool(Animations.Moving, play);

    public virtual void Jump()
    {
        IsOnGround = false;
        Velocity = new Vector2(Velocity.x, 0f);
        rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
    }

    public virtual void PlayJumpAnimation(bool play = true, string animationName = Animations.Jumping)
        => animator.SetBool(Animations.Jumping, play);

    public virtual void GroundCollision()
        => IsOnGround = true;

    public virtual void PlayLandingAnimation()
        => animator.SetTrigger(Animations.Landing);

    #endregion

    #region  Teleport Interface

    /// <summary>
    /// Sets teleport destination.
    /// </summary>
    public void TeleportEnter(Vector3 destination)
    {
        if (TeleportState == TeleportState.Able)
        {
            positionToTeleportTo = destination;
            TeleportState = TeleportState.IsTeleporting;
            SpriteDivider.RenderDividedSprites();
            EnableColliders(false);
            HidePlayer();
            pushTeleportStateCommand.Execute(this);

            for (int i = 0; i < dividedSprites.Length; i++)
            {
                dividedSprites[i].Teleport(positionToTeleportTo.ToVector2() + dividedSprites[i].Position - Position.ToVector2(), .5f);
            }
        }
    }

    /// <summary>
    /// Teleports player.
    /// </summary>
    public void StopTeleporting()
    {
        EnableColliders();
        HidePlayer(false);
        Position = positionToTeleportTo;
        TeleportState = TeleportState.NotAble;
        popTeleportStateCommand.Execute(this);
    }

    /// <summary>
    /// Teleport exit.
    /// </summary>
    public void TeleportExit()
    {
        if(TeleportState == TeleportState.NotAble)
              TeleportState = TeleportState.Able;
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// Changes local scale X.
    /// </summary>
    private void ChangeLocalScaleX(float value)
        => Scale = Scale.ChangeComponentX(value);

    /// <summary>
    /// Updates divided sprites.
    /// </summary>
    private void UpdateDividedSprites()
    {
        if (TeleportState == TeleportState.Able) return;

        bool isInTeleportState = false;

        for (int i = 0; i < dividedSprites.Length; i++)
        {
            dividedSprites[i].Update(Time.deltaTime);

            // check for teleporting sprites 
            isInTeleportState = dividedSprites[i].State == global::State.TeleportAnimation;
        }

        if (TeleportState == TeleportState.IsTeleporting)
        {
            // there are busy sprites, so no need to expire them.
            if (isInTeleportState)
            {
                return;
            }

            // Expire
            for (int i = 0; i < dividedSprites.Length; i++)
            {
                dividedSprites[i].Active(false);
            }

            StopTeleporting();
        }
    }

    /// <summary>
    /// Enables/Disables colliders
    /// </summary>
    private void EnableColliders(bool value = true)
    {
        playerCollisionsCollider.enabled = value;
        playerGroundCollider.enabled = value;
    }

    /// <summary>
    /// Sets sprite renderer alpha value to 0 or 1;
    /// </summary>
    private void HidePlayer(bool value = true)
    {
        spriteRenderer.color = spriteRenderer.color.SetAlpha(value ? 0f : 1f);
    }



    #endregion
}
