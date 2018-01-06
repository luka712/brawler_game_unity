using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldWrapperLeftRight : MonoBehaviour
{

    [SerializeField]
    private float left;
    [SerializeField]
    private float right;


    private void Update()
    {
        if (this.gameObject.transform.position.x > right)
        {
            this.gameObject.transform.position = this.gameObject.transform.position.ChangeComponentX(left);
        }
        else if (this.gameObject.transform.position.x < left)
        {
            this.gameObject.transform.position = this.gameObject.transform.position.ChangeComponentX(right);
        }
    }
}
