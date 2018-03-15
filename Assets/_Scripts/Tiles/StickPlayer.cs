using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickPlayer : MonoBehaviour
{
    // stick player to platform, only if rotation is zero.

    public float _playerGravityScale = 1.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Player))
        {
            var player = collision.gameObject.GetComponentInParent<MovePlayer>();
            if (!player.HasParent && this.transform.eulerAngles.z == 0) 
            {
                player.SetParent(this.transform.parent);
                player.GravityScale = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Player))
        {
            var player = collision.gameObject.GetComponentInParent<MovePlayer>();
            if (player.HasParent)
            {
                player.SetParent(null); // this.transform.parent);
                player.GravityScale = _playerGravityScale;
            }
        }
    }
}
