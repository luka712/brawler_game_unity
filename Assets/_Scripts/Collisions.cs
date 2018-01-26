using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions : MonoBehaviour
{

    private const float ForceFromSpike = 400f;
    private float ForceFromTrapBoxSpike = -800f;

    private Player player;
    private MovePlayer movePlayer;
    private SpriteDivider spriteDivider;

    private void Start()
    {
        player = GetComponent<Player>();
        movePlayer = GetComponent<MovePlayer>();
        spriteDivider = GetComponent<SpriteDivider>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag(Tags.Spike))
        {
            player.AddDamage(25, Vector2.up * ForceFromSpike);
            movePlayer.ResetJumpState();
            spriteDivider.Divide(Vector2.up);
        }
        else if (collision.gameObject.CompareTag(Tags.TrapBoxSpike))
        {
            var playerDirection = player.GetComponent<Rigidbody2D>().velocity;
            player.AddDamage(25, playerDirection.normalized * ForceFromTrapBoxSpike);

            var contactPoint = collision.contacts[0].point;
            var trapPosition = collision.gameObject.transform.position.ToVector2();
            var direction = contactPoint - trapPosition;
            direction.Normalize();

            if (player.Health <= 0f)
            {
                spriteDivider.Divide(direction);
            }
        }
    }
}
