using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State { Idle, Run, Roll, Hit }

    [Header("References")]
    public State state;
    private CombatController cc;
    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 lastInput;
    public Vector2 velocity;


    [Header("Movement Settings")]
    public float speed;
    public float rollSpeed;
    public float rollTime;

    [Header("Mouse Settings")]
    private Vector2 mousePosition;
    private Vector2 mousePositionTP; // Mouse position to player



    //////////////// Unity Functions ///////////////

    private void Awake()
    {

        rb = gameObject.GetComponent<Rigidbody2D>();
        cc = gameObject.GetComponent<CombatController>();
    }

    // Start is called before the first frame update
    public void Start()
    {
        state = State.Idle;
        NextState();
    }

    // Update is called once per frame
    void Update()
    {

        // Get keyboard movement input
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (input != Vector2.zero) lastInput = input;

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePositionTP = (mousePosition - (Vector2)transform.position).normalized;


    }

    // Fixed Update, runs on same speed as physics engine
    private void FixedUpdate()
    {
        rb.velocity = velocity;
    }

    //////////////// Functions ///////////////

    //////////////// Coroutine ///////////////

    //////////////// States ///////////////
    void NextState()
    {
        string methodName = state.ToString() + "State";

        // Get method
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);

        StartCoroutine((IEnumerator)info.Invoke(this, null)); // Call the next state
    }

    IEnumerator IdleState()
    {
        while (state == State.Idle)
        {
            if (input != Vector2.zero) state = State.Run;

            // Check to roll
            if (Input.GetKeyDown(KeyCode.LeftShift)) state = State.Roll;

            yield return null;
        }

        NextState();
    }

    IEnumerator RunState()
    {
        while (state == State.Run)
        {
            // Set movement velocity
            velocity = input * speed;

            // Check to roll
            if (Input.GetKeyDown(KeyCode.LeftShift)) state = State.Roll;

            // Check for stopping
            if (velocity == Vector2.zero) state = State.Idle;

            yield return null;
        }

        NextState();
    }

    IEnumerator RollState()
    {
        // Move velocity
        velocity = rollSpeed * lastInput;

        // Wait roll time
        yield return new WaitForSeconds(rollTime);


        state = State.Idle;
        velocity = Vector2.zero;
        NextState();
    }


}
