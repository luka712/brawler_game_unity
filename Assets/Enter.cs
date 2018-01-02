using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enter : MonoBehaviour
{

    private Exit exit;
    
    // Use this for initialization
    void Start()
    {
        exit = GetComponentInChildren<Exit>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(Tags.Player))
        {
            var player = collision.gameObject.GetComponentInParent<Player>();
            if(player != null)
            {
                 player.SetPosition(exit.transform.position);
            }
         
        }
    }
}
