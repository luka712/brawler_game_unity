using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlayerZugai : Player
{
    // code variables
    private bool isBulletSpawning = false;
    private int previousAnimatorStateHash;
    private int specialBulletHash;
    private int warmpUpBulletHash;
    private bool detonateBullet;
    private ZugaiBullet bullet;

    // editor variables
    public string _specialBulletTagName = "Zugai_Special_Attack";
    public GameObject _bulletObject;
    public Vector2 _shootingOffset = new Vector2(1f, 0f);

    public Animator Animator => GetComponent<Animator>();

    protected override void Start()
    {
        base.Start();
        State.Push(new PlayerIdleState());

        specialBulletHash = Animator.StringToHash(_specialBulletTagName);
        previousAnimatorStateHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        bullet = _bulletObject.GetComponent<ZugaiBullet>();
        bullet.gameObject.SetActive(false);
    }


    private void FixedUpdate()
    {
        
        if(State.Count > 0)
            State.Peek().HandleInput(this);
    }

    protected override void Update()
    {
        base.Update();
        if(State.Count > 0)
            State.Peek().Update(this);
    }

    private void FixedUpdateOld()
    {

        if (isBulletSpawning)
        {
            // player animation ending check
            if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == specialBulletHash)
            {
                isBulletSpawning = false;
                SpawnBullet();
            }
        }


        if (Input.GetButtonDown(_attackButton))
        {
            animator.SetTrigger(AnimationVariables.Attack);
        }
        else if (Input.GetButtonDown(_specialAttackButton) && !bullet.gameObject.activeInHierarchy)
        {
            animator.SetTrigger(AnimationVariables.SpecialAttack);
            previousAnimatorStateHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            isBulletSpawning = true;
            detonateBullet = false;
        }
        else if (Input.GetButtonDown(_specialAttackButton) && bullet.gameObject.activeInHierarchy && !detonateBullet)
        {
            bullet.StartDetonateSequence();
        }
    }

    private void SpawnBullet()
    {
        var direction = transform.localScale.x > 0 ? 1 : -1;
        bullet.gameObject.SetActive(true);
        bullet.gameObject.transform.position = this.transform.position + (new Vector2(_shootingOffset.x * direction, _shootingOffset.y)).ToVector3();

        // for some reason, when set active again rigidbody is not set to kinematic
        bullet.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        bullet.gameObject.transform.localScale = bullet.transform.localScale.ChangeComponentX
            (bullet.transform.localScale.x * direction);
        bullet.Fire(new Vector2(direction, 0));
    }



}
