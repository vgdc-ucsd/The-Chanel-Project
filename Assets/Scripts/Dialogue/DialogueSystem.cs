using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    
    public string CharacterName;
    public string Option1;
    public string Option2;
    public TextMeshProUGUI Text;
    public TextMeshProUGUI CharacterNameText;
    public TextMeshProUGUI Option1Text;
    public TextMeshProUGUI Option2Text;
    public List<Dialogue> DialogueText;
    public Sprite DialogueScrollNormal;
    public Sprite DialogueScrollRipped;
    public Image DialogueScroll;

    private Queue<Dialogue> dialogueQueue;    
    private Coroutine dialogueCoroutineRef;
    private float startTime;
    private bool finishedText;

    void Start() {
        CharacterNameText.text = CharacterName;
        Option1Text.text = Option1;
        Option2Text.text = Option2;
        DialogueScroll.sprite = DialogueScrollNormal;

        Option1Text.transform.parent.gameObject.SetActive(false);
        Option2Text.transform.parent.gameObject.SetActive(false);

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
                //CharacterName.text = currentDialogue.CharacterName;
                Text.maxVisibleCharacters = 0;
                dialogueCoroutineRef = StartCoroutine(DialogueCoroutine(currentDialogue));
            }
            else {
                // last dialogue
                DialogueScroll.sprite = DialogueScrollRipped;
                Option1Text.transform.parent.gameObject.SetActive(true);
                Option2Text.transform.parent.gameObject.SetActive(true);
                //EventManager.Instance.FinishEvent();
                //gameObject.SetActive(false);
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
