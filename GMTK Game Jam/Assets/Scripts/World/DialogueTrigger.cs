using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Transform text;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (TextAnimDialogue.instance.textRunning) return;
        if (other.tag == "Player")
        {
            TextAnimDialogue.instance.StartText(text);
        }
    }
}
