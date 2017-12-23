using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {


    [SerializeField]
    private float rotation = 300f;


	void Update ()
    {
        this.transform.Rotate(Vector3.forward * rotation * Time.deltaTime);	
	}
}
