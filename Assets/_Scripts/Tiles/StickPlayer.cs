using UnityEngine;

/// <summary>
/// Stick player interface.
/// </summary>
public interface IStickPlayerInterface
{
    bool IsStickedToParent { get; }
    void StickToParent(Transform parentTransform);
    void UnstickFromParent();
}

public class StickPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Player))
        {
            var player = collision.gameObject.GetComponentInParent<IStickPlayerInterface>();
            if (!player.IsStickedToParent && this.transform.rotation.eulerAngles.z == 0) 
            {
                player.StickToParent(this.gameObject.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Player))
        {
            var player = collision.gameObject.GetComponentInParent<IStickPlayerInterface>();
            if (player.IsStickedToParent)
            {
                player.UnstickFromParent();
            }
        }
    }
}
