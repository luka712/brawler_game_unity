using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [SerializeField]
    private List<Vector2> wayPoints = new List<Vector2>();
    [SerializeField]
    private float moveSpeed = 1f;

    private int index = 0;
    private Vector2 currentStartPoint = Vector2.zero;
    private Vector2 currentEndPoint = Vector2.zero;


    private void Start()
    {
        if (wayPoints.Count >= 1)
        {
            currentStartPoint = wayPoints[index];
            currentEndPoint = wayPoints[++index];

            this.transform.position = currentStartPoint.ToVector3(this.transform.position.z);
        }
        else
        {
            currentEndPoint = this.transform.position.ToVector2();
        }
    }

    private void Update()
    {
        if (wayPoints.Count > 1 && Mathf.Abs(Vector2.Distance(currentEndPoint, this.transform.position.ToVector2())) < 1f)
        {
            currentStartPoint = currentEndPoint;
            index++;
            if (index >= wayPoints.Count)
            {
                index = 0;
            }
            currentEndPoint = wayPoints[index];
        }

        var direction = (currentEndPoint - currentStartPoint).normalized;

        this.transform.Translate((direction * moveSpeed * Time.deltaTime).ToVector3(0)); 
    }
}
