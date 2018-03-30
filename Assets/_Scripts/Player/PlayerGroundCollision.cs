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
            if(IsCollidingGround(collision))
            {
                player.GroundCollision();
            }
        }
    }

    private bool IsCollidingGround(Collision2D collision)
    {
        if (collision.contacts.Length == 0)
            return false;

        var collisionContactPoint = collision.contacts[0].point;
        var checkIfBelowPlayer = collisionContactPoint.y < player.Position.y;

        return checkIfBelowPlayer;
    }

}
