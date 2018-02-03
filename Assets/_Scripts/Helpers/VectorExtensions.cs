
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 ToVector2(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector3 ToVector3(this Vector2 vector, float z = 0f)
    {
        return new Vector3(vector.x, vector.y, z);
    }

    public static Vector3 ChangeComponentX(this Vector3 vector, float x)
    {
        return new Vector3(x, vector.y, vector.z);
    }

    public static Vector3 ChangeComponentY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, y, vector.z);
    }
}

