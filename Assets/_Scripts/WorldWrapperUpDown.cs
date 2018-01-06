using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldWrapperUpDown : MonoBehaviour {


    [SerializeField]
    private float up;
    [SerializeField]
    private float down;


    private void Update()
    {
        if (this.gameObject.transform.position.y < down)
        {
            this.gameObject.transform.position = this.gameObject.transform.position.ChangeComponentY(up);
        }
        else if (this.gameObject.transform.position.y > up)
        {
            this.gameObject.transform.position = this.gameObject.transform.position.ChangeComponentY(down);
        }
    }
}
