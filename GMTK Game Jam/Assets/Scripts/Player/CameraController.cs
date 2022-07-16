using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Targeting Settings")]

    public Transform target;
    public float speed;
    public Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPosition = target.position + (Vector3)offset;
        targetPosition.z = -10;

        transform.position = Vector3.Lerp(transform.position, targetPosition, speed);
    }
}
