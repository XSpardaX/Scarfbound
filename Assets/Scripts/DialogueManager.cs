using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text speakerText;
    public TMP_Text dialogueText;
    public GameObject dialoguePanel;

    public string dialogueToLoad;

    private Dictionary<string, List<DialogueEntry>> sections;
    private bool isRunningDialogue;
    private bool nextPressed;

    private void Awake()
    {
        sections = new Dictionary<string, List<DialogueEntry>>();

        TextAsset dialogueFile = Resources.Load<TextAsset>(dialogueToLoad);
        string[] allLines = dialogueFile.text.Split('\n');

        string currentSection = null;
        List<DialogueEntry> currentEntries = null;

        foreach (string rawLine in allLines)
        {
            string trimmedLine = rawLine.Trim();

            if (trimmedLine.StartsWith("#SECTION"))
            {
                currentSection = trimmedLine.Split(' ')[1];
                currentEntries = new List<DialogueEntry>();
                sections[currentSection] = currentEntries;
                continue;
            }

            if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

            if (currentEntries == null) continue;

            string[] lineParts = trimmedLine.Split(':');
            if (lineParts.Length == 2)
            {
                currentEntries.Add(new DialogueEntry(lineParts[0].Trim(), lineParts[1].Trim()));
            }
        }
    }

    public void OnNextPressed()
    {
        nextPressed = true;
    }

    public void StartDialogue(string sectionName)
    {
        if (isRunningDialogue) return;

        StartCoroutine(RunDialogueSection(sections[sectionName]));
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
