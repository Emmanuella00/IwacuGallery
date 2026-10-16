using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Fires a ray from the centre of the camera every frame to find which exhibit the player
/// is looking at. When the player presses Interact on an in-range exhibit, it raises
/// ExhibitInspected so the UI can open the story panel.
/// </summary>
public class ExhibitInteractor : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset controls;
    [SerializeField] private PlayerController player;

    [Header("Raycast")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float maxDistance = 8f;

    [Header("Feedback UI")]
    [SerializeField] private Image crosshair;
    [SerializeField] private TMP_Text promptLabel;
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color targetColor = new Color(0.2f, 0.9f, 0.5f);
    [SerializeField] private GameObject mobileInspectButton;

    private InputAction interactAction;
    private Exhibit currentTarget;

    /// <summary>Raised when the player presses Interact while looking at an in-range exhibit.</summary>
    public event Action<Exhibit> ExhibitInspected;

    private void Awake()
    {
        interactAction = controls.FindActionMap("Player", true).FindAction("Interact", true);
        if (mobileInspectButton != null)
            mobileInspectButton.SetActive(false);
    }

    private void OnEnable() => interactAction.performed += OnInteract;

    private void OnDisable() => interactAction.performed -= OnInteract;

    private void Update()
    {
        currentTarget = player.ControlEnabled ? FindTarget() : null;
        UpdateFeedback();
    }

    private Exhibit FindTarget()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        // Triggers are ignored so the large invisible zone around an exhibit
        // never blocks the ray. Walls still block it, so you can't inspect through them.
        if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            return null;
        }

        Exhibit exhibit = hit.collider.GetComponentInParent<Exhibit>();
        return exhibit != null && exhibit.IsInRange ? exhibit : null;
    }

    private void UpdateFeedback()
    {
        bool hasTarget = currentTarget != null;

        if (mobileInspectButton != null)
            mobileInspectButton.SetActive(hasTarget);

        promptLabel.color = hasTarget ? targetColor : idleColor;

        crosshair.color = hasTarget ? targetColor : idleColor;
        promptLabel.gameObject.SetActive(hasTarget);

        if (hasTarget)
        {
            promptLabel.text = $"{currentTarget.Data.Title}  -  Inspect";
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentTarget == null) return;

        Debug.Log($"Inspect: {currentTarget.Data.Title}"); // TEMP: remove once the story panel exists
        ExhibitInspected?.Invoke(currentTarget);
    }
}
