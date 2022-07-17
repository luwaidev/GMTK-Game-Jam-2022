using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public enum State { Idle, Spawning, Waiting }

    public static WaveManager manager;
    public State state;
    public GameObject[] enemies;

    public int round;

    [Header("Wave Settings")]
    public float preWaveTime;
    public int startSpawnAmount;
    public float enemySpawnTime;

    public Vector2 battleAreaMax;
    public Vector2 battleAreaMin;

    private void Awake()
    {
        manager = this;
    }

    public void StartWaves()
    {
        state = State.Idle;
        NextState();
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
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                state = State.Spawning;
            }
            yield return null;

        }

        NextState();
    }

    IEnumerator SpawningState()
    {
        yield return new WaitForSeconds(preWaveTime);

        for (int i = 0; i < startSpawnAmount + Mathf.Sqrt(round); i++)
        {
            Mathf.Clamp(Random.Range(0, enemies.Length - 1) + Mathf.Sqrt(round) / 2, 0, enemies.Length - 1);
            yield return new WaitForSeconds(enemySpawnTime * Random.Range(0.5f, 0.12f));
        }

        state = State.Idle;
        NextState();
    }
}
