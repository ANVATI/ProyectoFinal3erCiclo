using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    [Header("UI_Vida y Stamina")]
    public Slider staminaBar;
    public GameObject slider;
    public PlayerController playerController;
    public PlayerActions playerAction;
    public Slider lifeBar;
    public Slider RageBar;

    [Header("UI_Dialogos NPC y enemigos")]
    public Dialogue dialogue;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogueText;
    public Image dialogueImage1;
    public Image dialogueImage2;
    public GameObject dialoguePanel;
    private int currentDialogueIndex = 0;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        if (playerController == null)
        {
            playerController = PlayerController.Instance;
        }
    }

    private void Update()
    {
        if (playerController != null )
        {
            staminaBar.value = playerController.playerAttributes.Stamina;
            lifeBar.value = playerController.playerAttributes.Life;
            RageBar.value = playerController.playerAction.GetEnemyKillCount();

            if (playerController.playerAction.inRageMode)
            {
                slider.SetActive(true);
                RageBar.value = playerController.playerAction.GetRemainingRageDuration();
            }
            else
            {
                slider.SetActive(false);
            }
        }

        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextDialogue();
        }

    }
    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        currentDialogueIndex = 0;
        DisplayNextDialogue();
    }

    public void DisplayNextDialogue()
    {
        if (currentDialogueIndex < dialogue.dialogueEntries.Count)
        {
            characterNameText.text = dialogue.dialogueEntries[currentDialogueIndex].characterName;
            dialogueText.text = dialogue.dialogueEntries[currentDialogueIndex].dialogueText;
            dialogueImage1.sprite = dialogue.dialogueEntries[currentDialogueIndex].dialogueImage1;
            dialogueImage2.sprite = dialogue.dialogueEntries[currentDialogueIndex].dialogueImage2;
            currentDialogueIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }

}
