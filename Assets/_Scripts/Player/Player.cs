
#define DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Player : MonoBehaviour, ITeleportObjectInterface, IStickPlayerInterface, ISpawnPlayerInterface
{
#if DEBUG

    public string _debugPlayerState;

#endif


    #region Editor Fields

    public string _horizontalAxis = "Horizontal_P1";
    public string _jumpButton = "Jump_P1";
    public string _attackButton = "Fire_P1";
    public string _specialAttackButton = "Fire2_P1";
    public float _movementSpeed = 12f;
    public float _jumpStrength = 15f;
    public float _doubleJumpStrength = 10f;
    public float _spawnAnimationSpeed = 1f;
    public int _playerHealth = 3;
    public int _playerSpawnFadeInOuts = 3;
    public int _group = 1;
    public GameObject _dagger;

    #endregion

    #region Fields

    private float initalGravity;
    private DividedSprite[] dividedSprites;
    private Vector3 positionToTeleportTo;
    private ICommand pushTeleportStateCommand = new PushTeleportStateCommand();
    private ICommand popTeleportStateCommand = new PopTeleportStateCommand();

    // Colliders
    protected BoxCollider2D playerGroundCollider;
    protected BoxCollider2D playerCollisionsCollider;
    protected BoxCollider2D playerAttackCollider;

    // Spawn variables
    private int spawnCounter;
    private bool goingUp;
    private bool gameStart = true;

    // Components
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigidBody;
    protected Animator animator;

    #endregion

    #region Events

    public event Action<ISpawnPlayerInterface> OnDeathSpawn;
    public event Action<Player> OnDeath;

    #endregion

    protected int health;



    // events


    #region Properties

    /// <summary>
    /// Handles player states.
    /// </summary>
    public Stack<IPlayerState> State { get; set; } = new Stack<IPlayerState>();

    /// <summary>
    /// Indicates if player is currently spawning.
    /// </summary>
    public bool IsSpawning { get; private set; }

    /// <summary>
    /// Move lookup.
    /// </summary>
    public PlayerMovesLookup MoveLookup { get; private set; }

    /// <summary>
    /// The player scale.
    /// </summary>
    public Vector3 Scale
    {
        get { return transform.localScale; }
        set { transform.localScale = value; }
    }

    /// <summary>
    /// The player position.
    /// </summary>
    public Vector3 Position
    {
        get { return transform.position; }
        private set { transform.position = value; }
    }

    /// <summary>
    /// The player velocity.
    /// </summary>
    public Vector2 Velocity
    {
        get { return rigidBody.velocity; }
        private set { rigidBody.velocity = value; }
    }

    /// <summary>
    /// Player rotation around z.
    /// </summary>
    public float Rotation
    {
        get { return transform.localRotation.eulerAngles.z; }
        set { transform.localRotation = Quaternion.Euler(0, 0, value); }
    }

    /// <summary>
    /// Current teleport state.
    /// </summary>
    public TeleportState TeleportState { get; set; }

    /// <summary>
    /// Is player on ground.
    /// </summary>
    public bool IsOnGround { get; private set; }

    /// <summary>
    /// The sprite divider interface.
    /// </summary>
    public ISpriteDivider SpriteDivider { get; private set; }

    /// <summary>
    /// Is player sticked to parent. Used for wall running.
    /// </summary>
    public bool IsStickedToParent => this.transform.parent != null;

    /// <summary>
    /// The player health.
    /// </summary>
    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health > _playerHealth)
            {
                health = _playerHealth;
            }
            else if (health <= 0)
            {
                health = 0;
                playerGroundCollider.gameObject.SetActive(false);
                if (OnDeath != null)
                {
                    spriteRenderer.color = spriteRenderer.color.SetAlpha(0f);
                    OnDeathSpawn(this);
                    OnDeath(this);
                }
            }
        }
    }

    #endregion

    protected virtual void Start()
    {
        MoveLookup = new PlayerMovesLookup
        {
            HorizontalAxis = _horizontalAxis,
            JumpButton = _jumpButton,
            AttackButton = _attackButton,
            SpecialAttackButton = _specialAttackButton
        };
        SpriteDivider = GetComponent<ISpriteDivider>();
        dividedSprites = SpriteDivider.DividedSprites;

        /* get colliders
         * attack collider should be active, only if attacking
         */
        var colliders = GetComponentsInChildren<BoxCollider2D>();
        playerGroundCollider = colliders
            .First(x => x.gameObject.CompareTag(Tags.PlayerGroundCollider));
        playerCollisionsCollider = colliders
            .First(x => x.gameObject.CompareTag(Tags.Player));
        playerAttackCollider = colliders
            .First(x => x.gameObject.CompareTag(Tags.AttackCollider));
        ActivateAttackCollider(false);

        rigidBody = GetComponent<Rigidbody2D>();
        initalGravity = rigidBody.gravityScale;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        Health = _playerHealth;
        spawnCounter = _playerSpawnFadeInOuts;
    }


    /// <summary>
    /// Gets player team.
    /// </summary>
    public int Group { get { return _group; } }

    protected virtual void Update()
    {

        UpdateDividedSprites();
        SpawnPlayer();

        if (Rotation != 0)
        {
            Rotation *= 0.9f;
            if (Rotation < 1f)
            {
                Rotation = 0f;
            }
        }

#if DEBUG
        _debugPlayerState = State.Peek().ToString();
#endif
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


    #region  Public player methods.

    public void CreateDagger()
    {
        var dagger = Instantiate(_dagger).GetComponent<Dagger>();
        dagger.Group = Group;
        dagger.Fire(this.transform.position, 
            this.transform.localScale.x > 0 ? Vector2.right : Vector2.left);
    }

    public bool CheckIfPlayerIsGrounded()
        => Velocity.y == 0 && IsOnGround;

    public void LookRight()
        => ChangeLocalScaleX(Mathf.Abs(Scale.x));

    public void LookLeft()
        => ChangeLocalScaleX(Mathf.Abs(Scale.x) * -1);

    public void MoveRight()
        => transform.Translate(new Vector3(_movementSpeed * Time.deltaTime, 0, 0));

    public void MoveLeft()
        => transform.Translate(new Vector3(-_movementSpeed * Time.deltaTime, 0, 0));


    public void PlayMoveAnimation(bool play = true, string animationName = AnimationVariables.Moving)
        => animator.SetBool(AnimationVariables.Moving, play);

    public void Jump()
    {
        IsOnGround = false;
        Velocity = new Vector2(Velocity.x, 0f);
        rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
    }

    public void DoubleJump()
    {
        IsOnGround = false;
        Velocity = new Vector2(Velocity.x, 0f);
        rigidBody.AddForce(Vector2.up * _doubleJumpStrength, ForceMode2D.Impulse);
    }

    public void PlayJumpAnimation(bool play = true, string animationName = AnimationVariables.Jumping)
        => animator.SetBool(AnimationVariables.Jumping, play);

    public void PlayFallingAnimation(bool play = true)
    => animator.SetBool(AnimationVariables.Falling, play);

    public void GroundCollision()
        => IsOnGround = true;

    public void PlayLandingAnimation()
        => animator.SetTrigger(AnimationVariables.Landing);

    public void PlayAttackAnimation()
        => animator.SetTrigger(AnimationVariables.Attack);

    public void PlaySpecialAttackAnimation()
        => animator.SetTrigger(AnimationVariables.SpecialAttack);

    public void ActivateAttackCollider(bool value = true)
        => playerAttackCollider.enabled = value;

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
        if (TeleportState == TeleportState.NotAble)
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

    #region Stick to parent

    /// <summary>
    /// Stick player to parent object.
    /// </summary>
    public void StickToParent(Transform parentTransform)
    {
        this.transform.parent = parentTransform;
        rigidBody.gravityScale = 0;
        Rotation = 0;
    }

    /// <summary>
    /// Unsticks player from parent object.
    /// </summary>
    public void UnstickFromParent()
    {
        this.transform.parent = null;
        rigidBody.gravityScale = initalGravity;
    }

    #endregion

    #region Spawn player interface

    /// <summary>
    /// Spawns the player at position.
    /// </summary>
    public void Spawn(Vector2 position)
    {
        this.transform.position = position.ToVector3(this.transform.position.z);
        Health = _playerHealth;
        if (gameStart)
        {
            gameStart = false;
        }
        else
        {
            IsSpawning = true;
            if (playerGroundCollider != null)
            {
                playerGroundCollider.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("playerGroundCollider is null. Player 114");
            }
            spriteRenderer.color = spriteRenderer.color.SetAlpha(0f);
            spawnCounter = _playerSpawnFadeInOuts;
        }
    }

    /// <summary>
    /// The methods responsible for player spawn animations.
    /// </summary>
    private void SpawnPlayer()
    {
        if (spawnCounter > 0)
        {
            var alpha = spriteRenderer.color.a;
            if (goingUp)
            {
                alpha += _spawnAnimationSpeed * Time.deltaTime;
                if (alpha >= 1f)
                {
                    goingUp = false;
                    if (--spawnCounter <= 0)
                    {
                        IsSpawning = false;
                    }
                }
            }
            else
            {
                alpha -= _spawnAnimationSpeed * Time.deltaTime;
                if (alpha <= 0f)
                {
                    goingUp = true;
                }
            }
            spriteRenderer.color = spriteRenderer.color.SetAlpha(alpha);
        }
    }

    #endregion
}
