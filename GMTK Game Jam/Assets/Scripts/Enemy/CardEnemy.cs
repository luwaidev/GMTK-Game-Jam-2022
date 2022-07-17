using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEnemy : MonoBehaviour, EnemyInterface
{
    public enum State { Idle, Follow, Attack, Hit }

    [Header("References")]
    public State state;
    private Rigidbody2D rb;
    private Animator anim;
    public string cardType;
    public PlayerController p;
    int health;

    [Header("Movement")]
    public float followDistance;
    public float followSpeed;
    public float offsetSpeed;
    public Vector2 v;
    public float d;

    [Header("Combat")]
    public Vector2 bulletPosition;
    public Transform gun;
    public float gunDistance;
    public GameObject bullet;
    public float knockbackTime;
    public float knockbackStrength;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }
    // Start is called before the first frame update
    void Start()
    {

        p = PlayerController.instance;

        state = State.Follow;
        NextState();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerPositionTE = (p.transform.position - transform.position).normalized;
        gun.transform.position = playerPositionTE * gunDistance + (Vector2)transform.position;
        gun.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(playerPositionTE.y, playerPositionTE.x) * Mathf.Rad2Deg);

        rb.velocity = v;
    }

    public void Hit(int damage, Vector2 position)
    {
        health -= damage;
        bulletPosition = position;

        state = State.Hit;
    }

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


            yield return null;
        }

        NextState();
    }

    IEnumerator FollowState()
    {
        d = Mathf.Sign(Random.Range(-1, 1));
        while (state == State.Follow)
        {
            Vector2 direction = p.transform.position - transform.position;
            v = followSpeed * direction.normalized;
            v += d * new Vector2(v.y, -v.x) * offsetSpeed;

            anim.Play(cardType + " Walk" + (direction.y > 0 ? " Back" : " Front"), 0);

            if (Vector2.Distance(transform.position, p.transform.position) < followDistance)
            {
                state = State.Attack;
            }
            yield return null;
        }

        NextState();
    }

    IEnumerator AttackState()
    {
        Vector2 direction = p.transform.position - transform.position;
        anim.Play(cardType + " Attack" + (direction.y > 0 ? " Back" : " Front"), 0);
        v = Vector2.zero;

        yield return new WaitForSeconds(1.15f);

        Instantiate(bullet, gun.transform.position, gun.transform.rotation);

        anim.Play(cardType + " Idle" + (direction.y > 0 ? " Back" : " Front"), 0);
        yield return new WaitForSeconds(1f);
        state = State.Follow;
        NextState();
    }

    IEnumerator HitState()
    {
        v = (bulletPosition - (Vector2)transform.position).normalized * knockbackStrength;
        yield return new WaitForSeconds(knockbackTime);

        state = State.Follow;
        NextState();
    }

}
