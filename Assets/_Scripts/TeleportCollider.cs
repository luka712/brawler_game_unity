using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCollider : MonoBehaviour
{

    [SerializeField]
    private TeleportCollider exit;
    [SerializeField]
    private Vector2 offset = new Vector2(0, 0.5f);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            var spriteDivider = collision.GetComponentInParent<SpriteDivider>();
            spriteDivider.Teleport(exit.transform.position.ToVector2() + offset);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            var player = collision.GetGameObjectComponentInParent<Player>();
            if (player.IsTeleporting == false)
            {
                player.CanTeleport = true;
            }
        }
    }
}
