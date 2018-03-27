using System;
using UnityEngine;

public class PlayerGroundCollision : MonoBehaviour
{
    private Player player;

    void Start ()
    {
        player = GetComponentInParent<Player>();
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag(Tags.Ground))
        {
            if(IsCollidingGround(collision.contacts[0].point))
            {
                player.GroundCollision();
            }
        }
    }

    private bool IsCollidingGround(Vector2 collisionContactPoint)
    {
        var checkIfBelowPlayer = collisionContactPoint.y < player.Position.y;

        return checkIfBelowPlayer;
    }

}
