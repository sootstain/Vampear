using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    //Making this into a singleton as well :)
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public Image characterImage;

    public float typeSpeed = 0.03f;
    //public float delayBetweenLines = 0.5f;

    private DialogueSO currentDialogue;
    private int currentLineIndex = 0;
    private bool dialogueActive = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        //if (dialoguePanel != null)
        //    dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueSO dialogue)
    {
        Debug.Log("Starting the dialogue 2");
        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialogueActive = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        DisplayLine();
    }

    public void DisplayLine()
    {
        if (currentLineIndex < currentDialogue.Dialogue.Count)
        {
            DialogueLine line = currentDialogue.Dialogue[currentLineIndex];
            speakerText.text = line.Speaker;
            characterImage.sprite = line.Image;
        
            StopAllCoroutines();
            StartCoroutine(TypeText(line.Text));
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        currentLineIndex++;
        yield return new WaitForSeconds(2f);
        DisplayLine();
    }

    void EndDialogue()
    {
        dialogueActive = false;
        dialoguePanel.SetActive(false);
        currentDialogue = null;
    }
}
