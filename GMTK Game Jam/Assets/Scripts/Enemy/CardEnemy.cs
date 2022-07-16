using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEnemy : MonoBehaviour, EnemyInterface
{
    int health;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Hit()
    {
        health -= 1;
    }
}
