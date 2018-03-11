
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    // scripts move parent transform, rotates current transform. Needs to have parent.

    // editor variables
    public List<Vector2> _wayPoints = new List<Vector2>();
    public List<float> _rotations = new List<float>();
    public float _moveSpeed = 1f;
    private float _distancePrecision = 0.1f;

    private Vector2 currentStartPoint = Vector2.zero;
    private Vector2 currentEndPoint = Vector2.zero;
    private bool canMove = true;
    // start distance is inital distance, needs for rotations calc. Distance is current one.
    private float distance = 0f;
    private float startDistance = 0f;
    private Transform parentTransform;

    private int Index { get; set; }
    private int EndIndex
    {
        get
        {
            // start
            if (Index >= _wayPoints.Count - 1)
            {
                return 0;
            }
            return Index + 1;
        }
    }

    private void Start()
    {
        parentTransform = this.transform.parent;
        if (_wayPoints.Count >= 1)
        {
            currentStartPoint = _wayPoints[Index++];
            if (_wayPoints.Count > 1)
            {
                currentEndPoint = _wayPoints[EndIndex];
            }
            parentTransform.position = currentStartPoint.ToVector3(parentTransform.position.z);
        }
        else
        {
            currentEndPoint = parentTransform.position.ToVector2();
        }
    }

    private void Update()
    {

        if (_wayPoints.Count > 1 && canMove)
        {
            distance = Mathf.Abs(Vector2.Distance(currentEndPoint, parentTransform.position.ToVector2()));
            if (distance < _distancePrecision)
            {
                Rotate(Index, EndIndex, true);
                canMove = false;
                StartCoroutine(NextWaypoint());
            }
        }

        if (canMove)
        {
            var direction = (currentEndPoint - currentStartPoint).normalized;
            parentTransform.Translate((direction * _moveSpeed * Time.deltaTime).ToVector3(0));
            Rotate(Index, EndIndex);
        }

    }

    private IEnumerator NextWaypoint()
    {
        yield return new WaitForSeconds(5f);
        currentStartPoint = currentEndPoint;
        Index++;
        if (Index >= _wayPoints.Count)
        {
            Index = 0;
        }
        currentEndPoint = _wayPoints[EndIndex];
        // need when calcing rotations
        startDistance = Mathf.Abs(Vector2.Distance(currentEndPoint, parentTransform.position.ToVector2()));
        canMove = true;
    }

    private void Rotate(int startIndex, int endIndex, bool rotationEnd = false)
    {
        if (_wayPoints.Count >= startIndex && _wayPoints.Count >= endIndex)
        {
            var currentStartPosition = _wayPoints[startIndex];
            var currentEndPosition = _wayPoints[endIndex];

            if (_rotations.Count >= endIndex && _rotations.Count >= startIndex)
            {
                var currentStartRotation = _rotations[startIndex];
                var currentEndRotation = _rotations[endIndex];


                var distanceLength = 1f;
                if (distance > _distancePrecision)
                {
                    if (startDistance != 0)
                    {
                        distanceLength = Mathf.Abs((startDistance - distance) / startDistance);
                    }
                    else
                    {
                        distanceLength = 0;
                    }

                }

                var rotation = currentEndRotation;
                if (!rotationEnd)
                {
                    if (currentEndRotation > currentStartRotation)
                        rotation = currentStartRotation + Mathf.Abs(currentEndRotation - currentStartRotation) * distanceLength;
                    else
                        rotation = currentStartRotation - Mathf.Abs(currentStartRotation - currentEndRotation) * distanceLength;

                }
                this.transform.rotation = Quaternion.Euler(0, 0, rotation);
            }
        }
    }
}
