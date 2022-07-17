using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimDialogue : MonoBehaviour
{
    public static TextAnimDialogue instance;
    public Transform text;
    public bool textRunning;
    public int index;
    public bool inputed;
    public bool textReady;

    private void Awake()
    {
        instance = this;
    }
    public void StartText(Transform _text)
    {
        text = _text;

        textRunning = true;

        index = 0;

        text.GetChild(0).gameObject.SetActive(true);
    }

    public void NextText()
    {
        textReady = true;
        PlayerController.instance.state = PlayerController.State.Cutscene;
    }

    private void Update()
    {
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

                if (GameManager.instance.battleStarted) WaveManager.manager.StartWaves();
            }
        }
    }
}
