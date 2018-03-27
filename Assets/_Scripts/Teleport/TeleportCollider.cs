using System;
using UnityEngine;

/// <summary>
/// Teleport object state
/// </summary>
public enum TeleportState { Able, NotAble, IsTeleporting }

/// <summary>
/// Teleport object interface
/// </summary>
public interface ITeleportObjectInterface
{
    void TeleportEnter(Vector3 destination);
    void StopTeleporting();
    void TeleportExit();
    TeleportState TeleportState { get; set; }
}

/// <summary>
/// Teleport collider
/// </summary>
public class TeleportCollider : MonoBehaviour
{
    #region Editor Fields

    public TeleportCollider _exit;
    public Vector2 _offset = new Vector2(0, 0.5f);

    #endregion


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            var teleportObject = collision.GetComponentInParent<ITeleportObjectInterface>();
            teleportObject.TeleportEnter(_exit.transform.position.ToVector2() + _offset);
        }
    }

    private void TeleportObject_OnTeleportEnter(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            var teleportObject = collision.GetGameObjectComponentInParent<ITeleportObjectInterface>();
            teleportObject.TeleportExit();
        }
    }
}
