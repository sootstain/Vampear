using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    //Making this into a singleton as well :)
    public static DialogueManager Instance;

    private GameObject dialogueContainer;
    public GameObject VNDialogueContainer;
    public GameObject DSDialogueContainer;
    
    //For switching
    private GameObject dialoguePanel;
    private TextMeshProUGUI speakerText;
    private TextMeshProUGUI dialogueText;
    private Image characterImage;
    
    //For the DS Version, to link up in Inspector
    public GameObject DSDialoguePanel;
    public TextMeshProUGUI DSSpeakerText;
    public TextMeshProUGUI DSDialogueText;
    public Image DSCharacterImage;

    //For the VN Version - might clean up later
    public GameObject VNDialoguePanel;
    public TextMeshProUGUI VNSpeakerText;
    public TextMeshProUGUI VNDialogueText;
    public Image VNCharacterImage;
    
    public float typeSpeed = 0.03f;
    //public float delayBetweenLines = 0.5f;

    private DialogueSO currentDialogue;
    private int currentLineIndex = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        VNDialogueContainer.SetActive(false);
        DSDialogueContainer.SetActive(false);
    }
    
    public void StartDialogue(DialogueSO dialogue)
    {
        Debug.Log("Starting the dialogue 2");
        currentDialogue = dialogue;
        currentLineIndex = 0;

        if (dialogue.DialogueType == DialogueType.ShortDSStyle)
        {
            DSSetup();
        }
        else if (dialogue.DialogueType == DialogueType.VisualNovelStyle)
        {
            VNSetup();
        }
        DisplayLine();
    }

    private void DSSetup()
    {
        VNDialogueContainer.SetActive(false);
        DSDialogueContainer.SetActive(true);
        
        dialogueContainer = DSDialogueContainer;
        dialoguePanel = DSDialoguePanel;
        speakerText = DSSpeakerText;
        dialogueText = DSDialogueText;
        characterImage = DSCharacterImage;
    }

    private void VNSetup()
    {
        DSDialogueContainer.SetActive(false);
        VNDialogueContainer.SetActive(true);
        
        dialogueContainer = VNDialogueContainer;
        dialoguePanel = VNDialoguePanel;
        speakerText = VNSpeakerText;
        dialogueText = VNDialogueText;
        characterImage = VNCharacterImage;
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
        dialogueContainer.SetActive(false);
        currentDialogue = null;
    }
}
