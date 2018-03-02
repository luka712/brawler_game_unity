using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerZugai : Player
{
    // code variables
    private bool isBulletSpawning = false;
    private int previousAnimatorStateHash;
    private int specialBulletHash;

    // editor variables
    public string specialBulletTagName = "Zugai_Special_Attack";
    public GameObject bulletObject;
    public Vector2 shootingOffset = new Vector2(1f, 0f);
    public string attackButton = "Fire_P1";
    public string specialAttackButton = "Fire2_P1";
    public List<Bullet> bullets = new List<Bullet>();



    protected override void Start()
    {
        base.Start();

        specialBulletHash = Animator.StringToHash(specialBulletTagName);
        previousAnimatorStateHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

    }

    private void FixedUpdate()
    {

        if (isBulletSpawning)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == specialBulletHash)
            {
                isBulletSpawning = false;
                SpawnBullet();
            }
        }

        if (Input.GetButtonDown(attackButton))
        {
            animator.SetTrigger(ZugaiAnimations.Attack);
        }
        else if (Input.GetButtonDown(specialAttackButton))
        {
            animator.SetTrigger(ZugaiAnimations.SpecialAttack);
            previousAnimatorStateHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            isBulletSpawning = true;
        }
    }

    private void SpawnBullet()
    {
        var bullet = bullets.FirstOrDefault(x => !x.isActiveAndEnabled);
        var direction = transform.localScale.x > 0 ? 1 : -1;
        if (bullet == null)
        {
            bullet = Instantiate(bulletObject, 
                this.transform.position + (new Vector2(shootingOffset.x * direction, shootingOffset.y)).ToVector3(), 
                Quaternion.identity).GetComponent<Bullet>();
            bullets.Add(bullet);
        }
        else
        {
            bullet.gameObject.SetActive(true);
            bullet.gameObject.transform.position = this.transform.position + (new Vector2(shootingOffset.x * direction, shootingOffset.y)).ToVector3();
        }

        // for some reason, when set active again rigidbody is not set to kinematic
        bullet.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        bullet.gameObject.transform.localScale = bullet.transform.localScale.ChangeComponentX
            (bullet.transform.localScale.x * direction);
        bullet.Fire(new Vector2(direction, 0));
    }
}
