using System.Collections.Generic;
using UnityEngine;


public class LavaCollisions : MonoBehaviour
{
    #region Public Fields

    public float _minForce = 2f;
    public float _maxForce = 4f;

    #endregion

    #region Unity Methods

    Dictionary<string, object> dictionary = new Dictionary<string, object>(); 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Player))
        {
            var value = dictionary["nekiKey"];
            var player = collision.gameObject.GetComponentInParent<Player>();
            player.BlowUpPlayer(Vector2.up, _minForce, _maxForce);
        }

        List<string> list = new List<string>();
        foreach(var some in list)
        {

        }

        LavaCollisions.Display();
    }

    public static void Display() { }

    #endregion
}
