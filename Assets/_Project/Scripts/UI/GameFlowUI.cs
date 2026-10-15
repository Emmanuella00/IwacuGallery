using UnityEngine;

/// <summary>
/// Controls the start and completion screens and pauses player movement while either is open.
/// Runs after the PlayerController's default Start method so the opening screen keeps control.
/// </summary>
[DefaultExecutionOrder(100)]
public class GameFlowUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private ProgressManager progressManager;

    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject completionPanel;

    private void Awake()
    {
        if (startPanel != null) startPanel.SetActive(true);
        if (completionPanel != null) completionPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (progressManager != null)
            progressManager.AllDiscovered += ShowCompletion;
    }

    private void OnDisable()
    {
        if (progressManager != null)
            progressManager.AllDiscovered -= ShowCompletion;
    }

    private void Start()
    {
        if (player != null)
            player.SetControlEnabled(false);
    }

    /// <summary>Connect this to the Start button's On Click event.</summary>
    public void BeginGallery()
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (completionPanel != null) completionPanel.SetActive(false);
        if (player != null) player.SetControlEnabled(true);
    }

    private void ShowCompletion()
    {
        if (completionPanel != null) completionPanel.SetActive(true);
        if (player != null) player.SetControlEnabled(false);
    }

    /// <summary>Connect this to the completion screen's Play Again button.</summary>
    public void PlayAgain()
    {
 

        if (completionPanel != null) completionPanel.SetActive(false);
        if (startPanel != null) startPanel.SetActive(true);
        if (player != null)
            player.SetControlEnabled(false);
    }
}
