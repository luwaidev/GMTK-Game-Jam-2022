using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class CombatController : MonoBehaviour
{
    public enum State { Intro, Idle, Reload, Roll, Cutscene, Hit }

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
    public int[] curAmmo;
    public int[] maxAmmo;
    public SpriteRenderer[] sprites;
    public float[] reloadTime;
    public GameObject[] bullets;

    [Header("Feedbacks")]
    public MMFeedbacks shootFeedback1;
    public MMFeedbacks shootFeedback2;
    public MMFeedbacks shootFeedback3;
    public MMFeedbacks shootFeedback4;

    private void Awake()
    {
        p = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        state = State.Intro;
        NextState();
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse input
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePositionTP = (mousePosition - (Vector2)transform.position).normalized;

        gun.transform.position = mousePositionTP * gunDistance + (Vector2)transform.position;
        gun.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(mousePositionTP.y, mousePositionTP.x) * Mathf.Rad2Deg);

        sprites[currentWeapon].flipY = mousePositionTP.x < 0;


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

    IEnumerator IntroState()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = false;
        }

        yield return new WaitForSeconds(2.4f);

        sprites[currentWeapon].enabled = true;

        state = State.Idle;
        NextState();
    }
    IEnumerator IdleState()
    {
        while (state == State.Idle)
        {
            if (Input.GetButton("Fire1"))
            {
                Animator a = gun.transform.GetChild(0).GetChild(currentWeapon).GetComponent<Animator>();
                switch (currentWeapon)
                {
                    case 0:
                        Animator muzzle = a.transform.GetChild(0).GetComponent<Animator>();
                        muzzle.Play("Muzzle");
                        a.Play("Fireball Shoot");

                        shootFeedback1.PlayFeedbacks();
                        Instantiate(bullets[currentWeapon], gun.transform.position, gun.transform.rotation);
                        yield return new WaitForSeconds(0.4f);

                        a.Play("Fireball Idle");
                        break;
                    case 1:
                        Animator m = a.transform.GetChild(0).GetComponent<Animator>();
                        int num = Random.Range(1, 6);

                        a.Play("Dice " + num);
                        yield return new WaitForSeconds(0.2f);

                        a.Play("Dice Shoot");
                        m.Play("Muzzle");
                        GameObject b = Instantiate(bullets[currentWeapon], gun.transform.position, gun.transform.rotation);
                        b.GetComponent<BulletController>().damage = num;

                        shootFeedback2.PlayFeedbacks();
                        yield return new WaitForSeconds(0.5f);

                        a.Play("Dice Idle");
                        break;
                    case 2:
                        a.Play("Crystal Shoot");
                        shootFeedback3.PlayFeedbacks();
                        Instantiate(bullets[currentWeapon], gun.transform.position, gun.transform.rotation);
                        yield return new WaitForSeconds(0.3f);
                        a.Play("Crystal Idle");
                        break;
                    case 3:

                        a.Play("Laser Shoot");
                        shootFeedback4.PlayFeedbacks();
                        gun.transform.position = mousePositionTP * 8.5f + (Vector2)PlayerController.instance.transform.position;
                        shootFeedback1.PlayFeedbacks();
                        Instantiate(bullets[currentWeapon], gun.transform.position, gun.transform.rotation);
                        yield return new WaitForSeconds(0.15f);
                        a.Play("Laser Idle");
                        break;
                }
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

    public IEnumerator RollState(float time)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = false;
        }
        yield return new WaitForSeconds(time);


        yield return new WaitForSeconds(time);

        int lastWeapon = currentWeapon;
        while (currentWeapon == lastWeapon)
        {
            currentWeapon = Random.Range(0, 4);
        }
        sprites[currentWeapon].enabled = true;
        NextState();
    }

    public IEnumerator HitState(float time)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = false;
        }
        int lastWeapon = currentWeapon;
        while (currentWeapon == lastWeapon)
        {
            currentWeapon = Random.Range(0, 3);
        }
        sprites[currentWeapon].enabled = true;
        yield return new WaitForSeconds(time);


        NextState();
    }

    IEnumerator CutsceneState()
    {
        while (state == State.Cutscene)
        {
            yield return null;
        }

        NextState();
    }
}


