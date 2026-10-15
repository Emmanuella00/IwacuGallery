using UnityEngine;

/// <summary>
/// Starts gallery ambience after the player's first Inspect action, plays a chime when
/// an exhibit is first discovered, and fades drum audio in and out near the drum exhibit.
/// </summary>
public class GalleryAudioManager : MonoBehaviour
{
    [Header("Event sources")]
    [SerializeField] private ExhibitInteractor interactor;
    [SerializeField] private UIManager uiManager;

    [Header("Gallery ambience")]
    [SerializeField] private AudioSource ambientSource;

    [Header("Discovery chime")]
    [SerializeField] private AudioSource chimeSource;
    [SerializeField] private AudioClip discoveryChime;

    [Header("Drum proximity audio")]
    [SerializeField] private Exhibit drumExhibit;
    [SerializeField] private AudioSource drumSource;
    [SerializeField, Range(0f, 1f)] private float drumTargetVolume = 0.7f;
    [SerializeField, Min(0.01f)] private float fadeSpeed = 1.5f;

    private bool audioUnlocked;

    private void Awake()
    {
        // WebGL browsers may block audio until a player input has occurred.
        if (ambientSource != null)
        {
            ambientSource.playOnAwake = false;
            ambientSource.loop = true;
        }

        if (chimeSource != null)
            chimeSource.playOnAwake = false;

        if (drumSource != null)
        {
            drumSource.playOnAwake = false;
            drumSource.loop = true;
            drumSource.volume = 0f;
        }
    }

    private void OnEnable()
    {
        if (interactor != null)
            interactor.ExhibitInspected += UnlockAndStartAmbience;

        if (uiManager != null)
            uiManager.ExhibitDiscovered += PlayDiscoveryChime;
    }

    private void OnDisable()
    {
        if (interactor != null)
            interactor.ExhibitInspected -= UnlockAndStartAmbience;

        if (uiManager != null)
            uiManager.ExhibitDiscovered -= PlayDiscoveryChime;
    }

    private void Update()
    {
        if (drumExhibit == null || drumSource == null) return;

        float targetVolume = audioUnlocked && drumExhibit.IsInRange
            ? drumTargetVolume
            : 0f;

        drumSource.volume = Mathf.MoveTowards(
            drumSource.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime);

        if (targetVolume > 0f && !drumSource.isPlaying)
            drumSource.Play();
        else if (targetVolume <= 0f && drumSource.volume <= 0.001f && drumSource.isPlaying)
            drumSource.Stop();
    }

    private void UnlockAndStartAmbience(Exhibit exhibit)
    {
        if (audioUnlocked) return;

        audioUnlocked = true;
        if (ambientSource != null && !ambientSource.isPlaying)
            ambientSource.Play();
    }

    private void PlayDiscoveryChime(Exhibit exhibit)
    {
        if (chimeSource != null && discoveryChime != null)
            chimeSource.PlayOneShot(discoveryChime);
    }
}
