using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueManager manager;
    public string sectionName;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.StartDialogue(sectionName);
            Destroy(gameObject);
        }
    }
}