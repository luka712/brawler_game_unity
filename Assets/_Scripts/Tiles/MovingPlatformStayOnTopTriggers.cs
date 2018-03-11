using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformStayOnTopTriggers : MonoBehaviour
{
    [SerializeField]
    private new string tag = "";

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(tag))
        {
            this.transform.parent = collision.gameObject.transform;
        }
    }

   
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(tag))
        {
            this.transform.parent = null;
        }
    }
}
