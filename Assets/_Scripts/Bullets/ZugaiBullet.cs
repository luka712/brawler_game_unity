using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZugaiBullet : Bullet
{
    private int warmpUpBulletHash;
    private int explodeBulletHash;
    private BoxCollider2D boxCollider;

    // editor variables
    public string _bulletWarmupTagName = "Zugai_Bullet_WarmUp";
    public string _bulletExplodeTagName = "Zugai_Bullet_Explode";
    public string _explodeBulletAnimationTriggerName = "bullet_explode";
    public string _warmUpBulletAnimationTriggerName = "warm_up";
    public int _applyPlayerdamage = 100;
    public float _applyPlayerForce = 15f;

    public bool IsExploding { get; private set; }

    // Use this for initialization
    internal override void Awake()
    {
        base.Awake();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        warmpUpBulletHash = Animator.StringToHash(_bulletWarmupTagName);
        explodeBulletHash = Animator.StringToHash(_bulletExplodeTagName);
    }

    protected override void FixedUpdate()
    {
        if (IsExploding)
        {
            // animation ending
            if (Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == warmpUpBulletHash)
            {
                Explode();
            }
            else if(Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == explodeBulletHash)
            {
                gameObject.SetActive(false);
                boxCollider.enabled = false;
                IsExploding = false;
            }
        }
    }

    private void Explode()
    {
        Animator.SetTrigger(_explodeBulletAnimationTriggerName);
        rigBody.velocity = Vector2.zero;
        boxCollider.enabled = true;
    }

    internal void StartDetonateSequence()
    {
        Animator.SetTrigger(_warmUpBulletAnimationTriggerName);
        IsExploding = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Player))
        {
            var player = collision.gameObject.GetComponentInParent<Player>();
            if (Group != player.Group)
            {
                var direction = (collision.gameObject.transform.position - transform.position)
                    .ToVector2().normalized * _applyPlayerForce;
                if (player.Spawning == false)
                {
                    player.AddDamage(_applyPlayerdamage, direction);
                    if (player.Health <= 0)
                    {
                       // player.GetComponent<SpriteDivider>().Divide(direction);
                    }
                }
            }
        }
    }

}
