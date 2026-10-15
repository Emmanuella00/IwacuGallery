using UnityEngine;

/// <summary>
/// One exhibit in the gallery. The trigger collider on this object detects when the
/// player is close enough; the raycast in ExhibitInteractor decides which exhibit
/// the player is actually looking at.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class Exhibit : MonoBehaviour
{
    [SerializeField] private ExhibitData data;
    [SerializeField] private GameObject glowRing;
    [SerializeField] private Color inRangeColor = new Color(1f, 0.8f, 0.2f);
    [SerializeField] private Color discoveredColor = new Color(0.2f, 0.8f, 0.5f);

    private Renderer ringRenderer;

    public ExhibitData Data => data;
    public bool IsInRange { get; private set; }
    public bool IsDiscovered { get; private set; }

    private void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;
        ringRenderer = glowRing.GetComponent<Renderer>();
        RefreshRing();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        IsInRange = true;
        RefreshRing();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        IsInRange = false;
        RefreshRing();
    }

    /// <summary>Called once the player has read the story.</summary>
    public void MarkDiscovered()
    {
        IsDiscovered = true;
        RefreshRing();
    }

    private void RefreshRing()
    {
        glowRing.SetActive(IsInRange);
        ringRenderer.material.color = IsDiscovered ? discoveredColor : inRangeColor;
    }
}