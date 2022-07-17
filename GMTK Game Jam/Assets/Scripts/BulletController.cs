using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.tag == "Enemy")
        {
            if (other.tag == "Player")
            {
                other.GetComponent<PlayerController>().Hit(transform.position);
                Destroy(gameObject);
            }
            if (other.tag != "Camera" && other.tag != "Enemy")
            {
                Destroy(gameObject);
            }
        }
        else if (gameObject.tag == "Player Bullet")
        {
            if (other.tag == "Enemy")
            {
                if (other.GetComponent<EnemyInterface>() != null)
                {
                    other.GetComponent<EnemyInterface>().Hit(damage, (Vector2)transform.position);
                }
                Destroy(gameObject);
            }
            if (other.tag != "Player" && other.tag != "Camera") Destroy(gameObject);

        }

    }
}
