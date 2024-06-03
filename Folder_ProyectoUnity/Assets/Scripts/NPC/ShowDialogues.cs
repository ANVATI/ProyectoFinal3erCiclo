using UnityEngine;
public class ShowDialogue : MonoBehaviour
{
    public DialogueManager dialogueManager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag  == "Player")
        {
            dialogueManager.StartDialogue();
        }
    }
}
