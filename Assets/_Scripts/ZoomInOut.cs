using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ZoomInOut : MonoBehaviour
{
    [SerializeField]
    private List<Transform> playerTransforms = new List<Transform>();
    [SerializeField]
    private Vector2 edgesCheck = new Vector2(3, 3);

    [SerializeField]
    private float scalingFactor = 0.25f;
    [SerializeField]
    private float maxSize = 12.0f; 

    private float initalSize;
    private new Camera camera;


    private void Start()
    {
        camera = GetComponent<Camera>();
        initalSize = camera.orthographicSize;
    }


    private void Update()
    {
        // get max change
        var maxX = playerTransforms.Max(x => Mathf.Abs(x.position.x));
        var maxY = playerTransforms.Max(x => Mathf.Abs(x.position.y));

        float xDistance = 0f;
        float yDistance = 0f;


        // check distances from edge, if above edge
        if(maxX > edgesCheck.x)
        {
            xDistance = Mathf.Abs(maxX) - edgesCheck.x;
        }

        if(maxY > edgesCheck.y)
        {
            yDistance = Mathf.Abs(maxY) - edgesCheck.y;
        }

        // apply sizing
        if(xDistance != 0f || yDistance != 0f)
        {
            var distance = xDistance > yDistance ? xDistance : yDistance;

            camera.orthographicSize = initalSize + (distance * scalingFactor);
        }

        // don't go below this size
        if (camera.orthographicSize < initalSize)
        {
            camera.orthographicSize = initalSize;
        }
        else if(camera.orthographicSize > maxSize)
        {
            camera.orthographicSize = maxSize; 
        } 
    }

}
