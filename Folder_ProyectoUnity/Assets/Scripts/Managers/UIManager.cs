using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Slider staminaBar;
    public GameObject slider;
    public PlayerController playerController;
    public PlayerActions playerAction;
    public Slider lifeBar;
    public Slider RageBar;

    private void Start()
    {
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

    }

}
