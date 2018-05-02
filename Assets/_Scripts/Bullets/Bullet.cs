using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bullet : MonoBehaviour
{
    #region Editor Variables

    public float _speed = 10f;
    public float _timeToExpire = 3f;
    public float _fadeOutSpeed = 10f;

    #endregion

    #region Constants

    private const float StartFadingAfter = 2f;

    #endregion

    #region Fields

    protected Rigidbody2D rigBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool fadeOut;

    #endregion

    #region Properties

    public int Group { get; set; }

    public Animator Animator
    {
        get
        {
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
            return animator;
        }
    }

    #endregion

    #region Unity Methods 

    // Use this for initialization
    public virtual void Awake()
    {
        rigBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if(fadeOut)
        {
            spriteRenderer.color = spriteRenderer.color.SetAlpha(spriteRenderer.color.a - _fadeOutSpeed * Time.deltaTime);
        }
    }

    protected virtual void FixedUpdate()
    {

    }

    #endregion

    #region Methods 

    public virtual void Fire(Vector2 position, Vector2 direction)
    {
        this.gameObject.SetActive(true);
        this.transform.position = position.ToVector3();
        var x = this.transform.localScale.x;
        this.transform.localScale = this.transform.localScale
            .ChangeComponentX(direction.x > 0 ? x : Mathf.Abs(x) * -Math.Abs(x));
        rigBody.velocity = direction * _speed;

        // This will make any bullet inactive after 15 seconds.
        StartCoroutine(Timer());
    }

    public virtual IEnumerator Timer()
    {
        yield return new WaitForSeconds(_timeToExpire);
        this.gameObject.SetActive(false);
    }

    public void OnWallCollision()
    {
        rigBody.velocity = Vector2.zero;
        StartCoroutine(StartFadingOut());
    }

    public IEnumerator StartFadingOut()
    {
        yield return new WaitForSeconds(StartFadingAfter);
        fadeOut = true;
    } 


    #endregion


}
