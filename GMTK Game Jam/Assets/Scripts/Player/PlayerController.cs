using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State { Idle, Run, Roll, Hit, Cutscene, Dead }

    [Header("References")]
    public State state;
    public static PlayerController instance;
    private CombatController cc;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private BoxCollider2D boxCollider;
    private CircleCollider2D bounceCollider;
    private Animator anim;
    private Vector2 input;
    private Vector2 lastInput;
    public Vector2 velocity;

    [Header("Health and Damage Settings")]
    public int health;
    public float knockbackStrength;
    public float knockbackTime;
    public Vector2 enemyPosition;

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
        instance = this;
        rb = gameObject.GetComponent<Rigidbody2D>();
        cc = gameObject.GetComponent<CombatController>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        bounceCollider = gameObject.GetComponent<CircleCollider2D>();
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
        if (state != State.Roll && state != State.Hit) rb.velocity = velocity;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Enemy" && state != State.Hit)
        {
            StopAllCoroutines();
            Vector2 enemyTP = transform.position - other.transform.position;
            // Move velocity    
            rb.AddForce(knockbackStrength * enemyTP.normalized);
            StartCoroutine(HitState());
        }
    }
    //////////////// Functions ///////////////
    public void Hit(Vector2 position)
    {
        state = State.Hit;
        enemyPosition = position;
    }

    public string GetDirection()
    {
        float angle = Mathf.Atan2(mousePositionTP.y, Mathf.Abs(mousePositionTP.x)) * Mathf.Rad2Deg;
        if (angle < -45)
        {
            sr.flipX = mousePositionTP.x < 0;
            return "1";
        }
        else if (angle <= 0)
        {
            sr.flipX = mousePositionTP.x < 0;
            return "2";
        }
        else if (angle <= 45)
        {
            sr.flipX = mousePositionTP.x > 0;
            return "4";
        }
        else if (angle <= 90)
        {
            sr.flipX = mousePositionTP.x < 0;
            return "3";
        }
        return "0";

    }
    public string GetDirectionForRoll()
    {
        if (lastInput == new Vector2(0, -1))
        {
            sr.flipX = false;
            return "1";
        }
        else if (lastInput.x == 1 && lastInput.y == -1 || lastInput.y == 0)
        {
            sr.flipX = lastInput.x < 0;
            return "2";
        }
        else if (lastInput == new Vector2(0, 1))
        {
            sr.flipX = false;
            return "3";
        }
        else if (lastInput.x == 1 && lastInput.y == -1 || lastInput.y == 1)
        {
            sr.flipX = lastInput.x > 0;
            return "3";
        }

        return "0";

    }
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

            anim.Play((cc.currentWeapon + 1) + " Player Idle " + GetDirection(), 0);

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


            anim.Play((cc.currentWeapon + 1) + " Player Walk " + GetDirection(), 0);
            yield return null;
        }

        NextState();
    }

    IEnumerator RollState()
    {

        velocity = Vector2.zero;
        // Move velocity     
        rb.AddForce(lastInput * rollSpeed);

        cc.StopAllCoroutines();
        cc.StartCoroutine(cc.RollState(rollTime));

        boxCollider.enabled = false;
        bounceCollider.enabled = true;

        anim.Play((cc.currentWeapon + 1) + " Player Roll " + GetDirectionForRoll(), 0);
        // Wait roll time
        yield return new WaitForSeconds(rollTime);

        anim.Play((cc.currentWeapon + 1) + " Player Land " + GetDirectionForRoll(), 0);

        yield return new WaitForSeconds(rollTime);

        boxCollider.enabled = true;
        bounceCollider.enabled = false;

        state = State.Idle;
        velocity = Vector2.zero;
        NextState();
    }

    IEnumerator HitState()
    {

        // Move velocity     
        rb.AddForce((enemyPosition - (Vector2)transform.position).normalized * -knockbackStrength);

        cc.StopAllCoroutines();
        cc.StartCoroutine(cc.RollState(knockbackTime));

        health--;

        boxCollider.gameObject.SetActive(false);
        bounceCollider.gameObject.SetActive(true);

        // Wait roll time
        yield return new WaitForSeconds(knockbackTime);

        boxCollider.gameObject.SetActive(true);
        bounceCollider.gameObject.SetActive(false);

        state = State.Idle;
        velocity = Vector2.zero;
        NextState();
    }

    IEnumerator CutsceneState()
    {
        velocity = Vector2.zero;
        while (state == State.Cutscene)
        {
            cc.state = CombatController.State.Cutscene;
            yield return null;
        }

        cc.state = CombatController.State.Idle;
        NextState();
    }

    IEnumerator DeadState()
    {
        velocity = Vector2.zero;
        yield return new WaitForSeconds(10);
    }
}
