using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.Feedbacks;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    public Animator transition;

    [Header("Scene Loading Settings")]
    [SerializeField] float sceneTransitionTime;
    public bool loaded;
    public bool loadingScene;

    [Header("Score")]
    public int score;


    [Header("Intro")]
    public bool started;
    public bool cstarted;
    public Transform cameraPosition;
    public Transform battlePosition;
    public float playerTriggerBattle;
    public GameObject spawnBox;
    public GameObject captureBox;
    public MMFeedbacks fade;


    [Header("Battle")]
    public Transform battleText;
    public bool battleStarted;
    public AudioSource battleAudio;

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("GameController") != gameObject) Destroy(gameObject);

        instance = this;
        //DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
    }
    public void Load(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }



    public void LoadWithDelay(string sceneName, float delayTime)
    {
        StartCoroutine(Delay(sceneName, delayTime));
    }

    IEnumerator Delay(string sceneName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        StartCoroutine(LoadScene(sceneName));
    }

    public IEnumerator LoadScene(string sceneName)
    {
        Debug.Log("Loading Scene");

        if (loadingScene) yield break;
        loadingScene = true;

        // sound.Play();
        transition.SetTrigger("Transition"); // Start transitioning scene out
        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition

        // Start loading scene
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        load.allowSceneActivation = false;
        while (!load.isDone)
        {
            if (load.progress >= 0.9f)
            {
                load.allowSceneActivation = true;
            }

            yield return null;
        }
        load.allowSceneActivation = true;



        transition.SetTrigger("Transition"); // Start transitioning scene back

        yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition
        loadingScene = false;

        yield return new WaitForSeconds(1);
        instance = this;

        loaded = true;
    }

    private void Update()
    {

    }

    public void StartBattle()
    {
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {

        GameObject c = Instantiate(captureBox, PlayerController.instance.transform.position, Quaternion.identity);
        c.SetActive(true);
        PlayerController.instance.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        fade.PlayFeedbacks();

        yield return new WaitForSeconds(2.5f);
        Destroy(c);
        CameraController.instance.maxPosition = Vector2.one * 1000;
        CameraController.instance.target = battlePosition;
        yield return new WaitForSeconds(2.5f);


        yield return new WaitForSeconds(1);

        spawnBox.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        PlayerController.instance.gameObject.SetActive(true);
        PlayerController.instance.transform.position = new Vector2(50, 0);
        yield return new WaitForSeconds(1);

        CameraController.instance.target = PlayerController.instance.transform;
        PlayerController.instance.state = PlayerController.State.Idle;
        PlayerController.instance.NextState();

        CameraController.instance.maxPosition = new Vector2(52.2f, 3.25f);
        CameraController.instance.minPosition = new Vector2(47.9f, -3.25f);

        CameraController.instance.cameraSpeed = 0.125f;
        battleStarted = true;

        yield return new WaitForSeconds(1);
        WaveManager.manager.StartWaves();



    }

    public IEnumerator Death()
    {

        fade.PlayFeedbacks();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Game");
    }
}
