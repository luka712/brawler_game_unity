using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToPixel : MonoBehaviour
{
    public float pixelsPerUnit = 8;

    private void LateUpdate()
    {
        transform.position = transform.position.ChangeComponentX
            (Mathf.Round(transform.position.x * pixelsPerUnit) / pixelsPerUnit);

        transform.position = transform.position.ChangeComponentY
          (Mathf.Round(transform.position.y * pixelsPerUnit) / pixelsPerUnit);
    }
}

