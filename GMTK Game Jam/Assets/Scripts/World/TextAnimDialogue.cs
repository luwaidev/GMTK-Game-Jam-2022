using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;


public class TextAnimDialogue : MonoBehaviour
{
    public static TextAnimDialogue instance;
    public Transform text;
    public bool textRunning;
    public int index;
    public bool inputed;
    public bool textReady;

    public bool isBattle;

    [Header("Display")]
    public Transform cinematicBars;
    public float defaultPos;
    public float activePos;

    public Transform textBox;
    public float textBoxHidden;
    public float textBoxShown;

    public Transform cameraPosition;

    private void Awake()
    {
        instance = this;
    }
    public void StartText(Transform _text)
    {
        CameraController.instance.target = cameraPosition;
        text = _text;

        textRunning = true;

        index = 0;

        text.GetChild(0).gameObject.SetActive(true);
        PlayerController.instance.state = PlayerController.State.Cutscene;
    }

    public void NextText()
    {
        textReady = true;
        PlayerController.instance.state = PlayerController.State.Cutscene;
    }

    private void Update()
    {
        cinematicBars.localScale = new Vector2(1, Mathf.Lerp(cinematicBars.localScale.y, textRunning ? activePos : defaultPos, 0.125f));

        textBox.position = new Vector2(500, Mathf.Lerp(textBox.position.y, textRunning ? textBoxShown : textBoxHidden, 0.125f));

        if (Input.GetKeyDown(KeyCode.Space) && textRunning && textReady) inputed = true;
        if (inputed && textReady)
        {
            text.GetChild(index).gameObject.SetActive(false);
            index++;

            if (index < text.childCount)
            {
                text.GetChild(index).gameObject.SetActive(true);
                inputed = false;
                textReady = false;
            }
            else
            {
                index = 0;
                textRunning = false;

                inputed = false;
                textReady = false;
                PlayerController.instance.state = PlayerController.State.Idle;

                if (text.name == "Intro Scene")
                {
                    GameManager.instance.StartBattle();
                }
                if (GameManager.instance.battleStarted) WaveManager.manager.StartWaves();

                CameraController.instance.target = PlayerController.instance.transform;
                Destroy(text.gameObject);
            }
        }

        if (!textReady && textRunning && Input.GetKeyDown(KeyCode.Space))
        {
            text.GetChild(index).GetComponent<TextAnimatorPlayer>().SkipTypewriter();

        }
    }
}
