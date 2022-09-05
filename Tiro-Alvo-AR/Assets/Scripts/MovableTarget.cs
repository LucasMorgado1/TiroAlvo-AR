using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableTarget : Target
{
    [SerializeField] private float verticalLimit = 0;
    [SerializeField] private float horizontalLimit = 0;
    [SerializeField] private float speed = 10;

    private bool directionSwitch;
    private bool direction;

    private Vector2 maxPos;
    private Vector2 minPos;

    protected override void Start()
    {
        base.Start();
        DirectionDefinition();
    }

    protected override void Update()
    {
        base.Update();
        TargetMovement(maxPos, minPos);
    }

    private void DirectionDefinition()
    {
        int dir = Random.Range(0,1);

        direction = dir == 0 ? true : false;

        if (direction)
        {
            maxPos = new Vector2(transform.position.x + horizontalLimit, transform.position.y);
            minPos = new Vector2(transform.position.x - horizontalLimit, transform.position.y);
            return;
        }

        maxPos = new Vector2(transform.position.x, transform.position.y + verticalLimit);
        minPos = new Vector2(transform.position.x, transform.position.y - verticalLimit);
    }

    private void TargetMovement(Vector3 maxPoint, Vector3 minPoint)
    {
        if (directionSwitch)
        {
            PositionLerp(maxPoint);
            if (Vector3.Distance(transform.position, maxPoint) <= 0) directionSwitch = false;
            return;
        }

        PositionLerp(minPoint);
        if (Vector3.Distance(transform.position, minPoint) <= 0) directionSwitch = true;
    }

    private Vector3 PositionLerp(Vector3 point)
    {
        return Vector3.Lerp(transform.position, point, speed * Time.deltaTime);
    }
}
