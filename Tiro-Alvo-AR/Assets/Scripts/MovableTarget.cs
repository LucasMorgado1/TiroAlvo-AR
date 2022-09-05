using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableTarget : Target
{
    public float moveSpeed = 200f;
    public float frequency = 2f;
    public float magnitude = 200f;

    Camera cam;

    private Vector3 pos;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        pos = transform.position;
        cam = Camera.main;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        transform.LookAt(cam.transform);

        transform.position += transform.right * Time.deltaTime * moveSpeed;
    }
}
