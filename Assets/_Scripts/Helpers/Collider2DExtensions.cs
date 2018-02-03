

using UnityEngine;

public static class Collider2DExtensions
{
    public static bool CompareTag(this Collider2D collider, string tag)
    {
        return collider.gameObject.CompareTag(tag);
    }

    public static T GetGameObjectComponent<T>(this Collider2D collider)
    {
        return collider.gameObject.GetComponent<T>();
    }

    public static T GetGameObjectComponentInParent<T>(this Collider2D collider)
    {
        return collider.gameObject.GetComponentInParent<T>();
    }
}

