using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public TextMeshProUGUI CharacterName;
    public int debugNum;
    public Dialogue[] DialogueText;

    private Queue<Dialogue> dialogueQueue;    
    private Coroutine dialogueCoroutineRef;
    private float startTime;
    private bool finishedText;

    void Start() {
        finishedText = true;
        dialogueQueue = new Queue<Dialogue>(DialogueText);
        if(dialogueQueue.Count > 0) AdvanceDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO use button name from input manager instead of hard coded button
        if(Input.GetKeyDown(KeyCode.Space)) {
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue() {
        if(finishedText) {
            if(dialogueQueue.Count > 0) {
                Dialogue currentDialogue = dialogueQueue.Dequeue();
                if(dialogueCoroutineRef != null) StopCoroutine(dialogueCoroutineRef);
                Text.text = currentDialogue.Line;
                CharacterName.text = currentDialogue.CharacterName;
                Text.maxVisibleCharacters = 0;
                dialogueCoroutineRef = StartCoroutine(DialogueCoroutine(currentDialogue));
            }
            else {
                EventManager.Instance.FinishEvent();
                gameObject.SetActive(false);
            }
        }
        else {
            if(dialogueCoroutineRef != null) StopCoroutine(dialogueCoroutineRef);
            Text.maxVisibleCharacters = Text.text.Length;
            finishedText = true;
        }
    }

    private IEnumerator DialogueCoroutine(Dialogue currentDialogue) {
        finishedText = false;
        while(Text.maxVisibleCharacters < Text.text.Length) {
            startTime = Time.time;
            while(Time.time - startTime < currentDialogue.TextSpeed/1000f) {
                yield return null;
            }
            Text.maxVisibleCharacters++;
        }
        finishedText = true;
    }
}
