using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Player : MonoBehaviour
{
    protected Rigidbody2D rigidBody;
    protected SpriteRenderer spriteRenderer;
    protected BoxCollider2D playerGroundCollider;
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

    public bool Spawning { get; private set; }

    protected virtual void Start()
    {
        playerGroundCollider = GetComponentsInChildren<BoxCollider2D>()
            .FirstOrDefault(x => x.gameObject.CompareTag(Tags.PlayerGroundCollider));
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        CanTeleport = true;
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

    public bool CanTeleport;
    public bool IsTeleporting; 
    
    /// <summary>
    /// Gets player team.
    /// </summary>
    public int Group { get { return group; } }

    private void Update()
    {
        if (spawnCounter > 0)
        {
            var alpha = spriteRenderer.color.a;
            if (goingUp)
            {
                alpha += spawnAnimationSpeed * Time.deltaTime;
                if (alpha >= 1f)
                {
                    goingUp = false;
                    spawnCounter--;
                    if(spawnCounter <= 0)
                    {
                        Spawning = false;
                    }
                }
            }
            else
            {
                alpha -= spawnAnimationSpeed * Time.deltaTime;
                if (alpha <= 0f)
                {
                    goingUp = true;
                }
            }
            spriteRenderer.color = spriteRenderer.color.SetAlpha(alpha);
        }       
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
}
