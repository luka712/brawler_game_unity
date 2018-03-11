using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(Tags.Player))
        {
            collision.gameObject.transform.parent.gameObject.transform.SetParent(this.transform.parent);
            collision.gameObject.transform.parent.gameObject.transform.GetComponent<Rigidbody2D>().gravityScale = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Player))
        {
            collision.gameObject.transform.parent.gameObject.transform.SetParent(null);
            collision.gameObject.transform.parent.gameObject.transform.GetComponent<Rigidbody2D>().gravityScale = 1.5f;
        }
    }
}
