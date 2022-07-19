using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public bool bruh;
    public MMFeedbacks fade;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            StartCoroutine(timer());
        }
    }

    IEnumerator timer()
    {
        if (bruh) yield break;
        bruh = true;
        fade.PlayFeedbacks();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Game");
    }

}
