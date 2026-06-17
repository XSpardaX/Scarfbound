using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueManager manager;
    public string sectionName;

    private bool isWaitingForGround;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (CheckpointManager.IsRespawning) return;
        if (EndingState.isInEnding) return;
        if (isWaitingForGround) return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null && playerHealth.IsInvincible) return;

        Player player = other.GetComponentInParent<Player>();
        if (player == null) return;

        isWaitingForGround = true;
        StartCoroutine(WaitForGroundedThenStartDialogue(player));
    }

    private IEnumerator WaitForGroundedThenStartDialogue(Player player)
    {
        yield return new WaitUntil(() =>
            player == null ||
            (player.IsGrounded && !CheckpointManager.IsRespawning));

        if (player == null || CheckpointManager.IsRespawning || manager == null)
        {
            isWaitingForGround = false;
            yield break;
        }

        manager.StartDialogue(sectionName);
        Destroy(gameObject);
    }
}
