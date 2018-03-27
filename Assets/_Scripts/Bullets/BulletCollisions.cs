using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollisions : MonoBehaviour
{
    // editor variables
    public int _damage = 10;
    public float _force = 15f;

    private Bullet bullet;
    private Rigidbody2D rigBody;

    internal int Damage { get { return _damage; } }
    internal float ForceFeedback { get { return _force; } }

    // Use this for initialization
    void Start()
    {
        bullet = GetComponent<Bullet>();
        rigBody = GetComponent<Rigidbody2D>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Player))
        {
            var player = collision.gameObject.GetComponentInParent<Player>();
            if (bullet.Group != player.Group)
            {
                var direction = rigBody.velocity * _force;
                if (player.Spawning == false)
                {
                    player.AddDamage(_damage, direction);
                    if (player.Health <= 0)
                    {
                        //player.GetComponent<SpriteDivider>().Divide(direction);
                    }
                }
                bullet.gameObject.SetActive(false);
            }
        }
    }
}
