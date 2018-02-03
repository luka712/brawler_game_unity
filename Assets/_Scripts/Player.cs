using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D playerGroundCollider;
    private int health;

    // Spawn
    private int spawnCounter;
    private bool goingUp;
    private float spawnAnimationSpeed = 1f;
    private bool gameStart = true;

    // bullets 
    [SerializeField]
    private Bullet bulletInstance;
    private List<Bullet> bullets = new List<Bullet>();

    // team
    [SerializeField]
    private int group = 1;

    // buttons
    public string fireButton = "Fire_P1";

    // events
    public event Action<Player> OnDeath;

    public bool Spawning { get; private set; }

    private void Start()
    {
        playerGroundCollider = GetComponentsInChildren<BoxCollider2D>()
            .FirstOrDefault(x => x.gameObject.CompareTag(Tags.PlayerGroundCollider));
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    private void FixedUpdate()
    {
        if (Input.GetButtonDown(fireButton))
        {
            var bullet = bullets.FirstOrDefault(x => x.gameObject.activeSelf == false);

            if (bullet == null)
            {
                bullet = Instantiate(bulletInstance.gameObject).GetComponent<Bullet>();
                bullet.Group = Group;
                bullets.Add(bullet);
            }

            bullet.transform.position = this.transform.position;

            // only left right for now
            // TODO : up down movements
            var leftRightDirection = this.transform.localScale.x < 0 ? -1 : 1;
            bullet.Fire(new Vector2(leftRightDirection, 0));
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
        Spawning = true;
        if (gameStart)
        {
            gameStart = false;
        }
        else
        {
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
