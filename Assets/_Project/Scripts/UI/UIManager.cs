using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Owns the story panel. Opens it when the player inspects an exhibit, pauses the player
/// while it is open, and reports the first time each exhibit's story is closed (discovered).
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ExhibitInteractor interactor;
    [SerializeField] private PlayerController player;

    [Header("Story panel")]
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private TMP_Text titleLabel;
    [SerializeField] private TMP_Text subtitleLabel;
    [SerializeField] private Image artImage;
    [SerializeField] private TMP_Text storyLabel;
    [SerializeField] private TMP_Text sourceLabel;
    [SerializeField] private Button closeButton;

    private Exhibit currentExhibit;

    /// <summary>Raised the first time the player finishes reading an exhibit.</summary>
    public event Action<Exhibit> ExhibitDiscovered;

    private void Awake()
    {
        storyPanel.SetActive(false);
        closeButton.onClick.AddListener(CloseStory);
    }

    private void OnEnable() => interactor.ExhibitInspected += OpenStory;

    private void OnDisable() => interactor.ExhibitInspected -= OpenStory;

    private void OpenStory(Exhibit exhibit)
    {
        currentExhibit = exhibit;
        ExhibitData data = exhibit.Data;

        titleLabel.text = data.Title;
        subtitleLabel.text = data.Subtitle;
        storyLabel.text = data.Story;
        sourceLabel.text = string.IsNullOrEmpty(data.SourceCredit) ? string.Empty : $"Source: {data.SourceCredit}";

        artImage.sprite = data.Image;
        artImage.gameObject.SetActive(data.Image != null);

        storyPanel.SetActive(true);
        player.SetControlEnabled(false);
    }

    private void CloseStory()
    {
        storyPanel.SetActive(false);
        player.SetControlEnabled(true);

        if (currentExhibit == null) return;

        bool firstTime = !currentExhibit.IsDiscovered;
        currentExhibit.MarkDiscovered();

        if (firstTime)
        {
            ExhibitDiscovered?.Invoke(currentExhibit);
        }

        currentExhibit = null;
    }
}
