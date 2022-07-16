using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public enum State { Idle, Reload, Roll }

    [Header("References")]
    public State state;
    PlayerController p;

    [Header("Mouse Settings")]
    private Vector2 mousePosition;
    private Vector2 mousePositionTP; // Mouse position to player

    [Header("Visual Settings")]

    public GameObject gun;
    public float gunDistance;

    [Header("Weapon Settings")]
    public int currentWeapon;
    public int[] damage;
    public int[] curAmmo;
    public int[] maxAmmo;
    public float[] fireSpeed;
    public float[] reloadTime;
    public GameObject[] bullets;

    private void Awake()
    {
        p = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        state = State.Idle;
        NextState();
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse input
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePositionTP = (mousePosition - (Vector2)transform.position).normalized;

        gun.transform.position = mousePositionTP * gunDistance;
        gun.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(mousePositionTP.y, mousePositionTP.x) * Mathf.Rad2Deg);


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
            if (Input.GetButton("Fire1"))
            {
                yield return new WaitForSeconds(reloadTime[currentWeapon]);
            }

            if (curAmmo[currentWeapon] <= 0)
            {
                state = State.Reload;
            }
            yield return null;

        }

        NextState();
    }

    IEnumerator ReloadState()
    {
        int weapon = currentWeapon;
        yield return new WaitForSeconds(reloadTime[currentWeapon]);

        if (weapon == currentWeapon || state != State.Roll)
        {
            curAmmo[currentWeapon] = maxAmmo[currentWeapon];
            NextState();
        }
    }

    IEnumerator RollState()
    {
        yield return new WaitForSeconds(reloadTime[currentWeapon]);

        currentWeapon = Random.Range(0, 5);

        NextState();
    }
}


