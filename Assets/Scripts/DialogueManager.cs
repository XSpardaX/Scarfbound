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
    public GameObject dialogueWispPrefab;
    private Vector3 dialogueWispSpawnOffset = new Vector3(0f, 0f, 0f);
    private Vector3 dialogueWispHoverOffset = new Vector3(0.42f, 0.56f, 0f);
    private float dialogueWispMoveSpeed = 3.5f;
    private float dialogueWispReturnSpeed = 5f;
    private float dialogueWispTouchDistance = 0.25f;

    private float dialogueCameraDistance = 0.84f;
    private float dialogueCameraSideOffset = 0.39f;
    private float dialogueCameraHeight = 0.41f;
    private float dialogueCameraLookHeight = 0.41f;
    private float dialogueCameraPanDuration = 0.88f;

    private Dictionary<string, List<DialogueEntry>> sections;
    private bool isRunningDialogue;
    private bool nextPressed;
    private Transform playerTransform;
    private Transform dialogueCameraTransform;
    private Vector3 savedCameraPosition;
    private Quaternion savedCameraRotation;
    private GameObject activeDialogueWisp;
    private Coroutine dialogueWispHoverRoutine;
    private Coroutine dialogueCameraMaintainRoutine;

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
        if (SfxManager.Instance != null)
        {
            SfxManager.Instance.Play(SfxIds.ButtonPress);
        }

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

        ResolvePlayerTransform();
        ResolveDialogueCamera();

        yield return PanCameraToDialogueCloseup();
        dialogueCameraMaintainRoutine = StartCoroutine(MaintainDialogueCloseupCamera());

        if (ShouldShowDialogueWisp())
        {
            SpawnDialogueWisp();
        }

        CursorController.ApplyUnlocked();

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

        yield return ReturnDialogueWispToPlayer();

        if (dialogueCameraMaintainRoutine != null)
        {
            StopCoroutine(dialogueCameraMaintainRoutine);
            dialogueCameraMaintainRoutine = null;
        }

        yield return PanCameraBackToGameplay();

        DialogueState.isInDialogue = false;
        isRunningDialogue = false;

        CursorController.ApplyGameplay();
    }

    private bool ShouldShowDialogueWisp()
    {
        if (GameState.Instance == null)
        {
            return false;
        }

        bool hasAtLeastOneWisp = GameState.Instance.Wisps >= 1;
        bool hasMoreThanFourLives = GameState.Instance.Lives > 4;

        return hasAtLeastOneWisp || hasMoreThanFourLives;
    }

    private void SpawnDialogueWisp()
    {
        ResolvePlayerTransform();
        if (playerTransform == null)
        {
            return;
        }

        if (dialogueWispPrefab == null)
        {
            return;
        }

        if (activeDialogueWisp != null)
        {
            Destroy(activeDialogueWisp);
        }

        Vector3 spawnPosition = GetPlayerSpawnPoint();
        activeDialogueWisp = Instantiate(dialogueWispPrefab, spawnPosition, Quaternion.identity);

        if (dialogueWispHoverRoutine != null)
        {
            StopCoroutine(dialogueWispHoverRoutine);
        }

        dialogueWispHoverRoutine = StartCoroutine(HoverDialogueWispAtSide());
    }

    private IEnumerator HoverDialogueWispAtSide()
    {
        while (isRunningDialogue && activeDialogueWisp != null)
        {
            ResolvePlayerTransform();
            if (playerTransform == null)
            {
                yield return null;
                continue;
            }

            Vector3 hoverTargetPosition = GetPlayerHoverPoint();
            activeDialogueWisp.transform.position = Vector3.MoveTowards(
                activeDialogueWisp.transform.position,
                hoverTargetPosition,
                dialogueWispMoveSpeed * Time.deltaTime);

            yield return null;
        }

        dialogueWispHoverRoutine = null;
    }

    private IEnumerator ReturnDialogueWispToPlayer()
    {
        if (dialogueWispHoverRoutine != null)
        {
            StopCoroutine(dialogueWispHoverRoutine);
            dialogueWispHoverRoutine = null;
        }

        if (activeDialogueWisp == null)
        {
            yield break;
        }

        ResolvePlayerTransform();
        if (playerTransform == null)
        {
            Destroy(activeDialogueWisp);
            activeDialogueWisp = null;
            yield break;
        }

        float touchDistance = Mathf.Max(0.01f, dialogueWispTouchDistance);

        while (activeDialogueWisp != null)
        {
            Vector3 spawnPoint = GetPlayerSpawnPoint();
            activeDialogueWisp.transform.position = Vector3.MoveTowards(
                activeDialogueWisp.transform.position,
                spawnPoint,
                dialogueWispReturnSpeed * Time.deltaTime);

            float distanceToPlayer = Vector3.Distance(activeDialogueWisp.transform.position, spawnPoint);
            if (distanceToPlayer <= touchDistance)
            {
                Destroy(activeDialogueWisp);
                activeDialogueWisp = null;
                yield break;
            }

            yield return null;
        }
    }

    private Vector3 GetPlayerSpawnPoint()
    {
        return playerTransform.position + dialogueWispSpawnOffset;
    }

    private Vector3 GetPlayerHoverPoint()
    {
        return playerTransform.position
            + (playerTransform.right * dialogueWispHoverOffset.x)
            + (Vector3.up * dialogueWispHoverOffset.y)
            + (playerTransform.forward * dialogueWispHoverOffset.z);
    }

    private void ResolvePlayerTransform()
    {
        if (playerTransform != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    private void ResolveDialogueCamera()
    {
        if (dialogueCameraTransform != null)
        {
            return;
        }

        Player player = playerTransform != null ? playerTransform.GetComponent<Player>() : null;
        if (player != null && player.cameraTransform != null)
        {
            dialogueCameraTransform = player.cameraTransform;
        }
        else if (Camera.main != null)
        {
            dialogueCameraTransform = Camera.main.transform;
        }
    }

    private void GetDialogueCloseupPose(out Vector3 cameraPosition, out Quaternion cameraRotation)
    {
        Vector3 lookTarget = playerTransform.position + Vector3.up * dialogueCameraLookHeight;
        cameraPosition = playerTransform.position
            + (playerTransform.forward * dialogueCameraDistance)
            + (playerTransform.right * dialogueCameraSideOffset)
            + (Vector3.up * dialogueCameraHeight);

        Vector3 lookDirection = lookTarget - cameraPosition;
        if (lookDirection.sqrMagnitude < 0.0001f)
        {
            lookDirection = -playerTransform.forward;
        }

        cameraRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
    }

    private IEnumerator PanCameraToDialogueCloseup()
    {
        if (dialogueCameraTransform == null || playerTransform == null)
        {
            yield break;
        }

        savedCameraPosition = dialogueCameraTransform.position;
        savedCameraRotation = dialogueCameraTransform.rotation;

        GetDialogueCloseupPose(out Vector3 targetPosition, out Quaternion targetRotation);

        float duration = Mathf.Max(0.01f, dialogueCameraPanDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float blend = Mathf.Clamp01(elapsed / duration);

            GetDialogueCloseupPose(out Vector3 currentTargetPosition, out Quaternion currentTargetRotation);

            dialogueCameraTransform.position = Vector3.Lerp(savedCameraPosition, currentTargetPosition, blend);
            dialogueCameraTransform.rotation = Quaternion.Slerp(savedCameraRotation, currentTargetRotation, blend);

            yield return null;
        }

        GetDialogueCloseupPose(out Vector3 finalPosition, out Quaternion finalRotation);
        dialogueCameraTransform.position = finalPosition;
        dialogueCameraTransform.rotation = finalRotation;
    }

    private IEnumerator MaintainDialogueCloseupCamera()
    {
        while (isRunningDialogue)
        {
            if (dialogueCameraTransform != null && playerTransform != null)
            {
                GetDialogueCloseupPose(out Vector3 cameraPosition, out Quaternion cameraRotation);
                dialogueCameraTransform.position = cameraPosition;
                dialogueCameraTransform.rotation = cameraRotation;
            }

            yield return null;
        }
    }

    private IEnumerator PanCameraBackToGameplay()
    {
        if (dialogueCameraTransform == null)
        {
            yield break;
        }

        Vector3 startPosition = dialogueCameraTransform.position;
        Quaternion startRotation = dialogueCameraTransform.rotation;

        float duration = Mathf.Max(0.01f, dialogueCameraPanDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float blend = Mathf.Clamp01(elapsed / duration);

            dialogueCameraTransform.position = Vector3.Lerp(startPosition, savedCameraPosition, blend);
            dialogueCameraTransform.rotation = Quaternion.Slerp(startRotation, savedCameraRotation, blend);

            yield return null;
        }

        dialogueCameraTransform.position = savedCameraPosition;
        dialogueCameraTransform.rotation = savedCameraRotation;

        Player player = playerTransform != null ? playerTransform.GetComponent<Player>() : null;
        if (player != null)
        {
            player.SyncCameraPitchFromTransform();
        }
    }
}
