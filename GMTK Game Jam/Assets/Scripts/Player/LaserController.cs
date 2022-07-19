using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Timer());
    }

    // Update is called once per frame
    void Update()
    {

        // Get mouse input
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePositionTP = (mousePosition - (Vector2)PlayerController.instance.transform.position).normalized;
        transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(mousePositionTP.y, mousePositionTP.x) * Mathf.Rad2Deg);
        transform.position = mousePositionTP * 8.5f + (Vector2)PlayerController.instance.transform.position;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.tag == "Player Bullet")
        {
            if (other.tag == "Enemy")
            {
                if (other.GetComponent<EnemyInterface>() != null)
                {
                    other.GetComponent<EnemyInterface>().Hit(damage, (Vector2)transform.position);
                }
            }

        }

    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.15f);
        Destroy(gameObject);
    }
}
