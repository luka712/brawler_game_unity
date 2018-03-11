using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bullet : MonoBehaviour
{
    protected Rigidbody2D rigBody;

    // editor variables
    public float speed = 10f;
    public float timeToExpire = 3f;

    internal int Group { get; set; }

    private Animator animator;
    internal Animator Animator
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

    // Use this for initialization
    internal virtual void Awake()
    {
        rigBody = GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate()
    {
        
    }

    internal virtual void Fire(Vector2 one)
    {
        this.gameObject.SetActive(true);
        rigBody.velocity = one * speed;
        StartCoroutine(Timer());
    }

    internal virtual IEnumerator Timer()
    {
        yield return new WaitForSeconds(timeToExpire);
        this.gameObject.SetActive(false);
    }
}
