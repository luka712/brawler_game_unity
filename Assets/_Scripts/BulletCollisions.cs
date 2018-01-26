using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollisions : MonoBehaviour
{
    [SerializeField]
    public int damage = 10;
    [SerializeField]
    public float force = 5f;

    private Bullet bullet;
    private Rigidbody2D rigBody;

    public int Damage { get { return damage; } }
    public float ForceFeedback { get { return force; } }

    // Use this for initialization
    void Start()
    {
        bullet = GetComponent<Bullet>();
        rigBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag(Tags.Player))
        {
            var player = collision.gameObject.GetComponentInParent<Player>();
            if(bullet.Group != player.Group)
            {
                player.AddDamage(damage, rigBody.velocity * force);
            }
        }
    }
}
