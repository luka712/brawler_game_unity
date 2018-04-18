
using UnityEngine;

/// <summary>
/// Snaps unity game object to pixel on screen.
/// </summary>
public class SnapToPixel : MonoBehaviour
{
    #region Editor variables

    public float _pixelsPerUnit = 8;

    #endregion

    #region Methods

    private void LateUpdate()
    {
        transform.position = transform.position.ChangeComponentX
            (Mathf.Round(transform.position.x * _pixelsPerUnit) / _pixelsPerUnit);

        transform.position = transform.position.ChangeComponentY
          (Mathf.Round(transform.position.y * _pixelsPerUnit) / _pixelsPerUnit);
    }

    #endregion

}

