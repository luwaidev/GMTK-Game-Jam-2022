using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public static CameraController instance;
    [SerializeField] Transform target;
    [SerializeField] float cameraSpeed;
    [SerializeField] float deadzone;
    [SerializeField] Vector2 offset;
    [SerializeField] float mouseFollowMagnitude;
    public bool inCutscene;

    public Vector2 maxPosition;
    public Vector2 minPosition;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 targetPosition = (Vector2)target.transform.position + offset;


        // Look towards mouse
        Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - targetPosition;
        if (Mathf.Abs(mousePosition.x) > deadzone || Mathf.Abs(mousePosition.y) > deadzone)
        {
            Vector2 mouseOffset = new Vector2(
                Mathf.Sign(mousePosition.x) * Mathf.Sqrt(Mathf.Abs(mousePosition.x)),
                Mathf.Sign(mousePosition.y) * Mathf.Sqrt(Mathf.Abs(mousePosition.y))) * mouseFollowMagnitude;
            targetPosition += mouseOffset;
        }

        targetPosition.x = Mathf.Clamp(targetPosition.x, (minPosition.x != 0) ? minPosition.x : -100000, (maxPosition.x != 0) ? maxPosition.x : 100000);
        targetPosition.y = Mathf.Clamp(targetPosition.y, (minPosition.y != 0) ? minPosition.y : -100000, (maxPosition.y != 0) ? maxPosition.y : 100000);
        if (!inCutscene) transform.position = (Vector3)Vector2.Lerp(transform.position, targetPosition, cameraSpeed) - Vector3.forward * 10;

    }
}
