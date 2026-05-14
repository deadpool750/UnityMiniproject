using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Text UI")]
    [SerializeField] private TMP_Text shotsText;
    [SerializeField] private TMP_Text hitsText;
    [SerializeField] private TMP_Text ammoText;

    [Header("Reload UI")]
    [SerializeField] private GameObject reloadCircleObject;
    [SerializeField] private Image reloadCircleImage;
    [SerializeField] private TMP_Text reloadText;

    [Header("Ammo Box Prompt")]
    [SerializeField] private TMP_Text ammoBoxPromptText;

    private int shots;
    private int hits;

    private void Start()
    {
        UpdateShotsAndHits();
        HideReloadUI();
        HideAmmoBoxPrompt();
    }

    public void RegisterShot(bool hit)
    {
        shots++;

        if (hit)
        {
            hits++;
        }

        UpdateShotsAndHits();
    }

    public void UpdateAmmo(int currentAmmo, int magazineSize, int reserveAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + "/" + reserveAmmo;
        }
    }

    public void ShowReloadUI()
    {
        if (reloadCircleObject != null)
        {
            reloadCircleObject.SetActive(true);
        }

        if (reloadText != null)
        {
            reloadText.gameObject.SetActive(true);
            reloadText.text = "Reloading...";
        }

        if (reloadCircleImage != null)
        {
            reloadCircleImage.gameObject.SetActive(true);
            reloadCircleImage.fillAmount = 0f;
        }
    }

    public void UpdateReloadProgress(float progress)
    {
        if (reloadCircleImage != null)
        {
            reloadCircleImage.fillAmount = Mathf.Clamp01(progress);
        }
    }

    public void HideReloadUI()
    {
        if (reloadCircleObject != null)
        {
            reloadCircleObject.SetActive(false);
        }

        if (reloadText != null)
        {
            reloadText.gameObject.SetActive(false);
        }

        if (reloadCircleImage != null)
        {
            reloadCircleImage.fillAmount = 0f;
        }
    }

    public void ShowAmmoBoxPrompt(string message)
    {
        if (ammoBoxPromptText != null)
        {
            ammoBoxPromptText.gameObject.SetActive(true);
            ammoBoxPromptText.text = message;
        }
    }

    public void HideAmmoBoxPrompt()
    {
        if (ammoBoxPromptText != null)
        {
            ammoBoxPromptText.gameObject.SetActive(false);
        }
    }

    private void UpdateShotsAndHits()
    {
        if (shotsText != null)
        {
            shotsText.text = "Shots: " + shots;
        }

        if (hitsText != null)
        {
            hitsText.text = "Hits: " + hits;
        }
    }
}