using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaCollisions : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Player))
        {
            var player = collision.gameObject.GetComponentInParent<Player>();
            player.BlowUpPlayer(Vector2.up);
        }
    }
}
