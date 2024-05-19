using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    
    public string CharacterName;
    public string Option1;
    public string Option1HighlightText;
    public string Option2;
    public string Option2HighlightText;
    public TextMeshProUGUI Text;
    public TextMeshProUGUI CharacterNameText;
    public List<Dialogue> DialogueText;
    public Sprite DialogueScrollNormal;
    public Sprite DialogueScrollRipped;
    public Image DialogueScroll;
    public DialogueButton Option1Button;
    public DialogueButton Option2Button;

    private Queue<Dialogue> dialogueQueue;    
    private Coroutine dialogueCoroutineRef;
    private float startTime;
    private bool finishedText;

    void Start() {
        CharacterNameText.text = CharacterName;
        Option1Button.TMP.text = Option1;
        Option1Button.HighlightTMP.text = Option1HighlightText;
        Option2Button.TMP.text = Option2;
        Option2Button.HighlightTMP.text = Option2HighlightText;
        DialogueScroll.sprite = DialogueScrollNormal;

        Option1Button.gameObject.SetActive(false);
        Option2Button.gameObject.SetActive(false);

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
                Option1Button.gameObject.SetActive(true);
                Option2Button.gameObject.SetActive(true);
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
