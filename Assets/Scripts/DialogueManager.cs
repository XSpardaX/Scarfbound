using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text speakerText;
    public TMP_Text dialogueText;
    public GameObject dialoguePanel;

    private Dictionary<string, List<DialogueEntry>> sections;
    private bool isRunningDialogue;
    private bool nextPressed;

    public string dialogueToLoad;

    void Awake()
    {
        sections = new Dictionary<string, List<DialogueEntry>>();

        TextAsset file = Resources.Load<TextAsset>(dialogueToLoad);
        string[] lines = file.text.Split('\n');

        string currentSection = null;
        List<DialogueEntry> currentList = null;

        foreach (string uneditedText in lines)
        {
            string line = uneditedText.Trim();

            if (line.StartsWith("#SECTION"))
            {
                currentSection = line.Split(' ')[1];
                currentList = new List<DialogueEntry>();
                sections[currentSection] = currentList;
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (currentList != null)
            {
                string[] parts = line.Split(':');

                if (parts.Length == 2)
                    currentList.Add(new DialogueEntry(parts[0].Trim(), parts[1].Trim()));
            }
        }
    }

    public void OnNextPressed()
    { 
        nextPressed = true;
    }

    public void StartDialogue(string sectionName)
    {
        if (!isRunningDialogue)
        {
            StartCoroutine(RunDialogueSection(sections[sectionName]));

        }
    }

    private IEnumerator RunDialogueSection(List<DialogueEntry> entries)
    {
        isRunningDialogue = true;
        DialogueState.isInDialogue = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        dialoguePanel.SetActive(true);

        foreach (DialogueEntry entry in entries)
        {
            speakerText.text = entry.speaker;
            dialogueText.text = entry.line;

            nextPressed = false;

            yield return new WaitUntil(() =>
                nextPressed ||
                Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.E)
            );

            yield return new WaitForSeconds(0.15f);
        }

        speakerText.text = "";
        dialogueText.text = "";
        dialoguePanel.SetActive(false);

        DialogueState.isInDialogue = false;
        isRunningDialogue = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}